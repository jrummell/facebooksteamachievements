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
using Moq;
using NUnit.Framework;
using NUnit.Mocks;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class UserServiceFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _managerMock = new DynamicMock(typeof (IAchievementManager));
            IAchievementManager manager = (IAchievementManager) _managerMock.MockInstance;
            _service = new UserService(manager);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
        }

        #endregion

        private IUserService _service;
        private DynamicMock _managerMock;

        [Test]
        public void DeauthorizeUser()
        {
            const int facebookUserId = 1234;
            _managerMock.Expect("DeauthorizeUser", facebookUserId);

            _service.DeauthorizeUser(facebookUserId);

            _managerMock.Verify();
        }

        [Test]
        public void GetAutoUpdateUsers()
        {
            const string steamUserId = "user1";
            _managerMock.ExpectAndReturn("GetAutoUpdateUsers",
                                         new List<Data.User> {new Data.User {SteamUserId = steamUserId}});

            IEnumerable<string> users = _service.GetAutoUpdateUsers();

            Assert.That(users.Count(), Is.EqualTo(1));
            Assert.That(users.First(), Is.EqualTo(steamUserId));
            _managerMock.Verify();
        }

        [Test]
        public void GetUserByFacebookId()
        {
            const int facebookUserId = 1234;
            _managerMock.ExpectAndReturn("GetUser", new Data.User {FacebookUserId = facebookUserId}, facebookUserId);

            User user = _service.GetUser(facebookUserId);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.FacebookUserId, Is.EqualTo(facebookUserId));
            _managerMock.Verify();
        }

        [Test]
        public void GetUserBySteamId()
        {
            const string steamUserId = "user1";
            _managerMock.ExpectAndReturn("GetUser", new Data.User {SteamUserId = steamUserId}, steamUserId);

            User user = _service.GetUser(steamUserId);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.SteamUserId, Is.EqualTo(steamUserId));
            _managerMock.Verify();
        }

        [Test]
        public void UpdateUser()
        {
            User user = new User {AccessToken = "x", AutoUpdate = true, FacebookUserId = 1234, SteamUserId = "user1"};

            Mock<IAchievementManager> managerMock = new Mock<IAchievementManager>();
            managerMock.Setup(rep => rep.IsDuplicate(user.SteamUserId, user.FacebookUserId))
                .Returns(false).Verifiable();
            managerMock.Setup(
                rep =>
                rep.UpdateUser(
                    It.Is<Data.User>(u => u.SteamUserId == user.SteamUserId && u.FacebookUserId == user.FacebookUserId)))
                .Verifiable();

            UserService service = new UserService(managerMock.Object);
            service.UpdateUser(user);

            managerMock.Verify();
        }

        [Test]
        public void UpdateUser_Duplicate()
        {
            User user = new User {AccessToken = "x", AutoUpdate = true, FacebookUserId = 1234, SteamUserId = "user1"};
            _managerMock.ExpectAndReturn("IsDuplicate", true, user.SteamUserId, user.FacebookUserId);
            _managerMock.ExpectNoCall("UpdateUser");

            Assert.That(() => _service.UpdateUser(user), Throws.TypeOf(typeof (DuplicateSteamUserException)));

            _managerMock.Verify();
        }
    }
}