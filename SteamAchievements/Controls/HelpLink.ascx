﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HelpLink.ascx.cs" Inherits="SteamAchievements.Controls.HelpLink" %>
<steam:CanvasLink ID="helpLink" runat="server" CssClass="button" ClientIDMode="AutoID">
    <asp:Image ID="helpImage" runat="server" ImageUrl="~/images/help.png" AlternateText="Help"
        ImageAlign="Middle" ClientIDMode="AutoID" />
    Help</steam:CanvasLink>