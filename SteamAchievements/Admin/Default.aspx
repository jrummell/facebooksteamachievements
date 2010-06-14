<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Admin.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
    <h1>
        Admin Home</h1>
    <p>
        <a href="elmah.axd">Error Log</a>
    </p>
    <p>
        <asp:Button ID="updateAchievementsButton" runat="server" Text="Update Game Achievements"
            OnClick="updateAchievementsButton_Click" /></p>
    </form>
</asp:Content>
