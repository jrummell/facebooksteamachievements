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
using System.ServiceModel;
using System.ServiceModel.Web;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceContract]
    public interface IAchievementService
    {
        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <returns>All <see cref="Achievement"/>s for the given user and game.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Achievement> GetAchievements(GetAchievementsParameter json);

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns>All <see cref="Game"/>s.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Game> GetGames(); // returning IEnumerable<Game> causes a serialization exception

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        /// <returns>true if successful, else false.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateAchievements(UpdateAchievementsParameter json);

        /// <summary>
        /// Updates the steam user id.
        /// </summary>
        /// <param name="json">The json object.</param>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        /// <returns>true if successful, else false.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateSteamUserId(UpdateSteamUserIdParameter json);
    }

    public class GetAchievementsParameter
    {
        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>The steam user id.</value>
        public string SteamUserId { get; set; }

        /// <summary>
        /// Gets or sets the game id.
        /// </summary>
        /// <value>The game id.</value>
        public int GameId { get; set; }
    }

    public class UpdateAchievementsParameter
    {
        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>The steam user id.</value>
        public string SteamUserId { get; set; }
    }

    public class UpdateSteamUserIdParameter
    {
        /// <summary>
        /// Gets or sets the facebook user id.
        /// </summary>
        /// <value>The facebook user id.</value>
        public long FacebookUserId { get; set; }

        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>The steam user id.</value>
        public string SteamUserId { get; set; }
    }
}