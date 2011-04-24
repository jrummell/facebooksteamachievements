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
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

// Fakes based on Stephen Walther's ASP.NET MVC Tip #12
// http://stephenwalther.com/blog/archive/2008/07/01/asp-net-mvc-tip-12-faking-the-controller-context.aspx

namespace SteamAchievements.Web.Tests
{
    public class FakeControllerContext : ControllerContext
    {
        public FakeControllerContext(IController controller)
            : this(controller, null, null, null, null, null, null)
        {
        }

        public FakeControllerContext(IController controller, HttpCookieCollection cookies)
            : this(controller, null, null, null, null, cookies, null)
        {
        }

        public FakeControllerContext(IController controller, SessionStateItemCollection sessionItems)
            : this(controller, null, null, null, null, null, sessionItems)
        {
        }


        public FakeControllerContext(IController controller, NameValueCollection formParams)
            : this(controller, null, null, formParams, null, null, null)
        {
        }


        public FakeControllerContext(IController controller, NameValueCollection formParams,
                                     NameValueCollection queryStringParams)
            : this(controller, null, null, formParams, queryStringParams, null, null)
        {
        }


        public FakeControllerContext(IController controller, string userName)
            : this(controller, userName, null, null, null, null, null)
        {
        }


        public FakeControllerContext(IController controller, string userName, string[] roles)
            : this(controller, userName, roles, null, null, null, null)
        {
        }


        public FakeControllerContext
            (
            IController controller,
            string userName,
            string[] roles,
            NameValueCollection formParams,
            NameValueCollection queryStringParams,
            HttpCookieCollection cookies,
            SessionStateItemCollection sessionItems
            )
            : base(
                new FakeHttpContext(new FakePrincipal(new FakeIdentity(userName), roles), formParams, queryStringParams,
                                    cookies, sessionItems), new RouteData(), (ControllerBase) controller)
        {
        }
    }

    public class FakeHttpContext : HttpContextBase
    {
        private readonly HttpCookieCollection _cookies;
        private readonly NameValueCollection _formParams;
        private readonly FakePrincipal _principal;
        private readonly NameValueCollection _queryStringParams;
        private readonly SessionStateItemCollection _sessionItems;

        public FakeHttpContext(FakePrincipal principal, NameValueCollection formParams,
                               NameValueCollection queryStringParams, HttpCookieCollection cookies,
                               SessionStateItemCollection sessionItems)
        {
            _principal = principal;
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
            _sessionItems = sessionItems;
        }

        public override HttpRequestBase Request
        {
            get { return new FakeHttpRequest(_formParams, _queryStringParams, _cookies); }
        }

        public override IPrincipal User
        {
            get { return _principal; }
            set { throw new NotImplementedException(); }
        }

        public override HttpSessionStateBase Session
        {
            get { return new FakeHttpSessionState(_sessionItems); }
        }
    }

    public class FakeHttpRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection _cookies;
        private readonly NameValueCollection _formParams;
        private readonly NameValueCollection _queryStringParams;

        public FakeHttpRequest(NameValueCollection formParams, NameValueCollection queryStringParams,
                               HttpCookieCollection cookies)
        {
            _formParams = formParams;
            _queryStringParams = queryStringParams;
            _cookies = cookies;
        }

        public override NameValueCollection Form
        {
            get { return _formParams; }
        }

        public override NameValueCollection QueryString
        {
            get { return _queryStringParams; }
        }

        public override HttpCookieCollection Cookies
        {
            get { return _cookies; }
        }
    }

    public class FakeHttpSessionState : HttpSessionStateBase
    {
        private readonly SessionStateItemCollection _sessionItems;

        public FakeHttpSessionState(SessionStateItemCollection sessionItems)
        {
            _sessionItems = sessionItems;
        }

        public override int Count
        {
            get { return _sessionItems.Count; }
        }

        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get { return _sessionItems.Keys; }
        }

        public override object this[string name]
        {
            get { return _sessionItems[name]; }
            set { _sessionItems[name] = value; }
        }

        public override object this[int index]
        {
            get { return _sessionItems[index]; }
            set { _sessionItems[index] = value; }
        }

        public override void Add(string name, object value)
        {
            _sessionItems[name] = value;
        }

        public override IEnumerator GetEnumerator()
        {
            return _sessionItems.GetEnumerator();
        }

        public override void Remove(string name)
        {
            _sessionItems.Remove(name);
        }
    }

    public class FakeIdentity : IIdentity
    {
        private readonly string _name;

        public FakeIdentity(string userName)
        {
            _name = userName;
        }

        #region IIdentity Members

        public string AuthenticationType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAuthenticated
        {
            get { return !String.IsNullOrEmpty(_name); }
        }

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }

    public class FakePrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private readonly string[] _roles;

        public FakePrincipal(IIdentity identity, string[] roles)
        {
            _identity = identity;
            _roles = roles;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (_roles == null)
            {
                return false;
            }
            return _roles.Contains(role);
        }

        #endregion
    }
}