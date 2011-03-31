<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="Settings.aspx.cs" Inherits="SteamAchievements.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <form runat="server">
    <h1>
        Settings</h1>
    <div id="saveSuccess" class="fbinfobox message" style="display: none;">
        Your settings have been saved successfully.</div>
    <div id="duplicateError" class="fberrorbox message" style="display: none;">
        This Steam Community URL is already in use.</div>
    <fieldset>
        <legend>Steam Community Custom URL</legend>
        <div>
            <p>
                http://steamcommunity.com/id/
                <asp:TextBox ID="steamIdTextBox" runat="server" MaxLength="50" />
                <asp:RequiredFieldValidator ID="steamIdRequiredValidator" runat="server" Display="Dynamic"
                    ControlToValidate="steamIdTextBox" ErrorMessage="Required" />
                <steam:HelpLink ID="steamHelpLink" runat="server" HelpAnchor="Configure_your_Steam_Community_Profile" />
            </p>
        </div>
    </fieldset>
    <fieldset>
        <legend>Publish Options</legend>
        <div>
            <asp:CheckBox ID="autoUpdateCheckBox" runat="server" />
            <label for="autoUpdateCheckBox">
                Update and publish my achievements automatically.</label>
        </div>
        <div>
            <asp:CheckBox ID="publishDescriptionCheckBox" runat="server" />
            <label for="publishDescriptionCheckBox">
                Include achievement descriptions when publishing.</label>
        </div>
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
    <asp:PlaceHolder ID="duplicateErrorScript" runat="server" Visible="false">
        <script type="text/javascript">
            $(document).ready(function ()
            {
                $achievements.showMessage("#duplicateError");
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>
