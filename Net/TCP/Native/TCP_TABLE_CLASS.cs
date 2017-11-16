﻿namespace NightlyCode.Net.TCP.Native {

    /// <summary>
    /// table classes for usage with <see cref="NetStat.GetAllConnections"/>
    /// </summary>
    public enum TCP_TABLE_CLASS {

        /// <summary>
        /// A MIB_TCPTABLE table that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_BASIC_LISTENER,

        /// <summary>
        /// A MIB_TCPTABLE table that contains all connected TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_BASIC_CONNECTIONS,

        /// <summary>
        /// A MIB_TCPTABLE table that contains all TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_BASIC_ALL,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_PID_LISTENER,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that structure that contains all connected TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_PID_CONNECTIONS,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID structure that contains all TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_PID_ALL,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_MODULE_LISTENER,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all connected TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,

        /// <summary>
        /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all TCP endpoints on the local computer is returned to the caller.
        /// </summary>
        TCP_TABLE_OWNER_MODULE_ALL
    }
}