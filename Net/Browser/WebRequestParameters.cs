using System.Net;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// parameters for web request
    /// </summary>
    public class WebRequestParameters {

        /// <summary>
        /// referer from which request was initiated
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// credentials for http authentication
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// query parameters
        /// </summary>
        public Parameter[] Query { get; set; }

        /// <summary>
        /// form parameters
        /// </summary>
        public Parameter[] FormData { get; set; }
    }
}