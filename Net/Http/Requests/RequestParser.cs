using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NightlyCode.Net.Http.Requests {

    /// <summary>
    /// parses requests from incoming data
    /// </summary>
    public class RequestParser {

        /// <summary>
        /// parses a http request from a string
        /// </summary>
        /// <param name="datareader">stream used to read data</param>
        /// <returns>parsed request</returns>
        public HttpRequest Parse(Stream datareader) {
            StreamReader reader = new StreamReader(datareader);
            reader.BaseStream.ReadTimeout = 100;

            string line;
            try {
                line = reader.ReadLine();
            }
            catch(Exception) {
                throw new MissingHeaderException("No header could be read from stream");
            }

            if(string.IsNullOrEmpty(line))
                throw new MissingHeaderException("No header could be read from stream");

            Match match = Regex.Match(line, @"(?<method>[a-zA-Z]+) (?<resource>[^ ]+) HTTP/(?<httpversion>([0-9]|\.)+)");
            if(!match.Success)
                throw new Exception("Unable to parse http request method");

            HttpRequest request;
            switch(match.Groups["method"].Value) {
            case "POST":
                request = new HttpPostRequest(datareader, Uri.UnescapeDataString(match.Groups["resource"].Value));
                break;
            default:
                request = new HttpRequest(match.Groups["method"].Value, Uri.UnescapeDataString(match.Groups["resource"].Value));
                break;
            }

            request.Version = Version.Parse(match.Groups["httpversion"].Value);

            ReadHeaders(request, reader);
            return request;
        }

        void ReadHeaders(HttpRequest request, StreamReader reader) {
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine())) {
                int indexof = line.IndexOf(':');
                if(indexof==-1)
                    throw new Exception("Invalid header");

                request[line.Substring(0, indexof)] = line.Substring(indexof + 2);
            }
        }
    }
}