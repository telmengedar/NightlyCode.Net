using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace NightlyCode.Net.Browser {

    /// <summary>
    /// browser used for crawling processes
    /// </summary>
    public class SlimBrowser : IBrowser {
        readonly CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// triggered on redirect
        /// </summary>
        public event Func<string, bool> Redirect;

        bool OnRedirect(string target) {
            return Redirect?.Invoke(target) ?? true;
        }

        void PrepareParameters(HttpWebRequest request, WebRequestParameters parameters) {
            if(parameters.Credentials != null)
                request.Credentials = parameters.Credentials;

            if(parameters.Referer != null)
                request.Referer = parameters.Referer;

            if(parameters.FormData == null) return;

            request.ContentType = "application/x-www-form-urlencoded";
            using(StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                sw.Write(CreateUriParameters(parameters.FormData));
        }

        string CreateUriParameters(IEnumerable<Parameter> parameters) {
            return string.Join("&", parameters.Select(p => p.Name + "=" + (string.IsNullOrEmpty(p.Value) ? "" : Uri.EscapeUriString(p.Value))));
        }

        /// <summary>
        /// loads data of an uri
        /// </summary>
        /// <param name="address">address from which to load document</param>
        /// <param name="parameters">request parameters</param>
        /// <returns></returns>
        public BrowserResponse Load(string address, WebRequestParameters parameters=null)
        {
            if (parameters?.Query != null)
                address += "?" + CreateUriParameters(parameters.Query);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(address));
            request.AllowAutoRedirect = false;
            request.PreAuthenticate = true;
            request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
            request.CookieContainer = cookies;

            request.Method = parameters?.FormData != null ? "POST" : "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //request.Timeout = 60000;
            if (parameters != null)
                PrepareParameters(request, parameters);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.Found:
                    string location = response.Headers.Get("Location");

                    if (OnRedirect(location))
                    {
                        // close response or redirect could freeze
                        response.Close();
                        Uri target = Uri.IsWellFormedUriString(location, UriKind.Absolute) ? new Uri(location) : new Uri(response.ResponseUri, location);
                        return Load(target.AbsoluteUri);
                    }
                    break;
                default:
                    throw new WebException("Unable to load document");
            }

            Encoding encoding = DetectEncoding(response);
            return new BrowserResponse(response.GetResponseStream(), encoding);
        }

        Encoding DetectEncoding(HttpWebResponse response) {

            try {
                if(!string.IsNullOrEmpty(response.ContentEncoding))
                    return Encoding.GetEncoding(response.ContentEncoding);
            }
            catch(Exception) {
                return Encoding.UTF8;
            }

            try {
                if(!string.IsNullOrEmpty(response.ContentType)) {
                    Match regex = Regex.Match(response.ContentType, "charset=(?<charset>.+)$");
                    if(regex.Success)
                        return Encoding.GetEncoding(regex.Groups["charset"].Value);
                }
            }
            catch(Exception) {
                return Encoding.UTF8;
            }

            return Encoding.UTF8;
        }
    }
}