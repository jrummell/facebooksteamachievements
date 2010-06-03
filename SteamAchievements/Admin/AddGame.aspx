<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Site.Master" AutoEventWireup="false"
    CodeBehind="AddGame.aspx.cs" Inherits="SteamAchievements.Admin.AddGame" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #gameImage
        {
            border: 1px solid #333;
            width: 184px;
            height: 69px;
            display: block;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
    <h1>
        Add a Game</h1>
    <div class="form">
        <asp:Label ID="abbreviationLabel" runat="server" AssociatedControlID="abbreviationTextBox">Game Abbreviation:</asp:Label>
        <asp:TextBox ID="abbreviationTextBox" runat="server" />
        <asp:RequiredFieldValidator ID="abbreviationValidator" runat="server" ControlToValidate="abbreviationTextBox"
            Display="Dynamic" ErrorMessage="Abbreviation is required." />
        <asp:Label ID="nameLabel" runat="server" AssociatedControlID="nameTextBox">Game Name:</asp:Label>
        <asp:TextBox ID="nameTextBox" runat="server" />
        <asp:RequiredFieldValidator ID="nameValidator" runat="server" ControlToValidate="nameTextBox"
            Display="Dynamic" ErrorMessage="Name is required." />
        <asp:Label ID="imageLabel" runat="server" AssociatedControlID="imageTextBox">Game Image URL:</asp:Label>
        <asp:TextBox ID="imageTextBox" runat="server" Width="50ex" />
        <asp:RequiredFieldValidator ID="imageValidator" runat="server" ControlToValidate="imageTextBox"
            Display="Dynamic" ErrorMessage="Image is required." />
        <img id="gameImage" src="" width="184" height="69" alt="Invalid Image URL" />
        <asp:Label ID="steamUserIdLabel" runat="server" AssociatedControlID="steamUserIdTextBox">Steam User ID:</asp:Label>
        <asp:TextBox ID="steamUserIdTextBox" runat="server" />
        (To add achievements)
        <asp:RequiredFieldValidator ID="steamUserIdValidator" runat="server" ControlToValidate="steamUserIdTextBox"
            Display="Dynamic" ErrorMessage="Steam User ID is required." />
        <br />
        <asp:Button ID="addButton" runat="server" Text="Add" OnClick="addButton_Click" />
    </div>
    </form>

    <script type="text/javascript">
        $(document).ready(function()
        {
            // update the preview image when the image text box value changes
            $("#<%= imageTextBox.ClientID %>").change(updateImage).blur(updateImage);
        });

        function updateImage()
        {
            var url = $("#<%= imageTextBox.ClientID %>").val();
            $("#gameImage").attr("src", url);
        }
    </script>

</asp:Content>
