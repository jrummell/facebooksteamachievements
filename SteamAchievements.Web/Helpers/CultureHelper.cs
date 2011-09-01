#region License

// Copyright 2010 John Rummell
// 
// This file is part of SteamAchievements.
// 
//     SteamAchievements is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     SteamAchievements is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Web.Helpers
{
    public static class CultureHelper
    {
        // Include ONLY cultures you are implementing as views
        private static readonly Dictionary<string, bool> _cultures =
            new Dictionary<string, bool>
                {
                    // first culture is the DEFAULT
                    {"en-US", true},
                    {"fr-FR", true}
                };

        /// <summary>
        /// Returns a valid culture name based on "name" parameter. If "name" is not valid, it returns the default culture "en-US"
        /// </summary>
        /// <param name="name">Culture's name (e.g. en-US)</param>
        public static string GetValidCulture(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GetDefaultCulture(); // return Default culture
            }

            if (_cultures.ContainsKey(name))
            {
                return name;
            }

            // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
            // the function will return closes match that is "en-US" because at least the language is the same (ie English)            
            foreach (string c in _cultures.Keys)
            {
                if (c.StartsWith(name.Substring(0, 2)))
                {
                    return c;
                }
            }

            return GetDefaultCulture();
        }

        /// <summary>
        /// Returns default culture name which is the first name decalared (e.g. en-US)
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultCulture()
        {
            return _cultures.Keys.First();
        }
    }
}