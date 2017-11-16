using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NightlyCode.Net.Http.Requests {

    /// <summary>
    /// request to a http-server
    /// </summary>
    public class HttpRequest {
        readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

        /// <summary>
        /// creates a new <see cref="HttpRequest"/>
        /// </summary>
        /// <param name="method">method used for request</param>
        /// <param name="resource">requested resource</param>
        public HttpRequest(string method, string resource) {
            Method = method;
            DetermineResource(resource);
        }

        void DetermineResource(string resource) {
            int index = resource.IndexOf('?');

            if (index > -1)
            {
                foreach (string parameter in resource.Substring(index + 1).Split('&'))
                {
                    int valueindex = parameter.IndexOf('=');
                    if (valueindex > -1)
                        parameters[parameter.Substring(0, valueindex)] = parameter.Substring(valueindex + 1);
                }
                Resource = resource.Substring(0, index);
            }
            else Resource = resource;
        }

        /// <summary>
        /// parameter collection
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters => parameters;

        /// <summary>
        /// adds a parameter to the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParameter(string name, string value)
        {
            parameters[name] = value;
        }

        /// <summary>
        /// get value to a parameter name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetParameter(string name)
        {
            return parameters[name];
        }


        /// <summary>
        /// access to headers
        /// </summary>
        /// <param name="key">key of header</param>
        /// <returns>header value</returns>
        public string this[string key]
        {
            get { return headers[key]; }
            set { headers[key] = value; }
        }

        /// <summary>
        /// get header value from request
        /// </summary>
        /// <param name="key">key of header</param>
        /// <returns>header value or null if header is not contained in request</returns>
        public string GetHeader(string key) {
            string header;
            headers.TryGetValue(key, out header);
            return header;
        }

        /// <summary>
        /// determines whether the request contains a header
        /// </summary>
        /// <param name="key">key of header</param>
        /// <returns>true if request contains header with the specified key, false otherwise</returns>
        public bool HasHeader(string key) {
            return headers.ContainsKey(key);
        }

        /// <summary>
        /// used method
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// http-version
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// requested resource
        /// </summary>
        public string Resource { get; private set; }

        /// <summary>
        /// host which should create a response to the request
        /// </summary>
        public string Host => GetHeader("Host");

        /// <summary>
        /// string representation of parameters
        /// </summary>
        public string QueryString { get { return string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")); } }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Method).Append(" ");
            if (!string.IsNullOrEmpty(Host))
                sb.Append(Host);
            sb.Append(Resource);
            if (parameters.Count > 0)
                sb.Append("?").Append(QueryString);
            return sb.ToString();
        }

    }
}