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

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;
using UserAchievement = SteamAchievements.Data.UserAchievement;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class UserServiceFixture
    {
        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ISteamRepository>();
            _mapperMock = new Mock<IMapper>();

            _service = new UserService(_mapperMock.Object, _repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
        }

        private UserService _service;
        private Mock<ISteamRepository> _repositoryMock;
        private Mock<IMapper> _mapperMock;

        [Test]
        public void ChangeSteamUserId()
        {
            _repositoryMock.SetupGet(rep => rep.Users)
                           .Returns(new[] {new User {SteamUserId = "user1", Id = "1"}}.AsQueryable())
                           .Verifiable();
            _repositoryMock.SetupGet(rep => rep.UserAchievements)
                           .Returns(new[] {new UserAchievement {UserId = "1"}}.AsQueryable())
                           .Verifiable();
            _repositoryMock.Setup(rep => rep.DeleteAllOnSubmit(It.IsAny<IEnumerable<UserAchievement>>()))
                           .Verifiable();
            _repositoryMock.Setup(rep => rep.SubmitChanges())
                           .Verifiable();

            var user = new User {SteamUserId = "userxxx", Id = "1"};
            _service.ChangeSteamUserId(user.Id, user.SteamUserId);

            _repositoryMock.Verify();
            _repositoryMock.VerifyGet(rep => rep.Users, Times.Exactly(1));
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<User>()), Times.Never());
        }

        [Test]
        public void GetUserById()
        {
            var userId = 1234.ToString();
            var user = new User {Id = userId};
            _repositoryMock.SetupGet(r => r.Users)
                           .Returns(new[] {user}.AsQueryable())
                           .Verifiable();

            var model = new UserModel();
            _mapperMock.Setup(m => m.Map<UserModel>(user))
                       .Returns(model)
                       .Verifiable();

            var actual = _service.GetUser(userId);

            Assert.That(actual, Is.SameAs(model));

            _repositoryMock.Verify();
            _mapperMock.Verify();
        }

        [Test]
        public void ValidateDate()
        {
            var date = AchievementManager.ValidateDate(DateTime.MinValue);

            var sqlMinTicks = SqlDateTime.MinValue.Value.Ticks;
            Assert.That(date.Ticks, Is.GreaterThan(sqlMinTicks));
        }
    }
}