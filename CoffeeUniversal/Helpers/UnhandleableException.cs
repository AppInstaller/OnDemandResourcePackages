using System;

namespace CoffeeUniversal.Helpers
{
    public class UnhandleableException : Exception
    {
        public UnhandleableException(string message) : base(message)
        {
        }
    }
}
