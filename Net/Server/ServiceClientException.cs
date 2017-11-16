using System;

namespace NightlyCode.Net.Server {
    public class ServiceClientException : Exception
    {
        public ServiceClientException(string message) : base(message)
        {
        }

        public ServiceClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}