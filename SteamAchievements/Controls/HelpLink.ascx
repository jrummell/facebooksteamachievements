<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HelpLink.ascx.cs" Inherits="SteamAchievements.Controls.HelpLink" %>
<a class="button" href="Help.aspx?<%= Request.QueryString %>#<%= HelpAnchor %>">
    <asp:Image ID="helpImage" runat="server" ImageUrl="~/images/help.png" AlternateText="Help" ImageAlign="Middle" />
    Help</a>