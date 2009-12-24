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

using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceContract]
    public interface IAchievementService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetAchievements?steamUserId={steamUserId}&gameId={gameId}",
            ResponseFormat = WebMessageFormat.Json)]
        AchievementCollection GetAchievements(string steamUserId, int gameId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetGames", RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Collection<Game> GetGames(); // returning IEnumerable<GameDTO> causes a serialization exception

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateAchievements?steamUserId={steamUserId}",
            ResponseFormat = WebMessageFormat.Json)]
        void UpdateAchievements(string steamUserId);
    }
}