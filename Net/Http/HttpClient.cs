using System;
using System.IO;
using System.Net.Sockets;

namespace NightlyCode.Net.Http {

    /// <summary>
    /// client used to send responses to a http connection
    /// </summary>
    public class HttpClient {
        readonly TcpClient client;
        readonly StreamWriter writer;

        /// <summary>
        /// creates a new <see cref="HttpClient"/>
        /// </summary>
        /// <param name="client"></param>
        public HttpClient(TcpClient client) {
            this.client = client;
            writer = new StreamWriter(client.GetStream()) {
                AutoFlush = true
            };
        }

        internal bool KeepAlive { get; set; }

        /// <summary>
        /// writes a header to the http stream
        /// </summary>
        /// <param name="key">header key</param>
        /// <param name="value">header value</param>
        public void WriteHeader(string key, string value) {
            writer.WriteLine($"{key}: {value}");
        }

        /// <summary>
        /// writes a http status (should be the start of every header)
        /// </summary>
        /// <param name="status">status code</param>
        /// <param name="text">status text</param>
        /// <param name="server">name of server</param>
        public void WriteStatus(int status, string text, string server=null) {
            writer.WriteLine("HTTP/1.1 {0} {1}", status, text);
            WriteHeader("Date", $"{DateTime.Now:ddd, dd MMM yy HH:mm:ss} GMT");
            WriteHeader("Server", server ?? "Nightlycode-Webserver/1.0");
        }

        /// <summary>
        /// end writing of the header
        /// </summary>
        public void EndHeader() {
            /*if (KeepAlive)
            {
                WriteHeader("Connection", "Keep-Alive");
                WriteHeader("Keep-Alive", "timeout=10, max=20");
            }
            else*/
            //WriteHeader("Connection", "Close");

            writer.WriteLine();
        }

        /// <summary>
        /// get the http stream to write additional data
        /// </summary>
        /// <returns></returns>
        public Stream GetStream() {
            return client.GetStream();
        }
    }
}