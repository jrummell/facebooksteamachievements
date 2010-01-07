﻿#region License

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
using Facebook.Schema;
using Facebook.Web;

namespace SteamAchievements
{
    public partial class Site : CanvasIFrameMasterPage
    {
        // helper for testing
#if DEBUG
        public readonly bool Debug = true;
#else
        public readonly bool Debug = false;
#endif

        public Site()
        {
            if (!Debug)
            {
                RequireLogin = true;
                RequiredPermissions = new List<Enums.ExtendedPermissions> { Enums.ExtendedPermissions.publish_stream };
            }
        }
    }
}