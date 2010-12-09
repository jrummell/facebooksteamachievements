<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="Settings.aspx.cs" Inherits="SteamAchievements.Settings" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
    <h1>
        Settings</h1>
    <div id="saveSuccess" class="fbinfobox message" style="display: none;">
        Your settings have been saved successfully.</div>
    <fieldset>
        <legend>Steam Community Custom URL</legend>
        <div>
            <p>
                http://steamcommunity.com/id/
                <asp:TextBox ID="steamIdTextBox" runat="server" />
                <asp:RequiredFieldValidator ID="steamIdRequiredValidator" runat="server" Display="Dynamic"
                    ControlToValidate="steamIdTextBox" ErrorMessage="Required" />
                <steam:HelpLink ID="steamHelpLink" runat="server" HelpAnchor="steam" />
            </p>
        </div>
    </fieldset>
    <fieldset>
        <legend>Auto Update</legend>
        <asp:CheckBox ID="autoUpdateCheckBox" runat="server" />
        <label for="autoUpdateCheckBox">
            Update and publish my achievements automatically (Beta).</label>
    </fieldset>
    <p>
        <asp:LinkButton ID="saveSettingsButton" runat="server" OnClick="SaveSettingsButtonClick">
            <asp:Image ID="steamUserIdHelpImage" runat="server" ImageUrl="~/images/disk.png"
                AlternateText="Save" ImageAlign="Middle" />
            Save</asp:LinkButton>
        <img id="saveImage" class="loading" src="images/ajax-loader.gif" alt="Saving..."
            style="display: none" />
    </p>
    </form>
    <script type="text/javascript">
        $(document).ready(function ()
        {
            $achievements.init();

            $("#saveSettingsButton").click(function ()
            {
                if (Page_ClientValidate())
                {
                    $("#saveImage").show();
                }
            });
        });
    </script>
    <asp:PlaceHolder ID="saveSuccessScript" runat="server" Visible="false">
        <script type="text/javascript">
            $(document).ready(function ()
            {
                $achievements.showMessage("#saveSuccess");
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>
