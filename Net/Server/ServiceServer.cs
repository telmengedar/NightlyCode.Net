using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NightlyCode.Net.Server
{

    /// <summary>
    /// basic server for cardlogin
    /// </summary>
    public abstract class ServiceServer<T> : HttpSysServer
    {
        readonly Dictionary<string, IServiceHandler<T>> handlers = new Dictionary<string, IServiceHandler<T>>();
        readonly HashSet<string> ignorelogginpaths = new HashSet<string>();

        /// <summary>
        /// creates a new service server
        /// </summary>
        /// <param name="errorhandler">handles errors (eg. logging)</param>
        /// <param name="prefixes">prefixes to listen on</param>
        protected ServiceServer(Action<object, string, Exception> errorhandler, params string[] prefixes)
            : base(errorhandler, prefixes) { }

        /// <summary>
        /// adds a path to be ignored for logging
        /// </summary>
        /// <remarks>
        /// mainly used for requests which are called frequently and would flood log file without any real information
        /// </remarks>
        /// <param name="path"></param>
        protected void AddLogginPathIgnore(string path)
        {
            ignorelogginpaths.Add(path);
        }

        /// <summary>
        /// adds a service to the server
        /// </summary>
        /// <param name="path">path for which to add a handler</param>
        /// <param name="handler">handler to add</param>
        public void AddService(string path, IServiceHandler<T> handler)
        {
            handlers[path] = handler;
        }

        /// <summary>
        /// removes a servicehandler from the server
        /// </summary>
        /// <param name="path"></param>
        public void RemoveService(string path)
        {
            handlers.Remove(path);
        }

        IServiceHandler<T> GetService(string path)
        {
            IServiceHandler<T> handler;
            handlers.TryGetValue(path, out handler);
            return handler;
        }

        /// <summary>
        /// called when path is requested
        /// </summary>
        /// <param name="path">path which is requested on server</param>
        /// <param name="remote">client which requests the resource</param>
        protected virtual void OnRequestPath(string path, string remote) {   
        }

        /// <summary>
        /// handles a response
        /// </summary>
        /// <remarks>
        /// use this method to serialize the response and send it to the client
        /// </remarks>
        /// <param name="context">http context</param>
        /// <param name="response">response to send to client</param>
        protected abstract void OnResponse(HttpListenerContext context, T response);

        /// <summary>
        /// allows derived servers to process the request before service handlers are used
        /// </summary>
        /// <param name="context">context of request</param>
        /// <param name="path">requested path</param>
        /// <returns>base implementation always returns false</returns>
        protected virtual bool ProcessRequest(HttpListenerContext context, string path)
        {
            return false;
        }

        /// <summary>
        /// handles a request to the service server
        /// </summary>
        /// <param name="context">http context</param>
        protected override void HandleRequest(HttpListenerContext context)
        {
            string path = HttpExtensions.GetRelativePath(context.Request.Url.AbsoluteUri, Prefixes);
            if (!ignorelogginpaths.Contains(path))
                OnRequestPath(path, context.Request?.RemoteEndPoint?.ToString());

            if (ProcessRequest(context, path))
                return;

            if(context.Request.HttpMethod == "OPTIONS") {
                OnResponse(context, default(T));
            }
            else {
                IServiceHandler<T> handler = GetService(path);
                if(handler == null)
                    throw new ServiceServerException($"Service handler for '{path}' not found");

                T response;
                try {
                    response = handler.HandleRequest(context);
                }
                catch(Exception e) {
                    throw new ServiceServerException("Error handling request", e);
                }
                OnResponse(context, response);
            }
        }

        /// <summary>
        /// domain for which to accept requests
        /// </summary>
        protected override string Domain => Prefixes.First();
    }
}