using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace NightlyCode.Net.Websockets {

    /// <summary>
    /// class implementing the websocket protocol (RFC 6455)
    /// </summary>
    public class WebSocket {
        TcpClient client;

        public event Action Disconnected;

        /// <summary>
        /// triggered when a message was received
        /// </summary>
        public event Action<byte[]> Message;

        public void Disconnect() {
            try {
                client.Close();
            }
            catch(Exception) {
            }
            Disconnected?.Invoke();
        }

        void Read() {
            while(client.Connected) {
                using(BinaryReader reader = new BinaryReader(client.GetStream())) {
                    try {
                        ReadMessage(reader);
                    }
                    catch(Exception) {
                        Disconnect();
                    }
                }
            }
        }

        void ReadMessage(BinaryReader reader) {
            bool isfinal=false;

            List<byte> data=new List<byte>();
            while(!isfinal) {
                byte opcodedata = reader.ReadByte();
                isfinal = (opcodedata & 128) != 0;

                long length = reader.ReadByte();
                bool hasmask = (length & 128) != 0;
                if(length == 126)
                    length = reader.ReadInt16();
                else if(length == 127)
                    length = reader.ReadInt64();

                byte[] maskkey=null;
                if(hasmask)
                    maskkey = reader.ReadBytes(4);

                byte[] framedata = reader.ReadBytes((int)length);
                if(hasmask) {
                    for(int i=0;i<framedata.Length;++i)
                        data.Add((byte)(framedata[i]^maskkey[i&3]));
                }
                else data.AddRange(framedata);
            }

            Message?.Invoke(data.ToArray());
        }
    }
}