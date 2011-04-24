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

using AutoMapper;
using Bootstrap;
using SteamAchievements.Services;

namespace SteamAchievements.Web.Models
{
    public class ModelMapCreator : IMapCreator
    {
        #region IMapCreator Members

        public void CreateMap()
        {
            Mapper.CreateMap<User, SettingsViewModel>();

            Mapper.CreateMap<SettingsViewModel, User>()
                .ForMember(user => user.AccessToken, options => options.Ignore())
                .ForMember(user => user.FacebookUserId, options => options.Ignore());
        }

        #endregion
    }
}