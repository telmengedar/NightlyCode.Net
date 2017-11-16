using System.Net;
using NightlyCode.Net.TCP.Native;

namespace NightlyCode.Net.TCP {

    /// <summary>
    /// information about an open port in the system
    /// </summary>
    public class ConnectionInformation {

        /// <summary>
        /// creates a new connection information
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="localAddress"></param>
        /// <param name="localPort"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="remotePort"></param>
        /// <param name="process"></param>
        /// <param name="state"></param>
        public ConnectionInformation(Protocol protocol, IPAddress localAddress, int localPort, IPAddress remoteAddress, int remotePort, int process, ConnectionState state) {
            Protocol = protocol;
            LocalAddress = localAddress;
            LocalPort = localPort;
            RemoteAddress = remoteAddress;
            RemotePort = remotePort;
            Process = process;
            State = state;
        }

        /// <summary>
        /// protocol of connection
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// address on local computer
        /// </summary>
        public IPAddress LocalAddress { get; }

        /// <summary>
        /// port on local computer
        /// </summary>
        public int LocalPort { get; }

        /// <summary>
        /// remote address
        /// </summary>
        public IPAddress RemoteAddress { get; }

        /// <summary>
        /// remote port
        /// </summary>
        public int RemotePort { get; }

        /// <summary>
        /// process which opened the connection
        /// </summary>
        public int Process { get; private set; }

        /// <summary>
        /// state of the connection
        /// </summary>
        public ConnectionState State { get; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            return $"{LocalAddress}:{LocalPort} -> {RemoteAddress}:{RemotePort} ({State}), PID: {Process}, {Protocol}";
        }
    }
}