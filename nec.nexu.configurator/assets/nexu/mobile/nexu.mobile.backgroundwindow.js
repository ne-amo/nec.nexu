; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.config = nec.nexu.config || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.backgroundwindow = nec.nexu.mobile.controls.backgroundwindow || {};

    var n = nec.nexu.mobile.controls.backgroundwindow, kDs = kendo.data.DataSource;

    var templateHtml = '<li class="background-selector">' +
               '<div style="width:100%;"><div class="background-tile" style="background-image:url(\'' + nec.nexu.config.ASSETS + '#: data.ThumbnailPath #\')" title="#: data.DisplayName #" ></div></div></li>';

    var defaults = {
        data:[],
        container: '.mobile-background-window',
        list: '.mobile-background-list',
        template: kendo.template(templateHtml, { useWithBlock: false }),
        change: $.noop,
        close: $.noop
    }
    function _makeTemplate(obj) {
        return (typeof obj === 'string') ? kendo.template(obj) : obj;
    }

    function _init(options) {
        var opts = $.extend({}, defaults, options);
        var el = $(opts.container);
        n.container = el;

        n.list = $(opts.list, el).kendoListView({
            dataSource: kDs.create(opts.data),
            selectable: true,
            template: _makeTemplate(opts.template),
            change: opts.change
        }).data('kendoListView');

        n.window = el.kendoWindow({
            title: '',//'Choose Background',
            resizable: false,
            draggable: false,
            visible: false,
            activate: function (e) {
                e.sender.fullScreen()
            },
            close:opts.close
        }).data('kendoWindow').changeButton('.k-window-action',
			'<span role="presentation" class="glyphicon glyphicon-remove k-i-close"></span>');
    }
    function _open(options) {
        var opts = $.extend({data:[]}, options);
        n.window.open();
    }

    n.init = _init;
    n.open = _open;
    window._nec_ = nec;

})(window.jQuery, window.kendo, window._, window);