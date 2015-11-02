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
    public class MockUserService : IUserService
    {
        #region IUserService Members

        public void Dispose()
        {
        }

        public ICollection<User> GetAutoUpdateUsers()
        {
            throw new NotImplementedException();
        }

        public User GetUser(string userName)
        {
            return new User {FacebookUserId = 1000, UserName = userName, SteamUserId = "SteamUser"};
        }

        public User GetUser(long facebookUserId)
        {
            return new User { FacebookUserId = facebookUserId, UserName = facebookUserId.ToString(), SteamUserId = "SteamUser" };
        }

        public void UpdateUser(User user)
        {
        }

        public void DeauthorizeUser(long facebookUserId)
        {
        }

        #endregion
    }
}