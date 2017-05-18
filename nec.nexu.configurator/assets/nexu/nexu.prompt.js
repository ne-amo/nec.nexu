; var module_prompt = (function ($, kendo, _, window,undefined) {
    var el = $('#nexu-prompt'),
		msgArea = $('.nexu-prompt-message',el),
		btnConfirm = $('.nexu-prompt-confirm',el),
		btnDecline = $('.nexu-prompt-decline',el);
    var win = el.kendoWindow({
        modal:true,
        animation:false,
        resizable:false,
        draggable: false,
        visible:false,
        open: function (e) {
            e.sender.center();
            e.sender.element.prev().addClass('nexu-prompt-title');
        }
    }).data('kendoWindow');
    defaults = {
        showConfirm: true,
        confirmText: 'Confirm',
        showDecline: true,
        declineText: 'Cancel',
        title: 'Confirm'
    }
    var defaultAction = function(){
        win.close();
        return false;
    };
    win.close();


    function _open(opts) {
        /*if (!options)
            return;*/
        _change(opts);

        return win.open();
    }
    function _close() {
        return win.close();
    }
    function _change(opts) {
        var options = $.extend({}, defaults, opts);
        btnConfirm.off('click');
        btnDecline.off('click');
        if (options.title)
            win.title(options.title);
        else
            win.title("Confirm");
        if (options.message) {
            msgArea.html(options.message);
        }
        else {
            msgArea.empty();
        }

        if (!options.showConfirm) {
            btnConfirm.hide();
        }
        else {
            btnConfirm.on('click', defaultAction).show().text(options.confirmText);
            if (options.confirm && _.isFunction(options.confirm)) {
                btnConfirm.on('click', options.confirm);
            }
        }

        if (!options.showDecline) {
            btnDecline.hide();
        }
        else {
            btnDecline.on('click', defaultAction).show().text(options.declineText);
            if (options.decline && _.isFunction(options.decline)) {
                btnDecline.on('click', options.decline);
            }

        }
    }

    return {
        element:function(){
            return $('#nexu-prompt');
        },
        open:function(options)
        {
            return _open(options);
        },
        close: function () {
            return _close();
        },
        change: function (options) {
            return _change(options);
        }
    };
}(jQuery,kendo,_,window));