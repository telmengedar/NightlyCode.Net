namespace NightlyCode.Net.Websockets {

    /// <summary>
    /// opcode describing meaning of opcode
    /// </summary>
    public enum WebSocketOpCode {

        /// <summary>
        /// continuation frame
        /// </summary>
        Continuation=0,

        /// <summary>
        /// text frame
        /// </summary>
        Text=1,

        /// <summary>
        /// binary frame
        /// </summary>
        Binary=2,

        /// <summary>
        /// connection close
        /// </summary>
        Close=8,

        /// <summary>
        /// ping frame
        /// </summary>
        Ping=9,

        /// <summary>
        /// pong frame
        /// </summary>
        Pong=10
    }
}