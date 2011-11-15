using System;
using System.Xml;

namespace SteamAchievements.Services
{
    public class InvalidGamesXmlException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGamesXmlException"/> class.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="gamesUrl">The games URL.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidGamesXmlException(string steamUserId, Uri gamesUrl, XmlException innerException)
            : base(String.Format("Invalid games xml {0} ({1}).", steamUserId, gamesUrl), innerException)
        {
        }
    }
}