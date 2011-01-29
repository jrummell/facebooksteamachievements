<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HelpLink.ascx.cs" Inherits="SteamAchievements.Controls.HelpLink" %>
<asp:HyperLink ID="helpLink" runat="server" CssClass="button" ClientIDMode="AutoID"
    Target="_blank">
    <asp:Image ID="helpImage" runat="server" ImageUrl="~/images/help.png" AlternateText="Help"
        ImageAlign="Middle" ClientIDMode="AutoID" />
    Help</asp:HyperLink>