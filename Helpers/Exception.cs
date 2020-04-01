using System;

namespace Helpers
{
    public class KnownException : Exception
    {
        public KnownException() : base() { }
        public KnownException(string message) : base(message) { }
        public KnownException(string message, Exception e) : base(message, e) { }
    }
}
