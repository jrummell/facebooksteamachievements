<%@ Page Title="Auto Update" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
    CodeBehind="AutoUpdate.aspx.cs" Inherits="SteamAchievements.Admin.AutoUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>
        Auto Update</h1>
    <br />
    <div id="unauthorizedDiv" runat="server" class="fberrorbox" visible="false">
        Invalid auth key.
    </div>
    <div id="noUpdatesDiv" runat="server" class="fbinfobox" visible="false">
        No updates.
    </div>
    <asp:Repeater ID="userRepeater" runat="server">
        <ItemTemplate>
            <div>
                <%# Eval("SteamUserId") %>, Game:
                <%# Eval("GameName") %>, Achievement:
                <%# Eval("Description") %>, Exception:
                <%# Eval("ExceptionMessage")%>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
