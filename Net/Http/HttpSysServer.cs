using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NightlyCode.Net.Http {

    /// <summary>
    /// http server using .NET http listener
    /// </summary>
    public abstract class HttpSysServer {
        readonly HttpListener listener = new HttpListener();
        readonly Action<object, string, Exception> errorhandler;

        /// <summary>
        /// creates a new download server for updates
        /// </summary>
        protected HttpSysServer(Action<object, string, Exception> errorhandler, params string[] prefixes) {
            this.errorhandler = errorhandler;
            foreach(string prefix in prefixes)
                listener.Prefixes.Add(prefix);
            listener.AuthenticationSchemeSelectorDelegate += SelectAuthenticationScheme;
        }

        /// <summary>
        /// prefixes where http server listens on
        /// </summary>
        protected IEnumerable<string> Prefixes => listener.Prefixes;

        /// <summary>
        /// selects authentication scheme for specified request
        /// </summary>
        /// <remarks>
        /// the basic implementation allows access for all requests, override this method to present authentication or deny access completely
        /// </remarks>
        /// <param name="httprequest">request to evaluate</param>
        /// <returns><see cref="AuthenticationSchemes"/> to use</returns>
        protected virtual AuthenticationSchemes SelectAuthenticationScheme(HttpListenerRequest httprequest) {
            return AuthenticationSchemes.Anonymous;
        }

        /// <summary>
        /// authenticates the request
        /// </summary>
        /// <param name="request">request to authenticate</param>
        /// <returns>true when access for this request is allowed, false otherwise</returns>
        protected virtual bool Authenticate(HttpListenerContext request) {
            return true;
        }

        /// <summary>
        /// domain to present when doing http authenticate
        /// </summary>
        protected virtual string Domain => "nightlycode.de";

        /// <summary>
        /// starts the http server
        /// </summary>
        public void Start() {
            OnStart();
            listener.Start();
            new Task(WaitForConnections).Start();
        }

        /// <summary>
        /// stops the http server
        /// </summary>
        public void Stop() {
            OnStop();
            listener.Stop();
        }

        /// <summary>
        /// called when <see cref="HttpSysServer"/> starts
        /// </summary>
        protected virtual void OnStart() {
        }

        /// <summary>
        /// called when <see cref="HttpSysServer"/> stops
        /// </summary>
        protected virtual void OnStop() {
        }

        void WaitForConnections() {
            while(listener.IsListening) {
                try {
                    HttpListenerContext context = listener.GetContext();
                    new Task(() => InternalHandleRequest(context)).Start();
                }
                catch(Exception e) {
                    errorhandler(this, "Error waiting for connection", e);
                }
            }
        }

        void InternalHandleRequest(HttpListenerContext context) {
            try {
                if(SelectAuthenticationScheme(context.Request) != AuthenticationSchemes.Anonymous) {
                    if(!Authenticate(context)) {
                        context.Response.StatusCode = 401;
                        context.Response.StatusDescription = "Access denied";
                        context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"" + Domain + "\"");
                    }
                    else
                        HandleRequest(context);
                }
                else
                    HandleRequest(context);
            }
            catch(HttpListenerException e) {
                errorhandler(this, "ErrorCode " + e.ErrorCode + ". Propably unexpected connection close.", null);
            }
            catch(Exception e) {
                errorhandler(this, "Error handling request", e);
            }
            context.Response.Close();
        }

        /// <summary>
        /// handles a request
        /// </summary>
        /// <param name="context">context wrapping all request information</param>
        protected abstract void HandleRequest(HttpListenerContext context);
    }
}