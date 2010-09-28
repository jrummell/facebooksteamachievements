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
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Session;
using SteamAchievements.Services.Properties;

namespace SteamAchievements.Services
{
    public static class FacebookApiFactory
    {
        public static Api CreateInstance(IEnumerable<Enums.ExtendedPermissions> permissions)
        {
            string appKey = Settings.Default.APIKey;
            string appSecret = Settings.Default.ApplicationSecret;

            List<Enums.ExtendedPermissions> list = permissions == null ? new List<Enums.ExtendedPermissions>() : permissions.ToList();

            CanvasSession session = new IFrameCanvasSession(appKey, appSecret, list, false);
            return new Api(session);
        }
    }
}
