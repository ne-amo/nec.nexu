var module_360_view = (function ($, window, kendo, _, undefined) {
    /* Setup defaults */
    var elId = "#nexu-360-view",
		btnId = "#nexu-360-view-btn",
		defaultWidth = 600,
		defaultHeight = 500,
        spinnerDefaults = {
            width: 520,
            height: 420,
            loop: false,
            animate: false,
            onLoad: function (e) {
                $('div.loading', elId).hide();
                $(e.currentTarget).fadeIn();
            }
        },
        LOADER = nexu_loader;
    /* handler to open the lightbox */
    function _open(imageArray) {
        $(elId).data('kendoWindow').open();
    }
    /* handler to close the lightbox */
    function _close() {
        $('.k-overlay').off(window.CLICK);
        $('div.loading', elId).show();
        $(elId).data('kendoWindow').unbind('open').close();
    }
    /* intialize components */
    function init() {
        $(elId).kendoWindow({
            width: defaultWidth,
            //height: defaultHeight,
            modal: true,
            resizable: false,
            draggable: false,
            visible: false,
            open: function (e) {
                e.sender.element
					.css({ 'margin-top': '-1px' })
					.closest('.k-window').css({ 'padding-top': '0', 'margin-top': '37px' })
					.find('.k-window-titlebar').addClass('nexu-360-window');


                /* kendos window doesnt play nice as a lightbox, manually bind the close trigger */
                $('.k-overlay').on(window.CLICK, _close);
                /* again, kendo window, hide toolbar to make it like a lightbox */
                //$(e.sender.element).prev('.k-window-titlebar').hide();
                e.sender.center();
            }
        }).data('kendoWindow').close();
        $('div.view-spin', elId).spritespin(
            $.extend({}, spinnerDefaults, {
                onLoad: function (e) {
                    LOADER.Hide();
                    _open();
                    $(e.currentTarget).fadeIn();
                }
            })
        );
    }
    init();
    return {
        Click: function (event) {
            /* External click handler for easy use of basic processing */
            /* Get the sorted array of views */
            var array = _.sortBy(_nexu_.Template.get('Views').toJSON(), function (obj) { return obj.SortOrder });
            /* get array of formatted urls. see nexu.handler.rendering for url generation. Here we just append the spinner width/height vars */
            var urls = $.map(array, function (o, i) {
                if (o.Include360)
                    return _nexu_.Rendering.GetUrl(o.Id) + "&n=true&w=" + 520 + "&h=" + 420;
            });

            LOADER.Show();
            var winEl = $(elId),
                spinEl = $('div.view-spin', winEl);
            spinEl.spritespin({ source: urls })

            //        /* Bind a handler to open set the spinner when lightbox is opened */
            //	    $(elId).data('kendoWindow').bind('open', function (e) {
            //            /* kendos window doesnt play nice as a lightbox, manually bind the close trigger */
            //	        $('.k-overlay').on(window.CLICK, _close);
            //            /* again, kendo window, hide toolbar to make it like a lightbox */
            //	        $(e.sender.element).prev('.k-window-titlebar').hide();
            //	        e.sender.center();
            //	        var winEl = e.sender.element,
            //                spinEl = $('div.view-spin', winEl);
            //	        spinEl.hide();
            //            /* set the spinner */
            //	        spinEl.spritespin({source:urls});
            //	    });




            /* open window */
            //_open(urls);
            return false;
        },
        Open: function (images) {
            _open(images);
        },
        Close: function () {
            _close();
        }
    }
})(jQuery, window, kendo, _);