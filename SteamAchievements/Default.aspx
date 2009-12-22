<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Default" %>
<%@ Import Namespace="SteamAchievements.Data"%>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="body">

    <h2>
        Steam Achievements</h2>
    <p>
        <label for="steamIdTextBox">Steam Community ID</label><br />
        http://steamcommunity.com/id/
        <input id="steamIdTextBox"/>
        <span id="steamIdError" style="display:none; color:Red;">Required</span>
        <input type="button" value="Update" onclick="return updateSteamUserId();" />
    </p>
    <asp:Repeater ID="achievementsRepeater" runat="server">
        <ItemTemplate>
            <div>
                <div style="float: left;">
                    <img src="<%# ((Achievement)Container.DataItem).ImageUrl %>" alt="" />
                </div>
                <div>
                    <strong>
                        <%# ((Achievement)Container.DataItem).Name %></strong>
                    <p>
                        <%# ((Achievement)Container.DataItem).Description %></p>
                </div>
            </div>
        </ItemTemplate>
        <SeparatorTemplate>
            <br style="clear: both;" />
        </SeparatorTemplate>
    </asp:Repeater>
    
    <script type="text/javascript">
        function updateSteamUserId()
        {
            document.getElementById("steamIdError").setStyle("display", "none");
        
            var steamUserId = document.getElementById("steamIdTextBox").getValue();

            if (steamUserId == null || steamUserId == "")
            {
                document.getElementById("steamIdError").setStyle("display", "inline");
                return false;
            }
            
            //TODO: update achievements with web service
        }
    </script>
</asp:Content>
