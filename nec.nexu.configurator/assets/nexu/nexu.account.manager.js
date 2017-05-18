;controller_user = (function ($, kendo, window, undefined) {

    var svc = "http://localhost:50595/",
    CLICK = 'click',
    CLIENT = 'testapplication',
    ACCESS_ID = 'nx-access',
    REFRESH_ID = 'nx-refresh',
    REFRESH_EXPIRE = 'nx-expires',
    LOGIN_GRANT = { grant_type: 'password', client_id: CLIENT },
    REFRESH_GRANT = { grant_type: 'refresh_token', client_id: CLIENT },
    TIMEOUT = (30 * 60) * 1000,
    ajaxBasic = {
        type: "POST",
        url: svc + "Token"
    },
    ui = {},
    html = '<div id="nexu-user-panel" class="nexu-user-panel"> <div class="nx-user-section nexu-login"> <div class="nx-lg-row nexu-login-label">Don\'t have an account? <a href="#" id="nx-btnShowReg">Create one.</a></div> <div class="nx-lg-row nexu-login-fail">Login failed. <span class="nx-login-message"></span></div> <div class="nx-lg-row nexu-login-label">E-mail</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-login-inpEmail" validationMessage="E-mail required." type="email" data-email-msg="Email format is not valid"  /></div> <div class="nx-lg-row nexu-login-label">Password</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-login-inpPassword" validationMessage="Password required." type="password" /></div> <div class="nx-lg-row nexu-login-bottom"> <p><a href="#" id="lnkForgotPassword">Forgot your password?</a></p> <p><input type="checkbox" id="nx-login-chKeep" checked="checked" /> Keep me logged in.</p> <p><img class="nx-loading" src="/assets/img/loading.gif" /> <a href="#" id="nx-btnLogin" class="k-button" style="font-size:1.25em;">LOG IN</a></p> </div> </div> <div class="nx-user-section nexu-register"> <div class="nx-lg-row nexu-login-label">Already have an account? <a href="#" id="nx-btnShowLog">Login.</a></div> <div class="nx-lg-row nexu-login-label">E-mail</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-inp-Register-Email" name="nx-email" validationMessage="E-mail required." required  type="email" data-email-msg="Email format is not valid" /></div> <div class="nx-lg-row nexu-login-label">Name (displayed in gallery)</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-inp-Register-Name" name="nx-name"  validationMessage="Name required." required  type="text" /></div> <div class="nx-lg-row nexu-login-label">Country</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-inp-Register-Country" name="nx-country"  validationMessage="Country required."   type="text" /></div> <div class="nx-lg-row nexu-login-label">Password</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-inp-Register-Password" name="nx-pw"  validationMessage="Password required." required  type="password" /></div> <div class="nx-lg-row nexu-login-label">Confirm Password</div> <div class="nx-lg-row nexu-login-input"><input class="k-input" id="nx-inp-Register-Password-Confirm" name="nx-cpw"  validationMessage="Password required." required  type="password" /></div> <div class="nx-lg-row nexu-login-bottom"> <p><input type="checkbox" id="nx-register-chKeep" checked="checked" /> Keep me logged in.</p> <p><a href="#" id="nx-btnRegister" class="k-button" style="font-size:1.25em;">CREATE</a></p> <p>By clicking create, I agree to the <a href="#" id="lnkTermsAndConditions">Terms &amp; Conditions</a></p> </div> </div> <div class="nx-user-section nexu-logout"> <div class="nx-lg-row nexu-login-label">Hello <span class="nexu-logout-name"></span>!</div> <div class="nx-lg-row nexu-login-label">Logged in as: <span class="nexu-logout-email"></span></div> <div class="nx-lg-row nexu-login-bottom"> <p><img class="nx-loading" src="/assets/img/loading.gif" /> <a href="#" id="nx-btnLogout" class="k-button" style="font-size:1.25em;">LOG OUT</a></p> </div> </div> </div>';
    function addElement() {
        var d = document.createElement('div');
        d.innerHTML = html;
        document.documentElement.appendChild(d);
    }
    function sendLogin(params) {
        return $.ajax($.extend({}, ajaxBasic, { data: params }));
    }
    function sendLogout() {
        var url = svc + "api/Account/Logout?app=" + CLIENT;
        return $.ajax({
            type: "POST",
            url: url
        }).done(function () {
            clearStorage();
            ui_SetLogin();
        });
    }
    function sendRegistration(params, u, p) {
        return $.ajax({
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            url: svc + "api/Account/register/",
            data: params
        }).done(function (e) {
            sendLogin($.extend({}, LOGIN_GRANT, { username: u, password: p }));
        });
    }
    function getUserInfo() {
        return $.ajax({
            type: "GET",
            url: svc + "api/Account/userinfo"
        });
    }
    function canRefresh() {
        return localStorage.getItem(REFRESH_ID) != null;
    }
    function refreshAccess() {
        if (canRefresh()) {
            $.ajaxSetup({ /* for some reason sending the authentication to the token endpoint fails */
                beforeSend: function (xhr) { }
            });
            sendLogin($.extend({}, REFRESH_GRANT, { refresh_token: localStorage.getItem(REFRESH_ID) }));
        }
        else {
            /* no refresh token in storage, show login screen */
            ui_SetLogin();
        }
    }
    function setHeaders(token) {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                xhr.setRequestHeader("Authorization", "Bearer " + token.access_token);
            }
        });
    }
    function setStorage(storage, token) {
        storage.setItem(ACCESS_ID, token.access_token);
        storage.setItem(REFRESH_ID, token.refresh_token);
    }
    function clearStorage() {
        localStorage.removeItem(ACCESS_ID);
        localStorage.removeItem(REFRESH_ID);
        sessionStorage.removeItem(ACCESS_ID);
        sessionStorage.removeItem(REFRESH_ID);
    }
    function handleError(a, b, c) {
        if (a && a.responseJSON && (a.responseJSON.error_description != null || a.responseJSON.error_description != '')) {
            ui.FAIL_MESSAGE.text(a.responseJSON.error_description);
        }
        else {
            ui.FAIL_MESSAGE.text('Please verfiy e-mail and password are correct.');
        }
        ui.FAIL.show();
    }
    function handleToken(token) {
        if (!token) {
            /* could not login */
            ui_SetLogin(true);
        }
        var persist = ui.LOG_PERSIST.prop('checked') || canRefresh();
        if (persist) {
            setStorage(localStorage, token); /* store in local storage for long life */
            setInterval(refreshAccess, token.expires_in * 1000); /* refresh the token automatically after 30 minutes */
        }
        else {
            setStorage(sessionStorage, token); /* store in session storage, will be cleared when tab closes */
            setTimeout(clearStorage, TIMEOUT);
        }
        setHeaders(token);
        ui.LOG_UN.val('');
        ui.LOG_PW.val('');
        getUserInfo().done(function (data) {
            ui_SetLogout(data);
        });
    }
    $.extend(ajaxBasic, { success: handleToken, error: handleError });
    function ui_hideSections() {
        ui.SECTIONS.hide();
        ui.FAIL.hide();
        ui.LOADER.hide();
    }
    function ui_SetLogin(error) {
        ui_hideSections();
        ui.LOGIN.show();
        if (error) {
            ui.FAIL.show();
        }
    }
    function ui_SetRegister() {
        ui_hideSections();
        ui.REGISTER.show();
    }
    function ui_SetLogout(data) {
        ui_hideSections();
        if (data) {
            ui.LOGOUT_NAME.text(data.Nickname);
            ui.LOGOUT_EMAIL.text(data.Email);
        }
        ui.LOGOUT.show();
    }
    function click_btnLogin(e) {
        ui.FAIL.hide();
        ui.LOADER.show();
        var u = ui.LOG_UN.val(), p = ui.LOG_PW.val();
        sendLogin($.extend({}, LOGIN_GRANT, { username: u, password: p }));
        return false;
    }
    function click_btnLogout(e) {
        sendLogout();
        return false;
    }
    function click_btnRegister(e) {
        if (ui.VALIDATOR.validate()) {
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

    addElement();

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
        });
        ui.REG_BTN = $('#nx-btnRegister').on(CLICK, click_btnRegister);
        ui.LOGOUT = $('.nexu-logout');
        ui.LOGOUT_NAME = $('.nexu-logout-name', '.nexu-logout');
        ui.LOGOUT_EMAIL = $('.nexu-logout-email', '.nexu-logout');
        ui.LOGOUT_BTN = $('#nx-btnLogout', '.nexu-logout').on(CLICK, click_btnLogout);
        ui.SECTIONS = $('.nx-user-section');
        ui.VALIDATOR = ui.CONTAINER.kendoValidator().data("kendoValidator");
        $('.nexu-login', '#nexu-user-panel').on('keyup', 'input', function (e) {
            var key = e.which;
            if (key == 13) { click_btnLogin(); }
        })
        $('.nexu-register', '#nexu-user-panel').on('keyup', 'input', function (e) {
            var key = e.which;
            if (key == 13) { click_btnRegister(); }
        })
    })();

    ui_hideSections();

    $(function () {
        refreshAccess();
    });



    //var controller = {
    //    login: function () {

    //    },
    //    logout: function () {

    //    },
    //    getUser: function () {

    //    }
    //}
    //return controller;
})(jQuery,kendo, window);