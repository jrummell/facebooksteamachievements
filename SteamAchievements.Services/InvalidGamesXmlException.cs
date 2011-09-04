using System;
using System.Xml;

namespace SteamAchievements.Services
{
    public class InvalidGamesXmlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGamesXmlException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidGamesXmlException(string message, XmlException innerException)
            : base(message, innerException)
        {
            
        }
    }
}