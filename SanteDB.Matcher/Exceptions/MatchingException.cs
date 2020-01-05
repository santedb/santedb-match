using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Exceptions
{
    /// <summary>
    /// Represents a matching exception
    /// </summary>
    public class MatchingException : Exception
    {

        /// <summary>
        /// Default contructor
        /// </summary>
        public MatchingException()
        {

        }

        /// <summary>
        /// Creates a new matching exception with specified <paramref name="message"/>
        /// </summary>
        public MatchingException(String message)
        {

        }

        /// <summary>
        /// Creates a new matching exception with specified <paramref name="message"/> caused by <paramref name="innerException"/> 
        /// </summary>
        public MatchingException(String message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
