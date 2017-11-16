using System.Net;

namespace NightlyCode.Net.TCP.Native {
    public struct MIB_TCPROW_OWNERPID {
        public ConnectionState State { get; set; }
        public IPAddress LocalAddress { get; set; }
        public int LocalPort { get; set; }
        public IPAddress RemoteAddress { get; set; }
        public int RemotePort { get; set; }
        public int OwnerPID { get; set; }
    }
}