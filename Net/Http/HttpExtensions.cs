using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NightlyCode.Net.Browser;

namespace NightlyCode.Net.Http {

    /// <summary>
    /// helpers for http data
    /// </summary>
    public static class HttpExtensions {

        static string MakeFirstLetterUppercase(string value)
        {
            return value.First().ToString().ToUpper() + string.Join("", value.Skip(1));
        }

        /// <summary>
        /// convert the specified address to a string readable by humans
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string GetReadableAddress(string address) {
            if(string.IsNullOrEmpty(address))
                return address;
            
            Match addressmatch = Regex.Match(address, @"((?<protocol>[a-zA-Z]+):\/\/)?(?<host>[^\/:]+)(:(?<port>[0-9]+))?\/(?<relative>[^\?]*)(\?(?<querystring>.*))?");
            if (addressmatch.Success)
            {
                string[] split = addressmatch.Groups["host"].Value.Split('.');

                string host;
                if (split.Length == 4 && split.All(s => s.All(char.IsDigit)))
                {
                    // this is an ipv4 address
                    host = addressmatch.Groups["host"].Value;
                }
                else if (split.Length > 1)
                {
                    // an external dns name
                    host = MakeFirstLetterUppercase(split[split.Length - 2]);
                }
                else
                {
                    // could be a local dns name (localhost)
                    host = MakeFirstLetterUppercase(addressmatch.Groups["host"].Value);
                }

                if (addressmatch.Groups["port"].Success)
                    return host + ":" + addressmatch.Groups["port"].Value;
                return host;
            }

            // if there is no success in determining the host just return the original value
            return address;

        }

        /// <summary>
        /// get relative path of address to a bound prefix
        /// </summary>
        /// <param name="address">address to check</param>
        /// <param name="prefixes">prefix server is listening on</param>
        public static string GetRelativePath(string address, IEnumerable<string> prefixes) {
            Match addressmatch = Regex.Match(address, @"(?<protocol>[a-zA-Z]+):\/\/(?<host>[^\/:]+)(:(?<port>[0-9]+))?\/(?<relative>[^\?]*)(\?(?<querystring>.*))?");

            if(!addressmatch.Success)
                throw new Exception("Unable to analyse address");
            string relativeaddress = addressmatch.Groups["relative"].Value;

            foreach (string prefix in prefixes) {
                Match prefixmatch = Regex.Match(prefix, @"(?<protocol>([a-zA-Z]+)|\*|\+):\/\/(?<host>[^\/:]+)(:(?<port>[0-9]+))?\/(?<relative>.*)");
                if (!prefixmatch.Success)
                    throw new Exception("Unable to analyse prefix");


                switch (prefixmatch.Groups["host"].Value) {
                    case "*":
                    case "+":
                        string relativeprefix = prefixmatch.Groups["relative"].Value;
                        if (relativeaddress.StartsWith(relativeprefix))
                            return "/" + relativeaddress.Substring(relativeprefix.Length);
                        break;
                    default:
                        if(addressmatch.Groups["host"].Value == prefixmatch.Groups["host"].Value)
                            return "/" + relativeaddress.Substring(prefixmatch.Groups["relative"].Value.Length);
                        break;
                }
            }

            throw new Exception("address does not match any of the prefixes");
        }

        /// <summary>
        /// get relative uri for a uri with prefix information
        /// </summary>
        /// <param name="uri">absolute uri of which to get relative uri</param>
        /// <param name="prefixes">prefixes to use to generate relative uri</param>
        /// <returns>relative uri with query information</returns>
        public static Uri GetRelativeUri(Uri uri, IEnumerable<string> prefixes) {
            return new Uri(GetRelativePath(uri.AbsoluteUri, prefixes) + uri.Query, UriKind.Relative);
        }

        /// <summary>
        /// processes a template
        /// </summary>
        /// <param name="template">template to process</param>
        /// <param name="parameters">parameter collection containing parameters to fill in</param>
        /// <returns></returns>
        public static string ProcessTemplate(string template, params Parameter[] parameters) {
            int stage = 0;
            StringBuilder result = new StringBuilder();

            StringBuilder variable = new StringBuilder();
            foreach(char character in template) {
                switch(stage) {
                    case 0:
                        if(character == '$')
                            ++stage;
                        else
                            result.Append(character);
                        break;
                    case 1:
                        if(character == '$') {
                            variable.Length = 0;
                            result.Append("$");
                            result.Append(character);
                            stage = 0;

                        }
                        else {
                            variable.Append(character);
                            ++stage;
                        }
                        break;
                    case 2:
                        if(character == '$') {
                            stage = 0;

                            if(variable.Length > 0) {
                                string varname = variable.ToString();
                                Parameter parameter = parameters.FirstOrDefault(p => p.Name == varname);
                                if(parameter!=null) {
                                    result.Append(parameter.Value);
                                    variable.Length = 0;
                                }
                                else
                                    throw new InvalidOperationException("Variable not found");
                            }
                        }
                        else
                            variable.Append(character);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// creates a parameter enumeration out of linear string arguments
        /// </summary>
        /// <remarks>
        /// odd values are keys and even values are "values". if you specify an odd count of arguments this method will crash.
        /// </remarks>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IEnumerable<Parameter> CreateParameters(params string[] parameters)
        {
            for (int i = 0; i < parameters.Length; i += 2)
                yield return new Parameter(parameters[i], parameters[i + 1]);
        }

    }
}