using System.Net;

namespace NightlyCode.Net.Server
{

    /// <summary>
    /// handles service requests
    /// </summary>
    public interface IServiceHandler<T>
    {

        /// <summary>
        /// handles a request to the service
        /// </summary>
        /// <param name="context">http context</param>
        T HandleRequest(HttpListenerContext context);
    }
}