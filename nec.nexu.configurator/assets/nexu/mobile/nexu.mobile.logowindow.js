; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.logowindow = nec.nexu.mobile.controls.logowindow || {};

    var n = nec.nexu.mobile.controls.logowindow, kDs = kendo.data.DataSource;

    var templateHtml = '<li>' +
               '<div>' +
               '<img style="max-width:100%;" src="#: data.OfficialImage #?h=132" title="#: data.DisplayName #" />'+
               '</div>' +
               '</li>';

    var defaults = {
        data:[],
        container: '.mobile-logo-window',
        list: '.mobile-logo-list',
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

        //console.log("logos _init");
        //console.log(opts.data);

        n.list = $(opts.list, el).kendoListView({
            dataSource: opts.data,
            selectable: true,
            template: _makeTemplate(opts.template),
            change: opts.change
        }).data('kendoListView');

        //n.window = el.kendoWindow({
        //    title: 'Choose Logo',
        //    resizable: false,
        //    visible: false,
        //    draggable: false,
        //    activate: function (e) {
        //        e.sender.fullScreen();
        //    },
        //    close:opts.close
        //}).data('kendoWindow').changeButton('.k-window-action',
		//	'<span role="presentation" class="glyphicon glyphicon-remove k-i-close"></span>');
    }

    //this is when the logo images are displayed
    function _open(options) {
        var opts = $.extend({ data: [] }, options);
        n.list.setDataSource(kDs.create(opts.data));
        //console.log("logos _open");
        //console.log(opts.data);

        //n.window.open();

        //make sure that logo window is open
        //create function to populate color from dataset; for now testing with this
        $("#mobile-color-selector").removeClass("show");
        $("#mobile-logo-selector").addClass("show");
    }
    n.init = _init;
    n.open = _open;

    window._nec_ = nec;

})(window.jQuery, window.kendo, window._, window);