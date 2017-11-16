using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NightlyCode.Net.TCP {

    /// <summary>
    /// proxy for tcp clients/servers
    /// </summary>
    /// <remarks>
    /// this routes all traffic between client and server and allows to do something with it
    /// </remarks>
    public class TCPProxy {
        readonly string targethost;
        readonly int targetport;
        static TcpListener listener;

        /// <summary>
        /// creates a new <see cref="TCPProxy"/>
        /// </summary>
        /// <param name="listenport">port where proxy should listen on</param>
        /// <param name="targethost">host where to route traffic to</param>
        /// <param name="targetport">port where to route traffic to</param>
        public TCPProxy(int listenport, string targethost, int targetport) {
            this.targethost = targethost;
            this.targetport = targetport;
            listener = new TcpListener(IPAddress.Any, listenport);
        }

        /// <summary>
        /// packet was received by an endpoint
        /// </summary>
        public event Action<ProxyPacket> PacketReceived;

        /// <summary>
        /// starts the proxy
        /// </summary>
        public void Start() {
            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        /// <summary>
        /// stops the proxy
        /// </summary>
        public void Stop() {
            listener.Stop();
        }

        void OnClientConnected(IAsyncResult ar)
        {
            listener.BeginAcceptTcpClient(OnClientConnected, null);
            TcpClient client = listener.EndAcceptTcpClient(ar);
            TcpClient proxy = new TcpClient(targethost, targetport);
            Task.Run(() => Proxy(client, proxy));
        }

        void Proxy(TcpClient client, TcpClient proxy)
        {
            byte[] buffer = new byte[65536];
            using (NetworkStream proxystream = proxy.GetStream())
            {
                using (NetworkStream clientstream = client.GetStream())
                {
                    while (client.Connected && proxy.Connected) {
                        bool available = false;
                        if (proxystream.DataAvailable)
                        {
                            int read = proxystream.Read(buffer, 0, 65536);
                            if (read > 0) {
                                
                                ProxyPacket packet = new ProxyPacket {
                                    EndPoint = proxy.Client.RemoteEndPoint,
                                    Data = buffer.Take(read).ToArray(),
                                    Source = proxy,
                                    Target = client
                                };
                                OnPacketReceived(packet);
                                if(packet.Forward && packet.Data.Length > 0)
                                    clientstream.Write(packet.Data, 0, packet.Data.Length);
                            }
                            available = true;
                        }

                        if (clientstream.DataAvailable)
                        {
                            int read = clientstream.Read(buffer, 0, 65536);
                            if (read > 0)
                            {
                                ProxyPacket packet = new ProxyPacket
                                {
                                    EndPoint = client.Client.RemoteEndPoint,
                                    Data = buffer.Take(read).ToArray(),
                                    Source = client,
                                    Target = proxy  
                                };
                                OnPacketReceived(packet);
                                if (packet.Forward && packet.Data.Length > 0)
                                    proxystream.Write(packet.Data, 0, packet.Data.Length);
                            }
                            available = true;
                        }

                        if(!available)
                            Thread.Sleep(50);
                    }
                }
            }
            client.Close();
            proxy.Close();
        }

        /// <summary>
        /// called when a packet was received by an endpoint
        /// </summary>
        /// <param name="packet">packet received</param>
        protected virtual void OnPacketReceived(ProxyPacket packet) {
            PacketReceived?.Invoke(packet);
        }
    }
}