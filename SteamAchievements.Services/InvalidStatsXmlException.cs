using System;
using System.Xml;

namespace SteamAchievements.Services
{
    public class InvalidStatsXmlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStatsXmlException"/> class.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="url">The URL.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidStatsXmlException(string steamUserId, Uri url, XmlException innerException)
            : base(String.Format("Invalid stats xml for {0} ({1}).", steamUserId, url), innerException)
        {
        }
    }
}