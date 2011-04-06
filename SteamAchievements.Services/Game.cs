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
    public class Game : IEquatable<Game>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the stats URL.
        /// </summary>
        /// <value>The stats URL.</value>
        public string StatsUrl { get; set; }

        /// <summary>
        /// Gets or sets the store URL.
        /// </summary>
        /// <value>The store URL.</value>
        public string StoreUrl { get; set; }

        /// <summary>
        /// Gets or sets whether the game was played recently.
        /// </summary>
        public bool PlayedRecently { get; set; }

        #region IEquatable<Game> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Game other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.Id == Id && Equals(other.Name, Name) && Equals(other.ImageUrl, ImageUrl) &&
                   Equals(other.StatsUrl, StatsUrl) && Equals(other.StoreUrl, StoreUrl) &&
                   other.PlayedRecently.Equals(PlayedRecently);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
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
            if (obj.GetType() != typeof (Game))
            {
                return false;
            }
            return Equals((Game) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Id;
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                result = (result*397) ^ (StatsUrl != null ? StatsUrl.GetHashCode() : 0);
                result = (result*397) ^ (StoreUrl != null ? StoreUrl.GetHashCode() : 0);
                result = (result*397) ^ PlayedRecently.GetHashCode();
                return result;
            }
        }
    }
}