/* Kendo Prompt Widget 
 * Inherits from Kendo Window
 * 09/29/2014
 * Aaron Ortwein
*/
;(function ($) {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
        WINBASE = ui.Window,
        KWINDOWCONTENT = ".k-window-content",
        CLICK = 'click',
        CONFIRM = 'confirm',
        DECLINE = 'decline',
        _class = 'k-Prompt',
        buttons = '<div class="k-PromptButtons"><a href="#" class="k-button" data-role="confirm">Ok</a><a href="#" class="k-button" data-role="decline">Cancel</a></div>';
    var Prompt = WINBASE.extend({
        init: function (element, options) {
            var that = this;
            WINBASE.fn.init.call(this, element, options);
            that.element.closest('.k-window').addClass(_class);
        },
        options: {
            name: "Prompt",
            title: 'Confirm Action',
            modal: true,
            minWidth: 280,
            resizable: false,
            open: function (e) {
                e.sender.center();
            }
        },
        events: [CONFIRM, DECLINE],
        __unbind:function(){
            this.element.off(CLICK, 'a.k-button[data-role="confirm"],a.k-button[data-role="decline"]');
            return this;
        },
        __close : function (callback) {
            this.__unbind().close();
            if (callback && typeof callback === 'function')
                callback();
            return false;
        },
        open: function (args) {
            var that = this;
            if (args){
                that.element.html('<div>' + args.message + '</div>' + buttons);
                that.__unbind();
                that.element.on(CLICK, 'a.k-button[data-role="confirm"]', $.proxy(that.__close,that,args.confirm));
                that.element.on(CLICK, 'a.k-button[data-role="decline"]', $.proxy(that.__close,that,args.decline));
            }
            WINBASE.prototype.open();
        }
    });
    ui.plugin(Prompt);

    ui.Window.fn._object = function (element) {
        var content = element.children(KWINDOWCONTENT);
        return content.data("kendoWindow") || content.data("kendoPrompt") || content.data("kendo" + this.options.name);
    };

}(jQuery));
