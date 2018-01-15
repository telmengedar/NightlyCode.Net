using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using NightlyCode.Net.Http.Requests;

namespace NightlyCode.Net.Http {

    /// <summary>
    /// server for http requests
    /// </summary>
    public class HttpServer : IDisposable {
        readonly TcpListener server;
        bool listening;
        readonly RequestParser parser = new RequestParser();

        /// <summary>
        /// creates a new http server
        /// </summary>
        /// <param name="address">address to bind server to</param>
        /// <param name="port">port where to listen on</param>
        public HttpServer(IPAddress address, int port) {
            server = new TcpListener(address, port);
        }

        /// <summary>
        /// triggered when a new request was received
        /// </summary>
        public event Action<HttpClient, HttpRequest> Request;

        /// <summary>
        /// starts listening for http messages
        /// </summary>
        public void Start() {
            if(listening)
                throw new Exception("Http Server already started");

            server.Start();
            server.BeginAcceptTcpClient(ClientConnected, null);
            listening = true;
            Logger.Info(this, "Http Server started");
        }

        void ClientConnected(IAsyncResult ar) {
            
            TcpClient client;
            try {
                client = server.EndAcceptTcpClient(ar);
                server.BeginAcceptTcpClient(ClientConnected, null);
            }
            catch(ObjectDisposedException) {
                return;
            }
            catch(Exception e) {
                Logger.Error(this, "Error accepting tcp client", e);
                return;
            }

            HttpClient httpclient = new HttpClient(client);

            do {
                try {
                    HttpRequest request = parser.Parse(client.GetStream());
                    if(request.HasHeader("Connection")) {
                        httpclient.KeepAlive = request.GetHeader("Connection").ToLower() == "keep-alive";
                    }

                    Request?.Invoke(httpclient, request);
                }
                catch(MissingHeaderException) {
                    break;
                }
                catch(IOException) {
                    return;
                }
                catch(Exception e) {
                    Logger.Error(this, "Error parsing request", e);
                    try {
                        httpclient.WriteStatus(500, "Internal Server Error");
                        httpclient.EndHeader();
                        client.Close();
                    }
                    catch (Exception ex) {
                        Logger.Error(this, "Error sending error response", ex);
                    }
                }
            }
            while(httpclient.KeepAlive && client.Connected);
            
            client.Close();
        }

        /// <summary>
        /// stops the server
        /// </summary>
        public void Stop() {
            if(!listening)
                throw new InvalidOperationException("Server not running");

            try {
                server.Stop();
            }
            catch(Exception e) {
                Logger.Warning(this, "Error stopping server", e.ToString());
            }
            
            listening = false;
            Logger.Info(this, "Http Server stopped");
        }

        /// <summary>
        /// determines whether the server is running
        /// </summary>
        public bool Running => listening;

        /// <summary>
        /// port where the server is listening
        /// </summary>
        public int Port => (server.LocalEndpoint as IPEndPoint)?.Port ?? 0;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        void IDisposable.Dispose() {
            if(Running)
                Stop();
        }
    }
}