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

using System;

namespace SteamAchievements.Services
{
    public class User : IEquatable<User>
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto update]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [publish description].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [publish description]; otherwise, <c>false</c>.
        /// </value>
        public bool PublishDescription { get; set; }

        /// <summary>
        /// Gets or sets the facebook user id.
        /// </summary>
        /// <value>
        /// The facebook user id.
        /// </value>
        public long FacebookUserId { get; set; }

        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>
        /// The steam user id.
        /// </value>
        public string SteamUserId { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        #region IEquatable<User> Members

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.FacebookUserId == FacebookUserId && Equals(other.SteamUserId, SteamUserId);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (User))
            {
                return false;
            }
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FacebookUserId.GetHashCode()*397) ^ (SteamUserId != null ? SteamUserId.GetHashCode() : 0);
            }
        }
    }
}