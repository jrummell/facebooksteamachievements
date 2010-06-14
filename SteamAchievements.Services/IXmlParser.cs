using System;
using System.Collections.Generic;

namespace SteamAchievements.Services
{
    /// <summary>
    /// Parses <typeparamref name="T"/> xml.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IXmlParser<T>
    {
        /// <summary>
        /// Returns a collection of <typeparamref name="T"/>s from the given xml.
        /// </summary>
        IEnumerable<T> Parse(string xml);
    }
}
