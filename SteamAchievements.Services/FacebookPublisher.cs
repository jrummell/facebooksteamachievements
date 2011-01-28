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
using Facebook;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    /// <summary>
    /// Description of FacebookPublisher.
    /// </summary>
    internal class FacebookPublisher : IFacebookPublisher
    {
        public void Publish(User user, IDictionary<string, object> parameters)
        {
            FacebookApp app = new FacebookApp(user.AccessToken);
            string userFeedPath = String.Format("/{0}/feed/", user.FacebookUserId);
            app.Api(userFeedPath, parameters, HttpMethod.Post);
        }
    }
}
