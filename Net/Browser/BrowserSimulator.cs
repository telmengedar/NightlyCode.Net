using System;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// simulates browser behavior
    /// </summary>
    /// <remarks>can be used for tests</remarks>
    public class BrowserSimulator : IBrowser {
        readonly Func<string, WebRequestParameters, SimulationResponse> datafunc;

        /// <summary>
        /// creates a new <see cref="BrowserSimulator"/>
        /// </summary>
        /// <param name="datafunc">function to use for requests</param>
        public BrowserSimulator(Func<string, WebRequestParameters, SimulationResponse> datafunc) {
            this.datafunc = datafunc;
        }

        /// <summary>
        /// triggered when browser is redirected
        /// </summary>
        public event Func<string, bool> Redirect;

        /// <summary>
        /// simulates a browser load
        /// </summary>
        /// <param name="address">address to load</param>
        /// <param name="parameters">parameters for request</param>
        /// <returns>response of browser</returns>
        public BrowserResponse Load(string address, WebRequestParameters parameters = null) {
            SimulationResponse response = datafunc(address, parameters);
            if(!string.IsNullOrEmpty(response.RedirectionTarget)) {
                if(Redirect?.Invoke(response.RedirectionTarget)??false)
                    return Load(response.RedirectionTarget);
            }

            return new BrowserResponse(response.ResponseData, response.ResponseEncoding);
        }
    }
}