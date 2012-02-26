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

using System.Web.Mvc;
using System.Web.SessionState;
using Moq;
using NUnit.Framework;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Controllers;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Tests
{
    [TestFixture]
    public class HomeControllerFixture
    {
    	[TestFixtureSetUp]
    	public void TestFixtureSetUp()
    	{
    		ModelMapCreator mapCreator = new ModelMapCreator();
            mapCreator.CreateMappings();
            
    		ViewModelMapCreator vmMapCreator = new ViewModelMapCreator();
    		vmMapCreator.CreateMappings();
    	}
    	
        [Test]
        public void SettingsDuplicate()
        {
            MockFacebookContextSettings facebookContextSettings =
                new MockFacebookContextSettings("AppId", 1234, "AccessToken123", "http://apps.facebook.com/canvasPage",
                                                "SignedRequest");

            Mock<IUserService> mockUserService = new Mock<IUserService>();
            User originalUser = new User
                                    {
                                        AutoUpdate = true,
                                        FacebookUserId = facebookContextSettings.UserId,
                                        PublishDescription = true,
                                        SteamUserId = "user1"
                                    };
            mockUserService.Setup(service => service.GetUser(facebookContextSettings.UserId))
                .Returns(() => originalUser).Verifiable();

            User updatedUser = new User
                                   {
                                       AccessToken = originalUser.AccessToken,
                                       AutoUpdate = originalUser.AutoUpdate,
                                       FacebookUserId = originalUser.FacebookUserId,
                                       PublishDescription = originalUser.PublishDescription,
                                       SteamUserId = "NullReference"
                                   };
            mockUserService.Setup(service => service.UpdateUser(updatedUser))
                .Throws(new DuplicateSteamUserException()).Verifiable();

            Mock<IAchievementService> mockAchievementService = new Mock<IAchievementService>();

            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            HomeController controller = new HomeController(mockAchievementService.Object,
                                                           mockUserService.Object,
                                                           facebookContextSettings);
            FakeControllerContext context = new FakeControllerContext(controller, sessionItems);
            controller.ControllerContext = context;
            controller.UserSettings = originalUser;

            SettingsViewModel model =
                new SettingsViewModel
                    {
                        AutoUpdate = true,
                        PublishDescription = true,
                        SteamUserId = "NullReference"
                    };

            Assert.That(() => controller.SaveSettings(model), Throws.InstanceOf<DuplicateSteamUserException>());

            mockUserService.Verify();
        }

        [Test]
        public void SettingsSuccess()
        {
            Mock<IAchievementService> mockAchievementService = new Mock<IAchievementService>();

            MockFacebookContextSettings facebookContextSettings =
                new MockFacebookContextSettings("AppId", 1234567890, "AccessToken123",
                                                "http://apps.facebook.com/canvasPage", "SignedRequest");

            Mock<IUserService> mockUserService = new Mock<IUserService>();
            User originalUser = new User
                                    {
                                        AutoUpdate = true,
                                        FacebookUserId = facebookContextSettings.UserId,
                                        PublishDescription = true,
                                        SteamUserId = "NullReference"
                                    };
            mockUserService.Setup(service => service.GetUser(facebookContextSettings.UserId))
                .Returns(() => originalUser).Verifiable();

            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            HomeController controller = new HomeController(mockAchievementService.Object,
                                                           mockUserService.Object,
                                                           facebookContextSettings);
            FakeControllerContext context = new FakeControllerContext(controller, sessionItems);
            controller.ControllerContext = context;

            controller.UserSettings = originalUser;

            SettingsViewModel model =
                new SettingsViewModel
                    {
                        AutoUpdate = true,
                        PublishDescription = true,
                        SteamUserId = originalUser.SteamUserId
                    };

            ViewResult result = (ViewResult) controller.SaveSettings(model);

            Assert.That((bool) result.ViewBag.Success, Is.True);
            mockUserService.Verify();
        }
    }
}