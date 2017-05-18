<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="nec.nexu.configurator.Test"  %>

<%@ Register Src="~/layouts/LaunchSitecore/Controls/NExU/Configurator.ascx" TagPrefix="uc1" TagName="Configurator" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="nx_hfnexu" runat="server" />
    </form>
    <div class="container">
        <div>
            <uc1:Configurator runat="server" id="Configurator" />
        </div>

    </div>
</body>
</html>
