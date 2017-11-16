namespace NightlyCode.Net.TCP {

    /// <summary>
    /// scope of connections to get in netstat
    /// </summary>
    public enum ConnectionScope {

        /// <summary>
        /// all connections belonging to the caller process
        /// </summary>
        Process,

        /// <summary>
        /// all connections belonging to the caller module
        /// </summary>
        Module, 

        /// <summary>
        /// all connections known to system
        /// </summary>
        All
    }
}