using System;
using System.Collections.Generic;
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    public class Publisher : IDisposable
    {
        private readonly IAutoUpdateManager _autoUpdateManager;

        public Publisher(IAutoUpdateManager autoUpdateManager)
        {
            _autoUpdateManager = autoUpdateManager;
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        public void Publish()
        {
            ICollection<string> users = _autoUpdateManager.GetAutoUpdateUsers();

            Console.WriteLine("Users: {0}", String.Join(", ", users));

            foreach (string user in users)
            {
                Console.Write("Publishing {0} ... ", user);
                _autoUpdateManager.PublishUserAchievements(user);
                Console.WriteLine("done.");
            }

            Console.WriteLine("All users published.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoUpdateManager.Dispose();
            }
        }
    }
}