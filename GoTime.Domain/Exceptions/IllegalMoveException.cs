
using System;

namespace GoTime.Domain.Exceptions
{
    internal class IllegalMoveException : Exception
    {
        public IllegalMoveException(string message) : base(message)
        {

        }
    }
}
