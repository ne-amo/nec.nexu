; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.notification = nec.nexu.mobile.controls.notification || {};

    var n = nec.nexu.mobile.controls.notification, CLICK = 'click';

    var defaultCloseButtonText = $('.notify-close').html(), defaultConfirmButtonText = $('.notify-confirm').html(), defaultDeclineButtonText = $('.notify-decline').html();

    function _getElement() {
        return $('.mobile-notification');
    }

    var defaults = {
        message: "Notification",
        showConfirm: false,
        showDecline: false,
        showClose: true,
        showExclamation: true,
        isModal: false,
        cssClass: "centered",
        confirmText: null,
        declineText: null,
        closeText: null,
        confirmCallback: $.noop,
        declineCallback: $.noop,
        closeCallback: $.noop
    }

    function _showElement(jq) {
        var el = jq || _getElement();
        el
        .find('.notify-modal').show()
        .parent(el)
        .find('.mobile-notification-bar').slideDown('fast');
    }
    function _hideElement(jq) {
        var el = jq || _getElement();
        el
        .find('.notify-modal').hide()
        .parent(el)
        .find('.mobile-notification-bar').slideUp('fast');
    }

    function _init() {
        var el = _getElement();
        //,h = $('html').attr('nx-height');//el.closest('.mobile-container').height();
        //$('.notify-modal', el).height(h);
        _hideElement(el);
    }

    n.init = _init;
    n.show = function (options) {
        var opts = $.extend({}, defaults, options), el = _getElement();

        if (opts.message instanceof jQuery) {
            $('.notify-message', el).empty().append(opts.message);
        }
        else {
            $('.notify-message', el).html(opts.message);
        }

        $('.notify-close,.notify-confirm,.notify-decline', el).hide();

        el.find(".notify-message").removeClass("col-xs-11").removeClass("col-xs-12")

        if (opts.showExclamation)
        {
            el.find(".message .exclamation").show();
            el.find(".notify-message").addClass("col-xs-11");
        }
        else {

            el.find(".message .exclamation").hide();
            el.find(".notify-message").addClass("col-xs-12");
        }

        el
        .removeClass().addClass("mobile-notification").addClass(opts.cssClass)
        .find('.mobile-notification-bar').removeClass().addClass("mobile-notification-bar").addClass(opts.cssClass);

        $('.notify-decline').html(opts.declineText || defaultDeclineButtonText);
        $('.notify-confirm').html(opts.confirmText || defaultConfirmButtonText);
        $('.notify-close').html(opts.closeText || defaultCloseButtonText);


        if (opts.showConfirm || opts.showDecline) {
            if (opts.showDecline) {
                $('.notify-decline', el).show();
            }
            if (opts.showConfirm) {
                $('.notify-confirm', el).show();
            }
        }
        else {
            $('.notify-close', el).show();
        }


        el.off(CLICK)
            .on(CLICK, '.notify-confirm', function (e) {
                _hideElement();
                opts.confirmCallback.call(e);
                return false;
            })
            .on(CLICK, '.notify-decline', function (e) {
                _hideElement();
                opts.declineCallback.call(e);
                return false;
            })
            .on(CLICK, '.notify-close', function (e) {
                _hideElement();
                opts.closeCallback.call(e);
                return false;
            });

        _showElement(el);
    }
    n.close = function (options) {
        var opts = $.extend({}, defaults, options), el = _getElement();
        el.off(CLICK);
        _hideElement(el)
        opts.closeCallback.call(e);
    }

    window._nec_ = nec;

})(jQuery, kendo, _, window);