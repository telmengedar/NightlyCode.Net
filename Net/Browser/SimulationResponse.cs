using System.IO;
using System.Text;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// response of a load in <see cref="BrowserSimulator"/>
    /// </summary>
    public class SimulationResponse {

        /// <summary>
        /// where to redirect browser to
        /// </summary>
        public string RedirectionTarget { get; set; }

        /// <summary>
        /// stream containing data of response
        /// </summary>
        public Stream ResponseData { get; set; }

        /// <summary>
        /// encoding of response data
        /// </summary>
        public Encoding ResponseEncoding { get; set; }
    }
}