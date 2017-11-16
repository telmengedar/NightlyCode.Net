using System;
using System.IO;
using System.Text;

namespace NightlyCode.Net {

    /// <summary>
    /// reads multipart attachements
    /// </summary>
    /// <remarks>
    /// this looks like fiddly work and it is. The stream is a network stream most of the time so
    /// no seeking is possible. since all characters could be part of boundary or not and all delimiters
    /// are multiple characters wide reading has to be done byte by byte
    /// </remarks>
    public class MultipartReader {
        readonly Stream stream;
        readonly string boundary;
        bool hasdata = true;

        /// <summary>
        /// creates a new multipart reader
        /// </summary>
        /// <param name="contenttype">content type where boundary is set</param>
        /// <param name="stream">input stream containing multipart data</param>
        public MultipartReader(string contenttype, Stream stream) {
            this.stream = new BufferedStream(stream);
            string[] split = contenttype.Split(';');
            foreach(string argument in split) {
                if(!argument.Trim().StartsWith("boundary"))
                    continue;

                boundary = "--" + argument.Split('=')[1];
            }

            if(boundary == null)
                throw new InvalidOperationException("No boundary specifier found");

            // skip first boundary and trailing \r\n
            SkipBoundary();
        }

        /// <summary>
        /// progresses stream for the length of a boundary field
        /// </summary>
        /// <returns>false if end of stream was found, true otherwise</returns>
        public bool SkipBoundary() {
            for(int i = 0; i < boundary.Length + 2; ++i) {

                if(stream.ReadByte() == -1) {
                    hasdata = false;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// reads a header line from multipart data
        /// </summary>
        /// <returns></returns>
        public string ReadHeader() {
            StringBuilder sb = new StringBuilder();

            int read;
            bool flag = false;
            do {
                read = stream.ReadByte();
                if(read != -1) {
                    char character = (char)read;
                    switch(character) {
                        case '\r':
                            flag = true;
                            sb.Append(character);
                            break;
                        case '\n':
                            if(flag)
                                // fastest way to break loop without introducing more variables
                                read = -1;
                            break;
                        default:
                            flag = false;
                            sb.Append(character);
                            break;
                    }
                }
                else {
                    hasdata = false;
                    return null;
                }
            } while(read > -1);

            // trim last \r
            sb.Length = sb.Length - 1;
            return sb.ToString();
        }

        /// <summary>
        /// processes input stream until data section begins
        /// </summary>
        public void SkipReadingToData() {
            // ReSharper disable CSharpWarnings::CS0642
            while(!string.IsNullOrEmpty(ReadHeader()))
                // skip all other headers
                // this is supposed to be an empty control block, thus the suppression
                // of the warning
                ;
            // ReSharper restore CSharpWarnings::CS0642
        }

        /// <summary>
        /// reads data blocks
        /// </summary>
        /// <param name="processor">action to execute for every data block. first argument is data buffer, second argument is length of actual data in buffer</param>
        public void ReadData(Action<byte[], int> processor) {
            byte[] buffer = new byte[4096 + boundary.Length];
            int comparator = 0;
            int offset = 0;
            while(comparator < boundary.Length || offset == 0) {
                if(comparator == boundary.Length)
                    comparator = 0;

                int read = stream.ReadByte();
                if(read == -1) {
                    hasdata = false;
                    break;
                }

                if(read == boundary[comparator])
                    ++comparator;
                else {
                    if(comparator > 0) {
                        for(int i = 0; i < comparator; ++i)
                            buffer[offset++] = (byte)boundary[i];
                    }
                    buffer[offset++] = (byte)read;
                    comparator = 0;
                }

                if(offset >= 4096 && comparator == 0) {
                    processor(buffer, offset);
                    offset = 0;
                }
            }

            if(offset > 0)
                processor(buffer, Math.Max(offset - 2, 0));
        }

        /// <summary>
        /// determines whether there is more data to read
        /// </summary>
        /// <remarks>
        /// this is just a guess whether it makes sense try to read more
        /// </remarks>
        public bool HasData => hasdata;
    }
}