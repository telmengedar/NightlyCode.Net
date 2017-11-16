using System;

namespace NightlyCode.Net.Browser {
    /// <summary>
    /// interface for a browser
    /// </summary>
    public interface IBrowser {

        /// <summary>
        /// triggered on redirect
        /// </summary>
        event Func<string, bool> Redirect;

        /// <summary>
        /// loads data of an uri
        /// </summary>
        /// <param name="address">address from which to load document</param>
        /// <param name="parameters">request parameters</param>
        /// <returns></returns>
        BrowserResponse Load(string address, WebRequestParameters parameters = null);
    }
}