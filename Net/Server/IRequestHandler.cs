using System.Net;

namespace NightlyCode.Net.Server {

    /// <summary>
    /// interface for a request handler
    /// </summary>
    public interface IRequestHandler {

        /// <summary>
        /// handles a request
        /// </summary>
        /// <param name="context"></param>
        void HandleRequest(HttpListenerContext context);
    }
}