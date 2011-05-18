using System;

namespace SteamAchievements.Web.Models
{
    public class InvalidSignedRequestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignedRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidSignedRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}