using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;

namespace Core.Logging
{
    public static class Logger
    {
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        public static void LogMessage(string message = null, params  object[] parameters)
        {
            if (message.IsNullOrBlank())
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(string.Format(message, parameters));
            }
            
        }
    }
}
