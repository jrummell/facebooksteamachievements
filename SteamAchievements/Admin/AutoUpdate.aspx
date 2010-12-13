<%@ Page Title="Auto Update" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
    CodeBehind="AutoUpdate.aspx.cs" Inherits="SteamAchievements.Admin.AutoUpdate"
    EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>
        Auto Update</h1>
    <br />
    <div id="unauthorizedDiv" runat="server" class="fberrorbox" visible="false">
        Invalid auth key.
    </div>
</asp:Content>
