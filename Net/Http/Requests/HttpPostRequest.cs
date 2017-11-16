using System.IO;

namespace NightlyCode.Net.Http.Requests {

    /// <summary>
    /// request containing post data
    /// </summary>
    public class HttpPostRequest : HttpRequest {
        readonly Stream datastream;

        /// <summary>
        /// creates a new <see cref="HttpPostRequest"/>
        /// </summary>
        /// <param name="datastream"></param>
        public HttpPostRequest(Stream datastream, string resource)
            : base("POST", resource) {
            this.datastream = datastream;
        }

        /// <summary>
        /// length of content
        /// </summary>
        public int ContentLength => int.Parse(GetHeader("Content-Length"));

        /// <summary>
        /// determines whether the request contains the continue header
        /// </summary>
        public bool HasContinue => GetHeader("Expect") == "100-continue";

        /// <summary>
        /// get body of post request
        /// </summary>
        /// <returns>stream containing post data</returns>
        public Stream GetBody() {
            return datastream;
        }
    }
}