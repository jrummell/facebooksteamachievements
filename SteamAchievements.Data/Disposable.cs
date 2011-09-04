using System;

namespace SteamAchievements.Data
{
    public abstract class Disposable : IDisposable
    {
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DisposeManaged();
        }

        #endregion

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected abstract void DisposeManaged();
    }
}