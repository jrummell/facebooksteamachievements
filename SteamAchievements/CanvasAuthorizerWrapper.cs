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

using Facebook.Web;

namespace SteamAchievements
{
    public class CanvasAuthorizerWrapper : ICanvasAuthorizer
    {
        private readonly CanvasAuthorizer _authorizer = new CanvasAuthorizer {Perms = "publish_stream,offline_access"};

        #region ICanvasAuthorizer Members

        public string AppId
        {
            get { return _authorizer.AppId; }
        }

        public string Perms
        {
            get { return _authorizer.Perms; }
            set { _authorizer.Perms = value; }
        }

        public string UserId
        {
            get { return _authorizer.Session.UserId; }
        }

        public string AccessToken
        {
            get { return _authorizer.Session.AccessToken; }
        }

        public bool IsAuthorized()
        {
            return _authorizer.IsAuthorized();
        }

        public void HandleUnauthorizedRequest()
        {
            _authorizer.HandleUnauthorizedRequest();
        }

        #endregion
    }
}