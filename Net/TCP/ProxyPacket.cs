using System.Net;
using System.Net.Sockets;

namespace NightlyCode.Net.TCP {

    /// <summary>
    /// packet received by <see cref="TCPProxy"/>
    /// </summary>
    public class ProxyPacket {


        /// <summary>
        /// endpoint which sent the data
        /// </summary>
        public EndPoint EndPoint { get; set; }

        /// <summary>
        /// data received by endpoint
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// whether to forward packet to other endpoint
        /// </summary>
        public bool Forward { get; set; } = true;

        /// <summary>
        /// source from which data was sent
        /// </summary>
        public TcpClient Source { get; set; }

        /// <summary>
        /// target to which data should be sent
        /// </summary>
        public TcpClient Target { get; set; }
    }
}