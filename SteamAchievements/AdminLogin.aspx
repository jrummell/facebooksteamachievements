<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
    CodeBehind="AdminLogin.aspx.cs" Inherits="SteamAchievements.AdminLogin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
    <asp:Login ID="login" runat="server" DestinationPageUrl="~/Admin/Default.aspx" DisplayRememberMe="False"
        OnAuthenticate="LoginAuthenticate">
    </asp:Login>
    </form>
</asp:Content>
