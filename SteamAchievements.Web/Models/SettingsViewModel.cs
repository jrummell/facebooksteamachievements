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
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Resources;

namespace SteamAchievements.Web.Models
{
    public class SettingsViewModel : IValidatableObject
    {
        [Display(ResourceType = typeof (Strings), Name = "SettingsCustomUrl")]
        [Required(ErrorMessageResourceType = typeof (Strings), ErrorMessageResourceName = "SettingsCustomUrlRequired")]
        public string SteamUserId { get; set; }

        [Display(ResourceType = typeof (Strings), Name = "SettingsAutoUpdateLabel")]
        public bool AutoUpdate { get; set; }

        [Display(ResourceType = typeof (Strings), Name = "SettingsPublishDescriptionLabel")]
        public bool PublishDescription { get; set; }

        public long FacebookUserId { get; set; }

        public string SignedRequest { get; set; }

        public bool EnableLog { get; set; }

        #region IValidatableObject Members

        /// <summary>
        ///   Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext"> The validation context. </param>
        /// <returns> A collection that holds failed-validation information. </returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            const string steamUserIdMemberName = "SteamUserId";
            IDependencyResolver resolver = DependencyResolver.Current;

            // Validate the profile url.
            IAchievementService achievementService = resolver.GetService<IAchievementService>();
            SteamProfile profile;
            try
            {
                profile = achievementService.GetProfile(SteamUserId);
            }
            catch (Exception)
            {
                profile = null;
            }

            if (profile == null)
            {
                yield return new ValidationResult(Strings.SettingsInvalidCustomUrl, new[] {steamUserIdMemberName});
            }
        }

        #endregion
    }
}