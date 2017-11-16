using System.IO;
using System.Text;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// response of browser
    /// </summary>
    public class BrowserResponse {

        /// <summary>
        /// creates a new <see cref="BrowserResponse"/>
        /// </summary>
        /// <param name="response"></param>
        /// <param name="encoding"></param>
        public BrowserResponse(Stream response, Encoding encoding) {
            Encoding = encoding;
            Response = response;
        }

        /// <summary>
        /// encoding used in response
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// response stream
        /// </summary>
        public Stream Response { get; set; } 
    }
}