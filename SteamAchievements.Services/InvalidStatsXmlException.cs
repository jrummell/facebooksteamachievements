using System;
using System.Xml;

namespace SteamAchievements.Services
{
    public class InvalidStatsXmlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStatsXmlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidStatsXmlException(string message, XmlException innerException)
            : base(message, innerException)
        {
        }
    }
}