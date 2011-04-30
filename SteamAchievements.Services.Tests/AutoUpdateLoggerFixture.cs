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
using System.IO;
using NUnit.Framework;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AutoUpdateLoggerFixture
    {
        private const string _logPath = "testlog";

        [Test]
        public void Delete()
        {
            AutoUpdateLogger log = new AutoUpdateLogger(_logPath);

            string[] files = Directory.GetFiles(_logPath);
            Assert.That(files.Length, Is.EqualTo(3));

            log.Delete(new DateTime(2011, 1, 2));

            // make sure the file named 2011-1-1.log was deleted and 2011-1-2.log and the current date remain.
            files = Directory.GetFiles(_logPath);
            Assert.That(files.Length, Is.EqualTo(2));
            Assert.That(Path.GetFileName(files[0]), Is.EqualTo("2011-1-2.log"));
        }
    }
}