<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Error.aspx.cs" Inherits="SteamAchievements.Error"
    EnableTheming="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error</title>
    <link id="faviconLink" runat="server" href="~/favicon.ico" rel="icon" type="image/x-icon" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="fbbody">
        <div>
            <steam:Logo ID="logo" runat="server" />
        </div>
        <div class="menu">
            <steam:Navigation ID="nav" runat="server" />
        </div>
        <div class="fberrorbox">
            There was an unexpected error. Don't worry, we already know that it happened and
            are punishing the person responsible. However, if you would like to offer any more
            information about what you were doing before the error occured, please tell us on
            the feedback forum linked below.
        </div>
        <div class="footer">
            <steam:Disclaimer ID="disclaimer" runat="server" />
            <script type="text/javascript" src="http://www.ohloh.net/p/483250/widgets/project_thin_badge.js"></script>
        </div>
    </div>
    </form>
</body>
</html>
