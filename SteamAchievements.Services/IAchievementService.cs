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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SteamAchievements.Services
{
    [ServiceContract]
    public interface IAchievementService : IDisposable
    {
        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="gameId">The game id.</param>
        /// <returns>
        /// All <see cref="Achievement"/>s for the given user and game.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Obsolete("GetNewAchievements()")]
        List<SimpleAchievement> GetAchievements(string steamUserId, int gameId);

        /// <summary>
        /// Gets the new achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>
        /// The new achievements that haven't been stored yet.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SimpleAchievement> GetNewAchievements(string steamUserId);

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>
        /// 	<see cref="Game"/>s for the givem steam user id.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SimpleGame> GetGames(string steamUserId);

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>The number of achievements that were updated.</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Obsolete("GetNewAchievements() and PublishAchievements()")]
        int UpdateAchievements(string steamUserId);

        /// <summary>
        /// Updates the steam user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool UpdateSteamUserId(long facebookUserId, string steamUserId);

        /// <summary>
        /// Publishes the last 5 achievements added within the last hour for the given user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Obsolete("PublishAchievements()")]
        bool PublishLatestAchievements(long facebookUserId, string steamUserId);

        /// <summary>
        /// Updates the Published field for the given achievements for the given user.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool PublishAchievements(string steamUserId, IEnumerable<int> achievementIds);
    }
}