using HtmlAgilityPack;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// adapter for browser which processes http data
    /// </summary>
    public class HttpAdapter {
        readonly IBrowser browser;

        /// <summary>
        /// creates a new <see cref="HttpAdapter"/>
        /// </summary>
        /// <param name="browser"></param>
        public HttpAdapter(IBrowser browser) {
            this.browser = browser;
        }

        /// <summary>
        /// loads a document using the browser
        /// </summary>
        /// <param name="address"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public HtmlDocument LoadDocument(string address, WebRequestParameters parameters=null) {
            BrowserResponse response = browser.Load(address, parameters);
            using(response.Response) {
                HtmlDocument document = new HtmlDocument();
                document.Load(response.Response, response.Encoding);
                return document;
            }
        }
    }
}