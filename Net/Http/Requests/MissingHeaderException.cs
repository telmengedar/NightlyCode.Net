using System;
using System.Runtime.Serialization;

namespace NightlyCode.Net.Http.Requests {

    /// <summary>
    /// thrown when parser was expecting a header but nothing was read
    /// </summary>
    public class MissingHeaderException : Exception {
        public MissingHeaderException(string message)
            : base(message) {}

        public MissingHeaderException(string message, Exception innerException)
            : base(message, innerException) {}

        protected MissingHeaderException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}