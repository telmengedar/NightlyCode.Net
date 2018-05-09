using System;
using System.Threading;
using System.Threading.Tasks;

namespace NightlyCode.Net.Clients {

    /// <summary>
    /// base implementation for a client which reconnects on disconnect
    /// </summary>
    public abstract class NetworkClient {
        int disconnects;

        /// <summary>
        /// determines whether the client is connected
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// determines whether the client reconnects on disconnect
        /// </summary>
        protected bool Reconnect { get; set; } = true;

        /// <summary>
        /// maximum number of disconnects before client doesn't try to reconnect anymore
        /// </summary>
        protected virtual int MaxDisconnects { get; } = 5;

        /// <summary>
        /// called when client disconnected
        /// </summary>
        protected void OnDisconnected() {
            ++disconnects;
            if(disconnects <= MaxDisconnects && Reconnect) {
                double time = Math.Pow(2, Math.Min(disconnects, 6));
                Logger.Warning(this, $"Disconnected. Trying to reconnect in {time.ToString("F0")} seconds.");
                Thread.Sleep(TimeSpan.FromSeconds(time));
                if(Reconnect)
                    Connect();
            }
            else {
                if(Reconnect)
                    Logger.Warning(this, "Disconnected. Maximum number of retries exceeded.");
                else Logger.Warning(this, "Disconnected.");
            }
            
        }

        /// <summary>
        /// connect the client to the server
        /// </summary>
        public void Connect() {
            new Task(InternalConnect).Start();
        }

        void InternalConnect() {
            Logger.Info(this, "Connecting");
            try {
                ConnectClient();
                disconnects = 0;
                IsConnected = true;
            }
            catch(Exception) {
                OnDisconnected();
            }
        }

        /// <summary>
        /// forces the client to disconnect
        /// </summary>
        public void Disconnect() {
            Reconnect = false;
            try {
                DisconnectClient();
            }
            catch(Exception) {
            }
            OnDisconnected();
        }

        /// <summary>
        /// used to connect client to the server
        /// </summary>
        protected abstract void ConnectClient();

        /// <summary>
        /// used to disconnect client from the server
        /// </summary>
        protected abstract void DisconnectClient();
    }
}