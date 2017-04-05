#region License

//  Copyright 2012 John Rummell
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
using System.Collections.Generic;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    [Obsolete("replace with moq")]
    public class MockUserService : IUserService
    {
        #region IUserService Members

        public void Dispose()
        {
        }

        public UserModel GetUser(string userName)
        {
            return new UserModel {Id = 1000, UserName = userName, SteamUserId = "SteamUser"};
        }

        public UserModel GetUser(int userId)
        {
            return new UserModel { Id = userId, UserName = userId.ToString(), SteamUserId = "SteamUser" };
        }

        public void UpdateUser(UserModel user)
        {
        }

        public void DeauthorizeUser(int userId)
        {
        }

        #endregion
    }
}