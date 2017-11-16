using System;

namespace NightlyCode.Net.Server {
    public class ServiceServerException : Exception
    {
        public ServiceServerException(string message) : base(message)
        {
        }

        public ServiceServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}