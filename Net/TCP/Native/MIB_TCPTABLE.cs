namespace NightlyCode.Net.TCP.Native {

    /// <summary>
    /// The MIB_TCPTABLE structure contains a table of TCP connections for IPv4 on the local computer.
    /// </summary>
    public class MIB_TCPTABLE {

        /// <summary>
        /// number of entries in the table
        /// </summary>
        public int NumberOfEntries { get; set; }

        /// <summary>
        /// rows containing connection information
        /// </summary>
        public MIB_TCPROW[] Rows { get; set; }
    }
}