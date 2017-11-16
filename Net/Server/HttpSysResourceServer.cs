using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NightlyCode.Net.Server {

    /// <summary>
    /// serves resources for a http listener
    /// </summary>
    public class HttpSysResourceServer {
        readonly Assembly assembly;
        readonly string resourcepath;
        readonly string defaultresource;

        /// <summary>
        /// creates a new page server for http listener
        /// </summary>
        /// <param name="assembly">assembly which provides resources</param>
        /// <param name="rootpath">root path on server</param>
        /// <param name="resourcepath">resource path where http resources are stored</param>
        /// <param name="defaultresource">resource to provide if no resource is specified in request</param>
        public HttpSysResourceServer(Assembly assembly, string rootpath, string resourcepath, string defaultresource = null) {
            this.assembly = assembly;
            RootPath = rootpath;
            this.resourcepath = resourcepath;
            this.defaultresource = defaultresource;
        }

        /// <summary>
        /// root path on server
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// determines whether the resources contain a resource matching the specified url
        /// </summary>
        /// <param name="url">url to find</param>
        /// <returns>true if the resource server can serve a resource, false otherwise</returns>
        public bool ContainsResource(string url) {
            if(!string.IsNullOrEmpty(url)&&!url.StartsWith(RootPath))
                return false;

            url = url.Substring(RootPath.Length);
            if(string.IsNullOrEmpty(url) || url == "/")
                url = defaultresource;
            url = (resourcepath + url).Replace('/', '.');
            return assembly.GetManifestResourceNames().Any(n => n == url);
        }

        /// <summary>
        /// handles a http request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resource">path to resource (most likely determined by context)</param>
        public void HandleRequest(HttpListenerContext context, string resource) {
            if(!resource.StartsWith(RootPath))
                throw new InvalidOperationException("request not for this handler");

            string path = resource.Substring(RootPath.Length);
            if(string.IsNullOrEmpty(path) || path == "/")
                path = defaultresource;

            context.Response.ContentType = MimeTypes.GetMimeType(Path.GetExtension(path));

            path = (resourcepath + path).Replace('/', '.');

            using(Stream resourcestream = assembly.GetManifestResourceStream(path)) {
                if(resourcestream == null) {
                    context.Response.StatusCode = 404;
                    context.Response.StatusDescription = "Resource not found";
                }
                else {
                    context.Response.StatusCode = 200;
                    context.Response.StatusDescription = "OK";
                    resourcestream.CopyTo(context.Response.OutputStream);
                }
            }
        }
    }
}