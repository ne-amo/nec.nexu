;nexu_loader = (function ($,kendo,window,undefined) {

    var html = '<div id="nexu-loader"><img src="/assets/img/loading.gif" alt="Loading" title="Loading" />Loading...</div>',
        element = '#nexu-loader',
        container = '.nx-loader-container',
        modal = '.nx-loader-overlay',
        IN = 'in',
        OUT = 'out',
        FUNCTION = typeof(function(){}),
        defaults = {
            modal: false,
            bindToGlobalAjax: false,
            close: function () { },
            open: function () { }
        }
    function getContainer() {
        return $(container);
    }
    function getElement() {
        return $(element);
    }
    function addElement() {
        return $(html).appendTo(getContainer());
    }
    function open(f) {
        var l = getElement().show();
        kendo.fx(l).zoom(IN).play().done(function () {
            if (typeof f === FUNCTION)
                f.call();
        });
    }
    function close(f) {
        $.when(kendo.fx(getElement()).zoom(OUT).play()).done(function () {
            if (typeof f === FUNCTION)
                f.call();
        });
    }
    function showModal(){
        $(modal).show();
    }
    function closeModal() {
        $(modal).hide();
    }
    function handleOptions(options) {
        if (options.modal) {
            showModal();
        }
        if (options.bindToAjax) {
        
        }
    }
    addElement();
    close();
    return {
        Bind:function(options){

        },
        Show: function (options) {
            var opts = $.extend({}, defaults, options);
            open(opts.f);
        },
        Hide: function (options) {
            var opts = $.extend({}, defaults, options);
            close(opts.f);
        },
        onResolve: function (promise,result,options) {
            var opts = $.extend({}, defaults, options);
            handleOptions(opts);
            if (result == 'open') {
                close(opts.close);
                promise.always(open, opts.callback);
            }
            if (result == 'close') {
                open(opts.open);
                if (opts.modal) {
                    promise.always(close, opts.close, closeModal);
                }
                else {
                    promise.always(close, opts.close);
                }
            }
        }
    }
})(jQuery,window.kendo,window);