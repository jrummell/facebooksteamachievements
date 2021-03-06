﻿#region License

//  Copyright 2013 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;

namespace SteamAchievements.Services
{
    public class SteamCommunityDataException : Exception
    {
        private const string _messageFormat =
            "The Steam Community Data API is not available at this time. Please try again later. (Could not access url {0})";

        /// <summary>
        ///     Initializes a new instance of the <see cref="SteamCommunityDataException" /> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="innerException">The inner exception.</param>
        public SteamCommunityDataException(Uri url, Exception innerException)
            : base(String.Format(_messageFormat, url), innerException)
        {
        }
    }
}