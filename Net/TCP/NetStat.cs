using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using NightlyCode.Net.TCP.Native;
using AddressFamily = NightlyCode.Net.TCP.Native.AddressFamily;

namespace NightlyCode.Net.TCP
{

    /// <summary>
    /// provides information about open ports
    /// </summary>
    public static class NetStat
    {
        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern int GetExtendedTcpTable(byte[] pTcpTable,
            ref int bufferlength,
            bool sort,
            int ipVersion,
            TCP_TABLE_CLASS tblClass,
            int reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        static extern int GetExtendedUdpTable(byte[] pUdpTable,
               out int dwOutBufLen, bool sort, int ipVersion,
               UDP_TABLE_CLASS tblClass, int reserved);

        static int GetPort(byte[] buffer, int offset) {
            return buffer[offset] << 8 | buffer[offset + 1];
        }

        static IEnumerable<ConnectionInformation> GetAllConnections(Protocol protocol) {
            // the tables have quite a different layout
            int bufferlength;
            int ret;
            byte[] v4buffer;
            byte[] v6buffer;

            switch(protocol) {
            case Protocol.TCP:
                    bufferlength = 0;
                    ret = GetExtendedTcpTable(null, ref bufferlength, true, (int)AddressFamily.Internet, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                    if (ret != 0 && ret != 122) // 122 insufficient buffer size
                    throw new Exception("bad ret on check " + ret);
                    v4buffer = new byte[bufferlength];
                    ret = GetExtendedTcpTable(v4buffer, ref bufferlength, true, (int)AddressFamily.Internet, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                    if(ret != 0)
                        throw new Exception("bad ret " + ret);

                    bufferlength = 0;
                    ret = GetExtendedTcpTable(null, ref bufferlength, true, (int)AddressFamily.Internet6, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);

                    if (ret != 0 && ret != 122) // 122 insufficient buffer size
                    throw new Exception("bad ret on check " + ret);
                    v6buffer = new byte[bufferlength];

                    ret = GetExtendedTcpTable(v6buffer, ref bufferlength, true, (int)AddressFamily.Internet6, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                    if (ret != 0)
                    throw new Exception("bad ret " + ret);
                break;
            case Protocol.UDP:
                ret = GetExtendedUdpTable(null, out bufferlength, true, (int)AddressFamily.Internet, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if(ret != 0 && ret != 122) // 122 insufficient buffer size
                    throw new Exception("bad ret on check " + ret);
                v4buffer = new byte[bufferlength];

                ret = GetExtendedUdpTable(v4buffer, out bufferlength, true, (int)AddressFamily.Internet, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if(ret != 0)
                    throw new Exception("bad ret " + ret);

                ret = GetExtendedUdpTable(null, out bufferlength, true, (int)AddressFamily.Internet6, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if(ret != 0 && ret != 122) // 122 insufficient buffer size
                    throw new Exception("bad ret on check " + ret);
                v6buffer = new byte[bufferlength];

                ret = GetExtendedUdpTable(v6buffer, out bufferlength, true, (int)AddressFamily.Internet6, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if(ret != 0)
                    throw new Exception("bad ret " + ret);
                break;
            default:
                throw new InvalidOperationException();
            }

            bufferlength = 0;
            int length = BitConverter.ToInt32(v4buffer, bufferlength);
            bufferlength += 4;

            for(int i = 0; i < length; i++) {
                IPAddress local;
                int localport;
                int pid;
                switch(protocol) {
                case Protocol.TCP:
                    ConnectionState connectionstate = (ConnectionState)BitConverter.ToInt32(v4buffer, bufferlength);
                    bufferlength += 4;
                    local = new IPAddress(BitConverter.ToUInt32(v4buffer, bufferlength));
                    bufferlength += 4;
                    localport = GetPort(v4buffer, bufferlength);
                    bufferlength += 4;
                    IPAddress remote = new IPAddress(BitConverter.ToUInt32(v4buffer, bufferlength));
                    bufferlength += 4;
                    int remoteport = GetPort(v4buffer, bufferlength);
                    bufferlength += 4;
                    pid = Convert.ToInt32(BitConverter.ToUInt32(v4buffer, bufferlength));
                    bufferlength += 4;
                    yield return new ConnectionInformation(protocol, local, localport, remote, remoteport, pid, connectionstate);
                    break;
                case Protocol.UDP:
                    local = new IPAddress(BitConverter.ToUInt32(v4buffer, bufferlength));
                    bufferlength += 4;
                    localport = BitConverter.ToInt32(v4buffer, bufferlength);
                    bufferlength += 4;
                    pid = Convert.ToInt32(BitConverter.ToUInt32(v4buffer, bufferlength));
                    bufferlength += 4;
                    yield return new ConnectionInformation(protocol, local, localport, IPAddress.Any, 0, pid, ConnectionState.Established);
                    break;
                }
            }

            // get v6 entries
            byte[] ipbuffer = new byte[16];
            bufferlength = 0;
            length = BitConverter.ToInt32(v6buffer, bufferlength);
            bufferlength += 4;

            for(int i = 0; i < length; i++) {
                switch(protocol) {
                case Protocol.TCP:
                    Array.Copy(v6buffer, bufferlength, ipbuffer, 0, 16);
                    bufferlength += 16;
                    IPAddress local = new IPAddress(ipbuffer, BitConverter.ToInt32(v6buffer, bufferlength));
                    bufferlength += 4;
                    int localport = GetPort(v6buffer, bufferlength);
                    bufferlength += 4;

                    Array.Copy(v6buffer, bufferlength, ipbuffer, 0, 16);
                    bufferlength += 16;
                    IPAddress remote = new IPAddress(ipbuffer, BitConverter.ToUInt32(v6buffer, bufferlength));
                    bufferlength += 4;
                    int remoteport = GetPort(v6buffer, bufferlength);
                    bufferlength += 4;

                    ConnectionState connectionstate = (ConnectionState)BitConverter.ToInt32(v6buffer, bufferlength);
                    bufferlength += 4;
                    int pid = Convert.ToInt32(BitConverter.ToUInt32(v6buffer, bufferlength));
                    bufferlength += 4;
                    yield return new ConnectionInformation(protocol, local, localport, remote, remoteport, pid, connectionstate);
                    break;
                case Protocol.UDP:
                    Array.Copy(v6buffer, bufferlength, ipbuffer, 0, 16);
                    bufferlength += 16;
                    local = new IPAddress(v6buffer, BitConverter.ToUInt32(v6buffer, bufferlength));
                    bufferlength += 4;
                    localport = BitConverter.ToInt32(v6buffer, bufferlength);
                    bufferlength += 4;
                    pid = Convert.ToInt32(BitConverter.ToUInt32(v6buffer, bufferlength));
                    bufferlength += 4;
                    yield return new ConnectionInformation(protocol, local, localport, IPAddress.Any, 0, pid, ConnectionState.Established);
                    break;
                }
            }
        }

        /// <summary>
        /// get all open connections
        /// </summary>
        /// <param name="protocol">protocol to include</param>
        /// <returns>connections open on system</returns>
        public static IEnumerable<ConnectionInformation> GetOpenConnections(Protocol protocol = Protocol.All) {
            if((protocol & Protocol.TCP) == Protocol.TCP)
                foreach(ConnectionInformation connection in GetAllConnections(Protocol.TCP))
                    yield return connection;

            if((protocol & Protocol.UDP) == Protocol.UDP)
                foreach(ConnectionInformation connection in GetAllConnections(Protocol.UDP))
                    yield return connection;
        }
    }
}