using System;
using System.Net;
using System.Text;
using NightlyCode.Net.Browser;

namespace NightlyCode.Net.Server
{

    /// <summary>
    /// base class for service clients
    /// </summary>
    public abstract class ServiceClient<TRequest, TResponse>
    {
        readonly string host;
        readonly IWebProxy proxy;
        readonly Action<object, string, Exception> errorlogger;

        /// <summary>
        /// creates a basic service client
        /// </summary>
        /// <param name="host">host to which to connect</param>
        /// <param name="proxy">proxy to use</param>
        /// <param name="errorlogger">action used for logging</param>
        protected ServiceClient(string host, IWebProxy proxy=null, Action<object, string, Exception> errorlogger=null) {
            this.host = host;
            this.proxy = proxy;
            this.errorlogger = errorlogger;
        }

        /// <summary>
        /// host to which to send requests
        /// </summary>
        public string Host => host;

        string GetUrl(string path)
        {
            return host + path;
        }

        /// <summary>
        /// converts request to data to be sent to server
        /// </summary>
        /// <param name="request">request to be converted</param>
        /// <returns>web data</returns>
        protected abstract string GetRequestData(TRequest request);

        /// <summary>
        /// creates a response object from response data
        /// </summary>
        /// <param name="data">data to be converted to a response</param>
        /// <typeparam name="T">type of response to be created</typeparam>
        /// <returns>response object</returns>
        protected abstract T CreateResponse<T>(string data)
            where T : TResponse, new();

        /// <summary>
        /// creates a request error from an exception
        /// </summary>
        /// <param name="e">error occured</param>
        /// <typeparam name="T">type of response to be created</typeparam>
        /// <returns>error response object</returns>
        protected abstract T CreateRequestError<T>(Exception e)
            where T : TResponse, new();

        /// <summary>
        /// sends a request to the host
        /// </summary>
        /// <typeparam name="T">type of response to return</typeparam>
        /// <param name="path">request path on host</param>
        /// <param name="request">request to send as post data (optional)</param>
        /// <param name="credentials">credentials to use</param>
        /// <param name="parameters">parameters to append to query string</param>
        /// <returns>the response object returned by the server or an error object if the request fails somehow</returns>
        public T Request<T>(string path, TRequest request, ICredentials credentials = null, params Parameter[] parameters)
            where T : TResponse, new() {

            try {
                using(WebClient wc = new WebClient()) {
                    if(parameters.Length > 0)
                        foreach(Parameter parameter in parameters)
                            wc.QueryString.Add(parameter.Name, parameter.Value);

                    wc.Proxy = proxy;
                    wc.Encoding = Encoding.UTF8;
                    wc.Credentials = credentials;
                    string responsedata;

                    if(request != null) {
                        wc.Headers.Add("Content-Type", MimeTypes.GetMimeType(".json"));
                        string requestdata = GetRequestData(request);

                        // this would result in an exception.
                        //wc.Headers.Add("Content-Length", requestdata.Length.ToString());

                        responsedata = wc.UploadString(GetUrl(path), requestdata);
                    }
                    else responsedata = wc.DownloadString(GetUrl(path));

                    if(string.IsNullOrEmpty(responsedata) || responsedata == "null")
                        return CreateResponse<T>(null);

                    try {
                        return CreateResponse<T>(responsedata);
                    }
                    catch(Exception e) {
                        errorlogger?.Invoke(this, "Error creating response", e);
                        return CreateRequestError<T>(e);
                    }

                }
            }
            catch(Exception e) {
                errorlogger?.Invoke(this, "Error sending request", e);
                return CreateRequestError<T>(e);
            }
        }
    }
}