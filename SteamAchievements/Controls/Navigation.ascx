<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Navigation.ascx.cs"
    Inherits="SteamAchievements.Controls.Navigation" %>
<steam:CanvasLink ID="menuHomeLink" runat="server" CanvasPage="" Text="Home" />
|
<steam:CanvasLink ID="menuPublishLink" runat="server" CanvasPage="Publish.aspx" Text="Publish" />
|
<steam:CanvasLink ID="menuSettingsLink" runat="server" CanvasPage="Settings.aspx"
    Text="Settings" />
|
<steam:CanvasLink ID="menuHelpLink" runat="server" CanvasPage="Help.aspx" Text="Help" />
