; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.summary = nec.nexu.mobile.controls.summary || {};
    nec.nexu.models = nec.nexu.models || {};

    var n = nec.nexu.mobile.controls.summary, models = nec.nexu.models, kDs = kendo.data.DataSource;

    var regionTemplateHtml = '<li><div class="nexu-colorgrabber-swatch" style="background-color:\\##: data.Material.Hex #;">&nbsp;</div>' +
        '#: data.Region.DisplayName # - #: data.Material.DisplayName # #if (data.Region.ShowFabricContent) {# ' +
        '#: data.Material.FabricContent.DisplayName # #}#</li>',

        logoTemplateHtml = '<li>#: data.Location.DisplayName #</li>'

    var defaults = {
        container: '.mobile-summary-window',
        regionContainer: '.mobile-summary-regions',
        logoContainer: '.mobile-summary-logos',
        regionTemplate: kendo.template(regionTemplateHtml, { useWithBlock: false }),
        logoTemplate: kendo.template(logoTemplateHtml, { useWithBlock: false })
    }
    function _makeTemplate(obj) {
        return (typeof obj === 'string') ? kendo.template(obj) : obj;
    }



    function _init(options) {
        var opts = $.extend({}, defaults, options);
        var el = $(opts.container);
        n.container = el;
        n.regionList = $(opts.regionContainer, el).kendoListView({
            dataSource: [],
            selectable:false,
            template: _makeTemplate(opts.regionTemplate)
        }).data('kendoListView');
        n.logoList = $(opts.logoContainer, el).kendoListView({
            dataSource: [],
            selectable: false,
            template: _makeTemplate(opts.logoTemplate)
        }).data('kendoListView');
        n.window = el.kendoWindow({
            title: 'Product Summary',
            resizable: false,
            visible:false,
            activate: function (e) {
                e.sender.fullScreen();
            }
        }).data('kendoWindow').changeButton('.k-window-action',
			'<span role="presentation" class="glyphicon glyphicon-remove k-i-close"></span>');
    }
    function _open(options) {

        var opts = $.extend({ regions: [], logos: [] }, options);

        n.regionList.setDataSource(kDs.create(opts.regions));
        n.logoList.setDataSource(kDs.create(opts.logos));
        n.window.open();
    }
    n.init = _init;
    n.open = _open;


    window._nec_ = nec;

})(jQuery, kendo, _, window);