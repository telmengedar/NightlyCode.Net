using System;
using System.Net;
using System.Net.Sockets;

namespace NightlyCode.Net.UDP
{

    /// <summary>
    /// server providing data of a UDP socket
    /// </summary>
	public class UDPServer : IDisposable
    {
        readonly int port;

        readonly UdpClient udpclient;
        readonly IPEndPoint broadcast;

        readonly object listenlock = new object();
        bool islistening;
        bool isrunning;

        /// <summary>
		/// creates a new <see cref="UDPServer"/>
		/// </summary>
		public UDPServer(int port)
        {
            this.port = port;
            udpclient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            broadcast = new IPEndPoint(IPAddress.Broadcast, port);
        }

        /// <summary>
        /// starts the <see cref="UDPServer"/>
        /// </summary>
        public void Start() {
            Listen();
            isrunning = true;
        }

        /// <summary>
        /// stops the <see cref="UDPServer"/>
        /// </summary>
        public void Stop() {
            isrunning = false;
            StopListening();
        }

        void Listen() {
            lock(listenlock) {
                if(islistening)
                    return;

                udpclient.BeginReceive(ReceiveData, null);
                islistening = true;
            }
        }

        /// <summary>
        /// broadcasts data
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(byte[] data) {
            Send(broadcast, data);
        }

        /// <summary>
        /// sends data to a specific endpoint
        /// </summary>
        /// <param name="endpoint">endpoint to send data to</param>
        /// <param name="data">data to send</param>
        public void Send(IPEndPoint endpoint, byte[] data) {
            using(UdpClient client = new UdpClient()) {
                client.Send(data, data.Length, endpoint);
            }
        }

        void ReceiveData(IAsyncResult ar) {
            IPEndPoint clientendpoint = new IPEndPoint(IPAddress.Any, port);
            lock(listenlock) {
                try {
                    byte[] data = udpclient.EndReceive(ar, ref clientendpoint);
                    OnDataReceived(clientendpoint, data);
                }
                catch(Exception) {
                    
                }
                islistening = false;
            }

            // start listening again
            if(isrunning)
                Listen();
        }

        /// <summary>
        /// stops udp client
        /// </summary>
		void StopListening()
        {
            lock(listenlock) {
                if(islistening)
                    udpclient.Close();
            }
        }

        /// <summary>
        /// Triggered when data was received.
        /// </summary>
        public event Action<IPEndPoint, byte[]> DataReceived;

        void OnDataReceived(IPEndPoint remoteendpoint, byte[] data)
        {
            DataReceived?.Invoke(remoteendpoint, data);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return $"Server on port {port}";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        void IDisposable.Dispose()
        {
            try
            {
                StopListening();
            }
            catch (Exception e)
            {
                Logger.Error(this, "Unable to shut down server", e);
            }
        }
    }
}
