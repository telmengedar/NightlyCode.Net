using System;
using System.Net;
using System.Net.Sockets;

namespace NightlyCode.Net.TCP {

    /// <summary>
    /// server for tcp clients
    /// </summary>
    public class TcpServer {
        readonly TcpListener listener;

        /// <summary>
        /// creates a new <see cref="TcpServer"/>
        /// </summary>
        /// <param name="port">port to listen on</param>
        public TcpServer(int port)
            : this(IPAddress.Any, port) {}

        /// <summary>
        /// creates a new <see cref="TcpServer"/>
        /// </summary>
        /// <param name="bindingaddress">address to bind</param>
        /// <param name="port">port to listen on</param>
        public TcpServer(IPAddress bindingaddress, int port) {
            listener = new TcpListener(bindingaddress, port);
        }

        /// <summary>
        /// triggered when a client did connect
        /// </summary>
        public event Action<TcpClient> ClientConnected;

        /// <summary>
        /// starts listening for tcp connections
        /// </summary>
        public void Start() {
            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        void OnClientConnected(IAsyncResult ar) {
            TcpClient client=null;
            try {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch(SocketException) {
                return;
            }
            catch(ObjectDisposedException) {
                return;
            }
            catch(Exception e) {
                Logger.Error(this, "Error connecting client", e);
            }

            listener.BeginAcceptTcpClient(OnClientConnected, null);
            if(client != null)
                ClientConnected?.Invoke(client);
        }

        /// <summary>
        /// stops the tcp server
        /// </summary>
        public void Stop() {
            listener.Stop();
        }
    }
}