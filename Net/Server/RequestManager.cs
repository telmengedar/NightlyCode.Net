using System.Collections.Generic;
using System.Net;

namespace NightlyCode.Net.Server {

    /// <summary>
    /// manages request handlers for paths
    /// </summary>
    public class RequestManager {
        readonly Dictionary<string, IRequestHandler> handlers=new Dictionary<string, IRequestHandler>();

        /// <summary>
        /// adds a handler to the manager
        /// </summary>
        /// <param name="path"></param>
        /// <param name="handler"></param>
        public void AddHandler(string path, IRequestHandler handler) {
            handlers[path] = handler;
        }

        /// <summary>
        /// handles a request for a path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        public void HandleRequest(string path, HttpListenerContext context) {
            handlers[path].HandleRequest(context);
        }
    }
}