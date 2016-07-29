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

using Moq;
using NUnit.Framework;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class UserServiceFixture
    {
        [SetUp]
        public void SetUp()
        {
            _managerMock = new Mock<IAchievementManager>();
            _service = new UserService(_managerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
        }

        private IUserService _service;
        private Mock<IAchievementManager> _managerMock;

        [Test]
        public void DeauthorizeUser()
        {
            const int facebookUserId = 1234;
            _managerMock.Setup(manager => manager.DeauthorizeUser(facebookUserId))
                        .Verifiable();

            _service.DeauthorizeUser(facebookUserId);

            _managerMock.Verify();
        }

        [Test]
        public void GetUserByFacebookId()
        {
            const int userId = 1234;
            _managerMock.Setup(manager => manager.GetUser(userId))
                        .Returns(new steam_User {Id = userId})
                        .Verifiable();

            var user = _service.GetUser(userId);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Id, Is.EqualTo(userId));
            _managerMock.Verify();
        }

        [Test]
        public void UpdateUser()
        {
            var user = new User {Id = 1234, SteamUserId = "user1"};

            var managerMock = new Mock<IAchievementManager>();
            managerMock.Setup(
                              rep =>
                              rep.UpdateUser(
                                             It.Is<steam_User>(u => u.SteamUserId == user.SteamUserId && u.Id == user.Id)))
                       .Verifiable();

            var service = new UserService(managerMock.Object);
            service.UpdateUser(user);

            managerMock.Verify();
        }
    }
}