using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NightlyCode.Net.TCP {

    /// <summary>
    /// methods helping with connections
    /// </summary>
    public static class Connection {

        /// <summary>
        /// determines whether the host is online
        /// </summary>
        /// <param name="host">host to analyse</param>
        /// <param name="port">port to analyse</param>
        /// <param name="timeout">timeout before host will be returned as offline</param>
        /// <returns></returns>
        public static bool IsOnline(string host, int port, TimeSpan? timeout = null) {
            if(timeout == null)
                timeout = TimeSpan.FromSeconds(3.0);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                IAsyncResult result = socket.BeginConnect(host, port, null, null);
                return result.AsyncWaitHandle.WaitOne((int)timeout.Value.TotalMilliseconds, true);
            }
            finally {
                //You should always close the socket!
                socket.Close();

            }
        }

        /// <summary>
        /// parses the given host name to an ip-host or determines the ip host of the given host via dns-lookup
        /// </summary>
        /// <param name="host">host</param>
        /// <returns>ipaddress</returns>
        public static IPAddress GetIPAddress(string host) {
            if(host == null)
                throw new ArgumentNullException(nameof(host));
            if(string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("host is empty");
            IPAddress address;
            if(!IPAddress.TryParse(host, out address)) {
                IPHostEntry entry = Dns.GetHostEntry(host);
                if(entry?.AddressList == null || entry.AddressList.Length == 0)
                    throw new InvalidDataException($"No ip addresses found for host '{host}'");
                address = entry.AddressList.FirstOrDefault(item => item.ToString() == host);
                if(address == null && entry.AddressList.Length > 0)
                    address = entry.AddressList[0];
            }
            if(address == null)
                throw new InvalidDataException($"Response from dns for host '{host}' contained an IP-host which is null");
            return address;
        }

        /// <summary>
        /// get a free tcp port
        /// </summary>
        /// <returns></returns>
        public static int GetFreePort(Func<int, bool> filter=null) {
            return GetFreePorts(filter).First();
        }

        /// <summary>
        /// free ports in system
        /// </summary>
        public static IEnumerable<int> GetFreePorts(Func<int, bool> filter = null) {
            HashSet<int> used = new HashSet<int>(UsedPorts);
            for(int i = 1024; i < 65535; ++i)
                if(!used.Contains(i) && (filter == null || filter(i)))
                    yield return i;
        }

        /// <summary>
        /// ports used by other applications
        /// </summary>
        public static IEnumerable<int> UsedPorts {
            get {
                foreach(TcpConnectionInformation tcpinformation in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections())
                    yield return tcpinformation.LocalEndPoint.Port;
            }
        }

    }
}