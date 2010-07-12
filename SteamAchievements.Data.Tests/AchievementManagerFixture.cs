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
using System.Linq;
using NUnit.Framework;

namespace SteamAchievements.Data.Tests
{
    [TestFixture]
    public class AchievementManagerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            IQueryable<Achievement> achievements =
                (new[]
                     {
                         new Achievement
                             {Id = 1, GameId = 1, Name = "Achievement 1 for Game 1"},
                         new Achievement
                             {Id = 2, GameId = 1, Name = "Achievement 2 for Game 1"},
                         new Achievement
                             {Id = 3, GameId = 1, Name = "Achievement 3 for Game 1"},
                         new Achievement
                             {Id = 4, GameId = 2, Name = "Achievement 1 for Game 2"},
                         new Achievement
                             {Id = 5, GameId = 2, Name = "Achievement 2 for Game 2"}
                     }).AsQueryable();

            IQueryable<User> users =
                (new[]
                     {
                         new User {FacebookUserId = 1234567890, SteamUserId = "user1"},
                         new User {FacebookUserId = 1234567891, SteamUserId = "user2"}
                     }).AsQueryable();

            IQueryable<UserAchievement> userAchievements =
                (new[]
                     {
                         new UserAchievement
                             {
                                 Id = 1,
                                 AchievementId = 1,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1"
                             },
                         new UserAchievement
                             {
                                 Id = 2,
                                 AchievementId = 2,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1"
                             },
                         new UserAchievement
                             {
                                 Id = 3,
                                 AchievementId = 3,
                                 Date = DateTime.Now,
                                 SteamUserId = "user1"
                             }
                     }).AsQueryable();

            _repository = new MockSteamRepository
                              {
                                  Achievements = achievements,
                                  Users = users,
                                  UserAchievements = userAchievements
                              };

            _manager = new AchievementManager(_repository);
        }

        [TearDown]
        public void TearDown()
        {
            _manager.Dispose();
        }

        #endregion

        private AchievementManager _manager;
        private MockSteamRepository _repository;

        [Test]
        public void AddGameAchievements()
        {
            int gameId = 99;
            List<Achievement> gameAchievements = new List<Achievement>();
            for (int i = 0; i < 10; i++)
            {
                Achievement achievement = new Achievement
                                              {
                                                  Name = "Achievement " + i,
                                                  Description = "Achievement " + i,
                                                  ImageUrl = "http://example.com/achievement" + i + ".gif"
                                              };
                gameAchievements.Add(achievement);
            }

            _manager.AddAchievements(99, gameAchievements);

            Assert.That(_repository.Achievements.Count(a => a.GameId == gameId), Is.EqualTo(10));
        }

        [Test]
        public void AssignAchievements()
        {
            const string steamUserId = "user1";
            IEnumerable<int> achievementIds = _manager.GetUnassignedAchievementIds(steamUserId, _repository.Achievements);
            Assert.That(achievementIds.Any());

            IEnumerable<Achievement> achievements =
                from id in achievementIds
                select _repository.Achievements.Single(a => a.Id == id);

            Assert.That(achievementIds.Count(), Is.EqualTo(achievements.Count()));

            _manager.AssignAchievements(steamUserId, achievements);

            foreach (Achievement achievement in achievements)
            {
                int achievementId = achievement.Id;
                int count =
                    _repository.UserAchievements.Count(
                        ua => ua.SteamUserId == steamUserId && ua.AchievementId == achievementId);
                Assert.That(count, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetMissingAchievements()
        {
            Achievement achievement1Game5 = new Achievement {GameId = 5, Name = "Achievement 1 for Game 5"};
            Achievement achievement2Game5 = new Achievement {GameId = 5, Name = "Achievement 2 for Game 5"};
            IEnumerable<Achievement> communityAchievements =
                new[]
                    {
                        new Achievement {GameId = 1, Name = "Achievement 1 for Game 1"},
                        new Achievement {GameId = 1, Name = "Achievement 2 for Game 1"},
                        new Achievement {GameId = 1, Name = "Achievement 3 for Game 1"},
                        achievement1Game5,
                        achievement2Game5
                    };

            IEnumerable<Achievement> missingAchievements =
                _manager.GetMissingAchievements(communityAchievements);

            Assert.That(missingAchievements.Count(), Is.EqualTo(2));

            CollectionAssert.Contains(missingAchievements, achievement1Game5);
            CollectionAssert.Contains(missingAchievements, achievement2Game5);
        }

        [Test]
        public void GetUnassignedAchievementIds()
        {
            IEnumerable<int> achievementIds = _manager.GetUnassignedAchievementIds("user1", _repository.Achievements);

            Assert.That(achievementIds.Contains(4));
            Assert.That(achievementIds.Contains(5));
        }

        [Test]
        public void InsertMissingAchievements()
        {
            Achievement achievement1Game5 = new Achievement {GameId = 5, Name = "Achievement 1 for Game 5"};
            Achievement achievement2Game5 = new Achievement {GameId = 5, Name = "Achievement 2 for Game 5"};

            _manager.InsertMissingAchievements(new[] {achievement1Game5, achievement2Game5});

            Assert.That(_repository.Achievements.Count(a => a.Name == achievement1Game5.Name), Is.EqualTo(1));
            Assert.That(_repository.Achievements.Count(a => a.Name == achievement2Game5.Name), Is.EqualTo(1));
        }

        [Test]
        public void UpdateAchievements()
        {
            IEnumerable<Achievement> achievements =
                new[]
                    {
                        new Achievement
                            {Id = 1, GameId = 1, Name = "Achievement 1 for Game 1"},
                        new Achievement
                            {Id = 2, GameId = 1, Name = "Achievement 2 for Game 1"},
                        new Achievement
                            {Id = 3, GameId = 1, Name = "Achievement 3 for Game 1"},
                        new Achievement
                            {Id = 4, GameId = 2, Name = "Achievement 1 for Game 2"},
                        new Achievement
                            {Id = 5, GameId = 2, Name = "Achievement 2 for Game 2"},
                        new Achievement
                            {Id = 6, GameId = 2, Name = "Achievement 3 for Game 2"},
                        new Achievement
                            {Id = 7, GameId = 2, Name = "Achievement 4 for Game 2"},
                        new Achievement
                            {Id = 8, GameId = 2, Name = "Achievement 5 for Game 2"},
                        new Achievement
                            {Id = 9, GameId = 2, Name = "Achievement 6 for Game 2"}
                    };

            const string steamUserId = "user1";
            _manager.UpdateAchievements(steamUserId, achievements);

            foreach (Achievement achievement in achievements)
            {
                // assert that the new achievements were added
                int achievementId = achievement.Id;
                int gameId = achievement.GameId;
                string name = achievement.Name;
                int achievementCount =
                    _repository.Achievements.Count(a => a.Id == achievementId && a.GameId == gameId && a.Name == name);
                Assert.That(achievementCount, Is.EqualTo(1));

                // assert that the new achievements were assigned
                int userAchievementCount =
                    _repository.UserAchievements.Count(
                        ua => ua.SteamUserId == steamUserId && ua.AchievementId == achievementId);
                Assert.That(userAchievementCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void UpdateSteamUserId()
        {
            const string steamUserId = "userxxx";
            const int facebookUserId = 1234567890;
            _manager.UpdateSteamUserId(facebookUserId, steamUserId);

            Assert.That(_repository.Users.Single(u => u.FacebookUserId == facebookUserId).SteamUserId,
                        Is.EqualTo(steamUserId));
            Assert.That(!_repository.UserAchievements.Any(ua => ua.SteamUserId == steamUserId));
        }
    }
}