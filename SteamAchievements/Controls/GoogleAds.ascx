<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GoogleAds.ascx.cs"
    Inherits="SteamAchievements.Controls.GoogleAds" %>

<script type="text/javascript">
<!--
    google_ad_client = "<%= AdClient %>";
    google_ad_slot = "<%= AdSlot %>";
    google_ad_width = parseInt("<%= Width %>");
    google_ad_height = parseInt("<%= Height %>");
//-->
</script>

<script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>

