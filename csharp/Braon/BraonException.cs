using System;

namespace Braon
{
    public class BraonException : Exception
    {
        public BraonException()
        {
        }

        public BraonException(string message) : base(message)
        {
        }

        public BraonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}