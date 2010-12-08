<%@ Page Title="Admin Home" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
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
        <a href="AutoUpdate.aspx?auth=<%= AutoUpdateAuthKey %>">Auto Update</a>
    </p>
    </form>
</asp:Content>
