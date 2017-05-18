<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="nec.nexu.configurator.Login" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>New Era By You Login</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="/assets/css/reset.css" rel="stylesheet" type="text/css" />
    <link href="/assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/kendo/styles/kendo.common-bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/kendo/styles/kendo.flat.min.css" rel="stylesheet" type="text/css" />
    <link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
    <link href="/assets/css/nexu.css" rel="stylesheet" type="text/css" />
    <link href="/assets/css/nexu.mobile.css" rel="stylesheet" />
</head>
<body>
    <form id="nx_login_form" runat="server" novalidate>
    <style type="text/css">
        .absoluteHidden {
            display: none !important;
        }
        .default-hide {
            display: none;
        }
        #nexu-user-panel {
            margin: auto;
        }
    </style>
    <asp:HiddenField ID="nx_hfnexu" runat="server" />
    <div id="nexu-user-panel" class="nexu-user-panel">
        <div class="nexu-error-msg"><asp:Label ID="lblError" runat="server" /></div>
        <div class="nx-user-section nexu-login">
            <div class="nx-lg-row nexu-login-label">Don't have an account? <a href="#" id="nx-btnShowReg">Create one.</a></div>
            <div class="nx-lg-row nexu-login-fail">Login failed. <span class="nx-login-message"></span></div>
            <div class="nx-lg-row nexu-login-label">E-mail</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_login_email" runat="server" />
                <input class="k-input" id="nx-login-inpEmail" name="nx_log_email" validationmessage="E-mail required." type="email" required data-email-msg="Email format is not valid" /></div>
            <div class="nx-lg-row nexu-login-label">Password</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_login_password" runat="server"  />
                <input class="k-input" id="nx-login-inpPassword" name="nx_log_pw" validationmessage="Password required." required type="password" /></div>
            <div class="nx-lg-row nexu-login-bottom">
                <p><a href="#" id="lnkForgotPassword">Forgot your password?</a></p>
                <p>
                    <asp:HiddenField ID="nx_login_persist" runat="server" Value="true" />
                    <input type="checkbox" id="nx-login-chKeep" checked="checked" />
                    Keep me logged in.</p>
                <p>
                    <asp:Button ID="nx_btn_login" runat="server" OnClick="nx_click_login" Text="Login" CssClass="absoluteHidden" />
                    <img class="nx-loading default-hide" src="/assets/img/loading.gif" height="24" />
                    <a href="#" id="nx-btnLogin" class="k-button" style="font-size: 1.25em;">LOG IN</a></p>
            </div>
        </div>
        <div class="nx-user-section nexu-register default-hide">
            <div class="nx-lg-row nexu-login-label">Already have an account? <a href="#" id="nx-btnShowLog">Login.</a></div>
            <div class="nx-lg-row nexu-login-label">E-mail</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_register_email" runat="server" />
                <input class="k-input" id="nx-inp-Register-Email" name="nx_email" validationmessage="E-mail required." required type="email" data-email-msg="Email format is not valid" /></div>
            <div class="nx-lg-row nexu-login-label">Name (displayed in gallery)</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_register_name" runat="server" />
                <input class="k-input" id="nx-inp-Register-Name" name="nx_name" validationmessage="Name required." required type="text" /></div>
            <div class="nx-lg-row nexu-login-label">Country</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_register_country" runat="server" />
                <input class="k-input" id="nx-inp-Register-Country" name="nx_country" validationmessage="Country required." type="text" /></div>
            <div class="nx-lg-row nexu-login-label">Password</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_register_pw" runat="server" />
                <input class="k-input" id="nx-inp-Register-Password" name="nx_pw" validationmessage="Password required." required type="password" /></div>
            <div class="nx-lg-row nexu-login-label">Confirm Password</div>
            <div class="nx-lg-row nexu-login-input">
                <asp:HiddenField ID="nx_register_cpw" runat="server" />
                <input class="k-input" id="nx-inp-Register-Password-Confirm" name="nx-cpw" validationmessage="Password required." required type="password" /></div>
            <div class="nx-lg-row nexu-login-bottom">
                <p>
                    <asp:HiddenField ID="nx_register_persist" runat="server" Value="true" />
                    <input type="checkbox" id="nx-register-chKeep" checked="checked" />
                    Keep me logged in.</p>
                <p><asp:Button ID="nx_btn_register" runat="server" Visible="false" OnClick="nx_click_register" />
                    <a href="#" id="nx-btnRegister" class="k-button" style="font-size: 1.25em;">CREATE</a></p>
                <p>By clicking create, I agree to the <a href="#" id="lnkTermsAndConditions">Terms &amp; Conditions</a></p>
            </div>
        </div>
        <div class="nx-user-section nexu-logout default-hide">
            <div class="nx-lg-row nexu-login-label">Hello <span class="nexu-logout-name"></span>!</div>
            <div class="nx-lg-row nexu-login-label">Logged in as: <span class="nexu-logout-email"></span></div>
            <div class="nx-lg-row nexu-login-bottom">
                <p><asp:Button ID="nx_btn_logout" runat="server" Text="LOGOUT" CssClass="absoluteHidden" OnClick="nx_click_logout" />
                    <img class="nx-loading" src="/assets/img/loading.gif" />
                    <a href="#" id="nx-btnLogout" class="k-button" style="font-size: 1.25em;">LOG OUT</a></p>
            </div>
        </div>
    </div>




    </form>

    <link href="/assets/css/nexu.css" rel="stylesheet" />
    <link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
    <link href="/assets/css/nexu.mobile.css" rel="stylesheet" />

    <script src="/assets/js/jquery-2.1.0.min.js"></script>
    <script src="/assets/kendo/js/kendo.all.min.js"></script>
    <script type="text/javascript">


        var ui = {}, CLICK = 'click';
        var svc = "http://localhost:50595/";
        var hiddens = {
            TOKEN: $('#<%= nx_hfnexu.ClientID%>'),
            LOG_EMAIL: $('#<%= nx_login_email.ClientID%>'),
            LOG_PASSWORD: $('#<%= nx_login_password.ClientID%>'),
            REG_EMAIL: $('#<%= nx_register_email.ClientID%>'),
            REG_NAME: $('#<%= nx_register_name.ClientID%>'),
            REG_COUNTRY: $('#<%= nx_register_country.ClientID%>'),
            REG_PASSWORD: $('#<%= nx_register_pw.ClientID%>'),
            REG_CONF_PASS: $('#<%= nx_register_cpw.ClientID%>'),
            LOG_PERSIST: $('#<%= nx_login_persist.ClientID%>'),
            REG_PERSIST: $('#<%= nx_register_persist.ClientID%>'),
            LOG_SUBMIT: function(){
                $('#<%= nx_btn_login.ClientID %>').click();
            },
            REG_SUBMIT:function(){
                __doPostBack('<%= nx_btn_register.UniqueID %>','')
            },
            OUT_SUBMIT: function () {
                $('#<%= nx_btn_logout.ClientID %>').click();
            }
        };

        function ui_hideSections() {
            ui.SECTIONS.hide();
            ui.FAIL.hide();
            ui.LOADER.hide();
        }
        function ui_SetLogin(error) {
            ui_hideSections();
            ui.LOGIN.show();
            setOuterWindow();
            if (error) {
                ui.FAIL.show();
            }
        }
        function ui_SetRegister() {
            ui_hideSections();
            ui.REGISTER.show();
            setOuterWindow();
        }
        function ui_SetLogout(data) {
            ui_hideSections();
            if (data) {
                ui.LOGOUT_NAME.text(data.Nickname);
                ui.LOGOUT_EMAIL.text(data.Email);
            }
            ui.LOGOUT.show();
            setOuterWindow();
        }
        function click_btnLogin(e) {
            ui.FAIL.hide();
            ui.LOADER.show();
            if (ui.VALIDATOR_LOGIN.validate()) {
                var u = ui.LOG_UN.val(), p = ui.LOG_PW.val();
                hiddens.LOG_EMAIL.val(u);
                hiddens.LOG_PASSWORD.val(p);
                hiddens.LOG_SUBMIT();
            }
            return false;
        }
        function click_btnLogout(e) {
            hiddens.OUT_SUBMIT();
            return false;
        }
        function click_btnRegister(e) {
            if (ui.VALIDATOR_REG.validate()) {
                var email = ui.REG_EMAIL.val();
                var name = ui.REG_NAME.val();
                var country = ui.REG_COUNTRY.val();
                var pw = ui.REG_PW.val();
                var confirm_pw = ui.REG_CPW.val();
                if (pw == confirm_pw) {
                    var dto = {
                        Nickname: name,
                        Email: email,
                        Password: pw,
                        ConfirmPassword: confirm_pw
                    };
                    ui.LOADER.show();
                    sendRegistration(kendo.stringify(dto), email, pw);
                }
                else {

                }
            }
            return false;
        }
        function click_btnShowRegr(e) {
            ui_SetRegister();
            return false;
        }
        function click_btnShowLog(e) {
            ui_SetLogin();
            return false;
        }
        function setOuterWindow() {
            var win = window.parent.$("#nx-login-window").data("kendoWindow");
            if (!win)
                return;
            if ($('#nx_login_form').hasClass('mobile')) {
                win.maximize();
            }
            else {
                var bWidth = document.documentElement.offsetWidth;
                var bHeight = document.documentElement.offsetHeight;
                if (bWidth == 0 || bHeight == 0)
                    return;
                win.setOptions({
                    width: bWidth,
                    height: bHeight + 40
                });
                win.center();
            }
        }


        (function setElements() {
            ui.CONTAINER = $('.nexu-user-panel');
            ui.LOADER = $('.nx-loading');
            ui.WELCOME = $('.nexu-logout-name');
            ui.FAIL = $('.nexu-login-fail');
            ui.FAIL_MESSAGE = $('.nx-login-message', '.nexu-login-fail');
            ui.LOGIN = $('.nexu-login');
            ui.LOG_PW = $('#nx-login-inpPassword');
            ui.LOG_UN = $('#nx-login-inpEmail');
            ui.LOG_PERSIST = $('#nx-login-chKeep').on('change', function (e) {
                var isChecked = $(this).prop('checked');
                ui.REG_PERSIST.prop('checked', isChecked);
                hiddens.LOG_PERSIST.val(isChecked.toString());
                hiddens.REG_PERSIST.val(isChecked.toString());
            });
            ui.LOG_BTN = $('#nx-btnLogin').on(CLICK, click_btnLogin)
            ui.LOG_SHOW = $('#nx-btnShowLog').on(CLICK, click_btnShowLog);
            ui.REGISTER = $('.nexu-register');
            ui.REG_SHOW = $('#nx-btnShowReg').on(CLICK, click_btnShowRegr);
            ui.REG_EMAIL = $("#nx-inp-Register-Email");
            ui.REG_NAME = $("#nx-inp-Register-Name");
            ui.REG_COUNTRY = $("#nx-inp-Register-Country");
            ui.REG_PW = $("#nx-inp-Register-Password");
            ui.REG_CPW = $("#nx-inp-Register-Password-Confirm");
            ui.REG_PERSIST = $("#nx-register-chKeep").on('change', function (e) {
                var isChecked = $(this).prop('checked');
                ui.LOG_PERSIST.prop('checked', isChecked);
                hiddens.LOG_PERSIST.val(isChecked.toString());
                hiddens.REG_PERSIST.val(isChecked.toString());
            });
            ui.REG_BTN = $('#nx-btnRegister').on(CLICK, click_btnRegister);
            ui.LOGOUT = $('.nexu-logout');
            ui.LOGOUT_NAME = $('.nexu-logout-name', '.nexu-logout');
            ui.LOGOUT_EMAIL = $('.nexu-logout-email', '.nexu-logout');
            ui.LOGOUT_BTN = $('#nx-btnLogout', '.nexu-logout').on(CLICK, click_btnLogout);
            ui.SECTIONS = $('.nx-user-section');
            ui.VALIDATOR_REG = ui.REGISTER.kendoValidator().data("kendoValidator");
            ui.VALIDATOR_LOGIN = ui.LOGIN.kendoValidator().data("kendoValidator");

            $('.nexu-login', '#nexu-user-panel').on('keyup', 'input', function (e) {
                var key = e.which;
                if (key == 13) { click_btnLogin(); }
            });
            $('.nexu-register', '#nexu-user-panel').on('keyup', 'input', function (e) {
                var key = e.which;
                if (key == 13) { click_btnRegister(); }
            });
            $('.default-hide').removeClass('default-hide');
            setOuterWindow();
        })();
        ui_hideSections();
        if (hiddens.TOKEN.val() != '') {
            $.ajax({
                    type: "GET",
                    url: svc + "api/Account/userinfo",
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + hiddens.TOKEN.val());
                    }
                }).done(function (data) {
                    ui_SetLogout(data);
                });
        }
        else {
            ui_SetLogin();
            
        }
        if (document.documentElement.clientWidth < 768) {
            $('#nx_login_form').addClass('mobile');
        }
    </script>

</body>

</html>
