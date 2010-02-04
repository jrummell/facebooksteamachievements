using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Data
{
    /// <summary>
    /// A mock implementation of <see cref="ISteamRepository"/>.
    /// </summary>
    /// <remarks>
    /// Insert/Update/DeleteOnSubmit methods perform the operation immediately. <see cref="SubmitChanges"/> does nothing.
    /// </remarks>
    public class MockSteamRepository : ISteamRepository
    {
        #region Fields

        private List<Achievement> _achievements =
            new List<Achievement>
                {
                    new Achievement
                        {
                            Id = 1,
                            GameId = 1,
                            Name = "Achievement 1 Game 1",
                            Description = "Do some thing cool in Game 1",
                            ImageUrl =
                                "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg"
                        },
                    new Achievement
                        {
                            Id = 2,
                            GameId = 1,
                            Name = "Achievement 2 Game 1",
                            Description = "Do some thing cool in Game 1",
                            ImageUrl =
                                "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg"
                        },
                    new Achievement
                        {
                            Id = 3,
                            GameId = 1,
                            Name = "Achievement 3 Game 1",
                            Description = "Do some thing cool in Game 1",
                            ImageUrl =
                                "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg"
                        },
                    new Achievement
                        {
                            Id = 4,
                            GameId = 2,
                            Name = "Achievement 1 Game 2",
                            Description = "Do some thing cool in Game 2",
                            ImageUrl =
                                "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg"
                        },
                    new Achievement
                        {
                            Id = 5,
                            GameId = 2,
                            Name = "Achievement 2 Game 2",
                            Description = "Do some thing cool in Game 2",
                            ImageUrl =
                                "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg"
                        }
                };

        private List<Game> _games =
            new List<Game>
                {
                    new Game {Id = 1, Abbreviation = "game1", Name = "Game 1"},
                    new Game {Id = 2, Abbreviation = "game2", Name = "Game 2"}
                };

        private List<UserAchievement> _userAchievements =
            new List<UserAchievement>
                {
                    new UserAchievement
                        {
                            Id = 1,
                            AchievementId = 1,
                            Date = DateTime.Now,
                            SteamUserId = "user1",
                            Achievement = new Achievement
                                              {
                                                  Id = 1,
                                                  GameId = 1,
                                                  Name = "Achievement 1 Game 1",
                                                  Description = "Do some thing cool in Game 1",
                                                  ImageUrl =
                                                      "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg",
                                                  Game = new Game {Id = 1, Abbreviation = "game1", Name = "Game 1"}
                                              }
                        },
                    new UserAchievement
                        {
                            Id = 2,
                            AchievementId = 2,
                            Date = DateTime.Now,
                            SteamUserId = "user1",
                            Achievement = new Achievement
                                              {
                                                  Id = 2,
                                                  GameId = 1,
                                                  Name = "Achievement 2 Game 1",
                                                  Description = "Do some thing cool in Game 1",
                                                  ImageUrl =
                                                      "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg",
                                                  Game = new Game {Id = 1, Abbreviation = "game1", Name = "Game 1"}
                                              }
                        },
                    new UserAchievement
                        {
                            Id = 3,
                            AchievementId = 3,
                            Date = DateTime.Now,
                            SteamUserId = "user1",
                            Achievement = new Achievement
                                              {
                                                  Id = 3,
                                                  GameId = 1,
                                                  Name = "Achievement 3 Game 1",
                                                  Description = "Do some thing cool in Game 1",
                                                  ImageUrl =
                                                      "http://media.steampowered.com/steamcommunity/public/images/apps/550/8a1dbb0d78c8e288ed5ce990a20454073d01ba9b.jpg",
                                                  Game = new Game {Id = 1, Abbreviation = "game1", Name = "Game 1"}
                                              }
                        }
                };

        private List<User> _users = new List<User> {new User {FacebookUserId = 1234567890, SteamUserId = "user1"}};

        #endregion

        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        public IQueryable<Achievement> Achievements
        {
            get { return _achievements.AsQueryable(); }
            set { _achievements = new List<Achievement>(value); }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        public IQueryable<UserAchievement> UserAchievements
        {
            get { return _userAchievements.AsQueryable(); }
            set { _userAchievements = new List<UserAchievement>(value); }
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <value>The games.</value>
        public IQueryable<Game> Games
        {
            get { return _games.AsQueryable(); }
            set { _games = new List<Game>(value); }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<User> Users
        {
            get { return _users.AsQueryable(); }
            set { _users = new List<User>(value); }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            _users.Add(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _userAchievements.RemoveAll(ua => achievements.Contains(ua));
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            achievement.Id = _achievements.Max(a => a.Id) + 1;
            _achievements.Add(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            int maxId = _userAchievements.Max(ua => ua.Id);

            foreach (UserAchievement userAchievement in achievements)
            {
                userAchievement.Id = ++maxId;
            }

            _userAchievements.AddRange(achievements);
        }

        #endregion
    }
}