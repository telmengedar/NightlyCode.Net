using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NightlyCode.Net.TCP {

	/// <summary>
	/// a tcp server
	/// </summary>
	public class TCPPacketServer<TClient, TPacket>
        where TClient : TCPPacketClient<TPacket>
    {
	    readonly IPAddress address;
	    readonly int port;
		bool listening;
	    
	    TcpListener listener;

	    /// <summary>
	    /// creates a new <see cref="TCPPacketServer{TClient,TPacket}"/>
	    /// </summary>
	    /// <param name="port"></param>
	    public TCPPacketServer(int port)
	        : this(IPAddress.Any, port)
        {
	    }

	    /// <summary>
	    /// ctor
	    /// </summary>
	    /// <param name="address"></param>
	    /// <param name="port"></param>
	    public TCPPacketServer(IPAddress address, int port) {
	        this.address = address;
	        this.port = port;
	    }

        TcpListener Listener => listener ?? (listener = new TcpListener(address, port));

	    /// <summary>
        /// port where to listen on
        /// </summary>
	    public int Port => port;

	    /// <summary>
		/// starts the tcp server
		/// </summary>
		public void Start() {
            Listener.Start();
	        new Task(AcceptThread).Start();
		}

		/// <summary>
		/// stops the tcp server
		/// </summary>
		public void Stop() {
		    if(listener == null)
		        return;
			listening = false;
            Listener.Stop();
		}

	    /// <summary>
	    /// determines if the tcp server is listening
	    /// </summary>
	    public bool IsListening => listening;

	    /// <summary>
		/// accepts a new client connection
		/// </summary>
		void AcceptThread() {
			listening = true;
			while (listening) {
				try {
                    Socket clientsocket = Listener.AcceptSocket();
                    TClient client = (TClient)Activator.CreateInstance(typeof(TClient), clientsocket);
				    OnClientConnected(client);
				} catch(Exception) {
					listening = false;
				}
			}
		}

        void OnClientConnected(TClient client) {
            ClientConnected?.Invoke(client);
        }

	    /// <summary>
	    /// triggered when a new client has connected
	    /// </summary>
	    public event Action<TClient> ClientConnected;
	}
}
