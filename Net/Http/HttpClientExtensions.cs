using System.IO;
using System.Text;
using NightlyCode.Net.Http.Requests;

namespace NightlyCode.Net.Http {

    /// <summary>
    /// extension methods for <see cref="HttpClient"/>
    /// </summary>
    public static class HttpClientExtensions {

        /// <summary>
        /// serves a resource to the http client
        /// </summary>
        /// <param name="client">client to serve resource to</param>
        /// <param name="mimetype">mimetype as file extension for resource (optional)</param>
        /// <param name="resource">resource stream</param>
        public static void ServeResource(this HttpClient client, Stream resource, string mimetype=null) {
            client.WriteStatus(200, "OK");
            client.WriteHeader("Content-Type", MimeTypes.GetMimeType(mimetype));
            client.WriteHeader("Content-Length", resource.Length.ToString());
            client.EndHeader();
            resource.CopyTo(client.GetStream());
            client.GetStream().Flush();
        }

        /// <summary>
        /// serves a resource to the http client
        /// </summary>
        /// <param name="client">client to serve resource to</param>
        /// <param name="mimetype">mimetype as file extension for resource (optional)</param>
        /// <param name="data">data to serve</param>
        public static void ServeData(this HttpClient client, string data, string mimetype = null) {
            ServeData(client, Encoding.UTF8.GetBytes(data), mimetype);
        }

        /// <summary>
        /// serves a resource to the http client
        /// </summary>
        /// <param name="client">client to serve resource to</param>
        /// <param name="mimetype">mimetype as file extension for resource (optional)</param>
        /// <param name="data">data to serve</param>
        public static void ServeData(this HttpClient client, byte[] data, string mimetype = null)
        {
            client.WriteStatus(200, "OK");
            client.WriteHeader("Content-Type", MimeTypes.GetMimeType(mimetype));
            client.WriteHeader("Content-Length", data.Length.ToString());
            client.EndHeader();
            client.GetStream().Write(data, 0, data.Length);
            client.GetStream().Flush();
        }

        /// <summary>
        /// get content of post request
        /// </summary>
        /// <param name="client">client which is used to send data</param>
        /// <param name="request">initial request</param>
        /// <returns>stream containing post data</returns>
        public static Stream GetContent(this HttpClient client, HttpRequest request) {
            if((request as HttpPostRequest)?.HasContinue ?? false) {
                client.WriteStatus(100, "Continue");
                client.EndHeader();
            }

            return client.GetStream();
        }

        /// <summary>
        /// read full body of a post request
        /// </summary>
        /// <param name="client">client which sent the request</param>
        /// <param name="request">post request containing body</param>
        /// <returns>body of post request</returns>
        public static byte[] ReadBody(this HttpClient client, HttpRequest request) {
            HttpPostRequest post=request as HttpPostRequest;
            if(post == null)
                return null;

            Stream stream = GetContent(client, request);
            byte[] data = new byte[post.ContentLength];
            int offset = 0;
            while(offset < data.Length) {
                int read = stream.Read(data, offset, data.Length - offset);
                if(read > -1)
                    offset += read;
            }
            return data;
        }
    }
}