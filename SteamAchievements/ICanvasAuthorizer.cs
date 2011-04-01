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

namespace SteamAchievements
{
    public interface ICanvasAuthorizer
    {
        /// <summary>
        /// Gets the app id.
        /// </summary>
        string AppId { get; }

        /// <summary>
        /// Gets or sets the perms.
        /// </summary>
        /// <value>
        /// The perms.
        /// </value>
        string Perms { get; set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Determines whether this instance is authorized.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is authorized; otherwise, <c>false</c>.
        /// </returns>
        bool IsAuthorized();

        /// <summary>
        /// Handles the unauthorized request.
        /// </summary>
        void HandleUnauthorizedRequest();
    }
}