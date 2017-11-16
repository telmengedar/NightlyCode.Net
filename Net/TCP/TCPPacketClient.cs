using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NightlyCode.Net.TCP {

	/// <summary>
	/// a communication client for the tcp protocol
	/// </summary>
	public abstract class TCPPacketClient<TPacket> : IDisposable {
	    Socket socket;
	    readonly object writelock = new object();

	    readonly string host;
	    readonly int port;

	    /// <summary>
	    /// creates a new <see cref="TCPPacketClient{TPacket}"/>
	    /// </summary>
	    /// <param name="socket">socket to wrap</param>
	    protected TCPPacketClient(Socket socket) {
			if (socket == null)
				throw new ArgumentNullException(nameof(socket));
	        host = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
	        port = ((IPEndPoint)socket.RemoteEndPoint).Port;
			this.socket = socket;
		}

        /// <summary>
        /// creates a new <see cref="TCPPacketClient{TPacket}"/>
        /// </summary>
        /// <param name="host">host to which to connect</param>
        /// <param name="port">port to which to connect</param>
        protected TCPPacketClient(string host, int port) {
	        this.host = host;
	        this.port = port;
	    }

        /// <summary>
        /// deserializes packet data
        /// </summary>
        /// <param name="data">data to be deserialized to a packet</param>
        /// <returns>deserialized packet</returns>
        protected abstract TPacket Deserialize(byte[] data);

        /// <summary>
        /// serializes packet data to be send by tcp client
        /// </summary>
        /// <param name="packet">packet to be serialized</param>
        /// <returns>serialized packet</returns>
	    protected abstract byte[] Serialize(TPacket packet);

        /// <summary>
        /// triggered when the connection state changes
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// triggered when a packet was received
        /// </summary>
	    public event Action<TPacket> PacketReceived;

		/// <summary>
		/// determines if the client is connected
		/// </summary>
		public bool Connected => socket != null && socket.Connected;

	    void ReadThread() {
			while (socket?.Connected??false) {
				try {
				    OnPacketReceived(ReadPacket(ReadLength()));
				} catch (Exception) {
					Disconnect();
				}
			}
		}

        void OnPacketReceived(TPacket packet) {
            PacketReceived?.Invoke(packet);
        }

        void OnDisconnect() {
            Disconnected?.Invoke();
        }

	    byte[] Read(int length) {
	        byte[] buffer = new byte[length];
	        int read = 0;
	        while(socket.Connected&&read<length) {
	            int status = socket.Receive(buffer, read, length - read, SocketFlags.None);
	            if(status == 0)
	                Disconnect();
	            else read += status;
	        }
	        return buffer;
	    }

	    int ReadLength() {
	        return BitConverter.ToInt32(Read(4), 0);
	    }

	    TPacket ReadPacket(int size) {
	        return Deserialize(Read(size));
	    }

		/// <summary>
		/// disconnects the client
		/// </summary>
		public void Disconnect() {
			if (Connected)
				socket.Disconnect(false);

            OnDisconnect();
		}

		/// <summary>
		/// connects the client
		/// </summary>
		public void Connect() {
			if (!Connected)
				socket = CreateSocket();
		}

		/// <summary>
		/// creates a new socket
		/// </summary>
		/// <remarks>
		/// called in <see cref="Connect"/> if no socket exists
		/// </remarks>
		Socket CreateSocket() {
			IPAddress address = Connection.GetIPAddress(host);
			Socket newsocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			newsocket.Connect(host, port);
			return newsocket;
		}

		/// <summary>
		/// starts the tcp communication client
		/// </summary>
		public void Start() {
			if(!Connected)
				Connect();

		    new Task(ReadThread).Start();
		}

		/// <summary>
		/// sends the specified packet to the server
		/// </summary>
		/// <param name="packet">packet to send</param>
		public void Send(TPacket packet) {
		    if(!Connected)
		        throw new InvalidOperationException("Client not connected.");

		    byte[] data = Serialize(packet);
		    lock(writelock) {
		        Write(BitConverter.GetBytes(data.Length));
		        Write(data);
		    }
		}

	    void Write(byte[] data) {
	        int written = 0;
	        while(socket.Connected && written < data.Length) {
	            int status = socket.Send(data, written, data.Length - written, SocketFlags.None);
	            if(status == 0)
	                Disconnect();
	            else written += status;
	        }
	    }

	    /// <summary>
		/// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
	        socket?.Dispose();
	    }
	}
}
