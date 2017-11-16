using System;

namespace NightlyCode.Net.TCP.Native {

    /// <summary>
    /// protocol for network service
    /// </summary>
    [Flags]
    public enum Protocol {

        /// <summary>
        /// tcp connections
        /// </summary>
        TCP=1,

        /// <summary>
        /// udp connections
        /// </summary>
        UDP=2,

        /// <summary>
        /// used as mask to get all connections
        /// </summary>
        All=3
    }
}