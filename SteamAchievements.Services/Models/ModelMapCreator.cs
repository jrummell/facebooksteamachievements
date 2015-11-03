﻿#region License

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
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Models
{
    public class ModelMapCreator
    {
        public void CreateMappings()
        {
            // User
            Mapper.CreateMap<Data.steam_User, User>()
                .ForMember(model => model.SignedRequest, options => options.Ignore());

            Mapper.CreateMap<User, Data.steam_User>()
                .ForMember(entity => entity.UserAchievements, options => options.Ignore())
                .ForMember(entity => entity.AccessFailedCount, options => options.Ignore())
                .ForMember(entity => entity.Claims, options => options.Ignore())
                .ForMember(entity => entity.Email, options => options.Ignore())
                .ForMember(entity => entity.EmailConfirmed, options => options.Ignore())
                .ForMember(entity => entity.Logins, options => options.Ignore())
                .ForMember(entity => entity.LockoutEnabled, options => options.Ignore())
                .ForMember(entity => entity.LockoutEndDateUtc, options => options.Ignore())
                .ForMember(entity => entity.PasswordHash, options => options.Ignore())
                .ForMember(entity => entity.PhoneNumber, options => options.Ignore())
                .ForMember(entity => entity.PhoneNumberConfirmed, options => options.Ignore())
                .ForMember(entity => entity.SecurityStamp, options => options.Ignore())
                .ForMember(entity => entity.TwoFactorEnabled, options => options.Ignore())
                .ForMember(entity => entity.Roles, options => options.Ignore())
                .ForMember(entity => entity.AccessToken,
                           options => options.MapFrom(model => model.AccessToken ?? String.Empty));

            // UserAchievement
            Mapper.CreateMap<UserAchievement, Data.steam_UserAchievement>()
                .ForMember(entity => entity.User, options => options.Ignore())
                .ForMember(entity => entity.Id, options => options.Ignore())
                .ForMember(entity => entity.Hidden, options => options.Ignore())
                .ForMember(entity => entity.UserId, options => options.Ignore())
                .ForMember(entity => entity.Published, options => options.Ignore());

            // Achievement
            Mapper.CreateMap<Achievement, Data.steam_Achievement>()
                .ForMember(entity => entity.UserAchievements, options => options.Ignore())
                .ForMember(entity => entity.AchievementNames,
                           options => options.MapFrom(model =>
                                                      new EntitySet<steam_AchievementName>
                                                          {
                                                              new steam_AchievementName
                                                                  {
                                                                      AchievementId = model.Id,
                                                                      Language = model.Language,
                                                                      Name = model.Name,
                                                                      Description = model.Description
                                                                  }
                                                          }));

            string language = CultureHelper.GetLanguage();
            Mapper.CreateMap<Data.steam_Achievement, Achievement>()
                .ForMember(model => model.Name, options => options.Ignore())
                .ForMember(model => model.Description, options => options.Ignore())
                .ForMember(model => model.Game, options => options.Ignore())
                .ForMember(model => model.Language, options => options.MapFrom(entity => language))
                .AfterMap((entity, model) =>
                              {
                                  steam_AchievementName name = entity.AchievementNames
                                                             .Where(n => n.Language == language)
                                                             .SingleOrDefault() ??
                                                         entity.AchievementNames.FirstOrDefault();
                                  if (name != null)
                                  {
                                      model.Name = name.Name;
                                      model.Description = name.Description;
                                  }
                              });
        }
    }
}