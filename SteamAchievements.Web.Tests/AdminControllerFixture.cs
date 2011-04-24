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

using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SteamAchievements.Services;
using SteamAchievements.Web.Controllers;

namespace SteamAchievements.Web.Tests
{
    [TestFixture]
    public class AdminControllerFixture
    {
        [Test]
        public void AutoUpdateUsers()
        {
            Mock<IAutoUpdateManager> managerMock = new Mock<IAutoUpdateManager>();
            managerMock.Setup(m => m.GetAutoUpdateUsers()).Returns("user1, user2").Verifiable();
            Mock<IAutoUpdateLogger> logMock = new Mock<IAutoUpdateLogger>();

            AdminController controller = new AdminController(managerMock.Object, logMock.Object);
            ContentResult result = controller.GetAutoUpdateUsers("1234");

            managerMock.Verify();
            Assert.That(result.Content, Is.EqualTo("user1, user2"));
        }

        [Test]
        public void PublishUserAchievements()
        {
            Mock<IAutoUpdateManager> managerMock = new Mock<IAutoUpdateManager>();
            managerMock.Setup(m => m.PublishUserAchievements("user1")).Verifiable();
            Mock<IAutoUpdateLogger> logMock = new Mock<IAutoUpdateLogger>();

            AdminController controller = new AdminController(managerMock.Object, logMock.Object);
            ContentResult result = controller.PublishUserAchievements("1234", "user1");

            managerMock.Verify();
            Assert.That(result.Content.Trim(), Is.EqualTo("user1 published."));
        }
    }
}