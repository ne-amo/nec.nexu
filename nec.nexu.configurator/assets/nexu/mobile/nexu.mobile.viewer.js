/* Mobile Viewer */
; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.mobile.viewer = nec.nexu.mobile.viewer || {};
    nec.nexu.models = nec.nexu.models || {};
    nec.nexu.ui = nec.nexu.ui || {};
    nec.nexu.data = nec.nexu.data || {};
    nec.nexu.config = nec.nexu.config || {};
    nec.nexu.events = nec.nexu.events || {};
    nec.nexu.events = nec.nexu.events || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.menu = nec.nexu.mobile.controls.menu || {};

    var viewer = nec.nexu.mobile.viewer,
		data = nec.nexu.data,
		config = nec.nexu.config,
		mobile = nec.nexu.mobile,
		models = nec.nexu.models,
		ui = nec.nexu.ui,
		events = nec.nexu.events;
    menu = nec.nexu.mobile.controls.menu;

    var divId = '#nexu-configurator-viewer',
        context = '#nexu-configurator',
		selector = $('.mobile-view-selector'),
		heightBuffer = 48,
        defaultsAjax = {
            type: "GET",
            url: config.SVCURL + "TemplateViews/",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
            , complete: function () {

                mobile.setImageScale();
                console.log('Viewer Load Complete');
            }
        };

    function widget() {
        return $(divId).data('kendoNexuView');
    }
    function getViewerHeight() {

        return window.innerHeight * .8
    }

    function _initViewer(view) {
        var g = $(divId);
        g.kendoNexuView({
            product: models.Product,
            view: view,
            autoCenter: false,
            autoBind: true,
            maxWidth: g.parent().width(),
            maxHeight: getViewerHeight(),
            allowMultiSelect: true, /////TODO
        }).on('regionSelected', $.proxy(viewer, models.Product))
        .on('regionSelected', $.proxy(viewer.setInMenu));
    }

    function _ajaxLoad(id, options) {
        var a = $.extend({}, defaultsAjax, { url: defaultsAjax.url + id }, options);
        return $.ajax(a);
    }

    function successLoad(msg) {
        if (!msg)
            return;
        // Parse JSON response
        var view = msg;
        // Set object to active.
        view.Current = true;
        // Merge the returned view message into the model's view array.
        $.extend(_.findWhere(ui.Views, { Id: view.Id }), view);
        ui.Selected.View = view.Id;
        // Pass the view message to the viewer.
        _initViewer(view);
        return widget();
    }
    function successChange(msg) {
        console.log('sucessChange');
        if (!msg && !msg)
            return;
        var view = msg,
            w = widget();
        view.Current = true;
        // Merge the returned view message into the model's view array.
        $.extend(_.findWhere(ui.Views, { Id: view.Id }), view);
        w.changeView(view, ui.Selected.Menu) // Have to pass in the -1 for Logos or enable the regions
            .done(function () {
                /* Re-enable the view carousel */
                //_nexu_.View.Carousel.Enable();
                //console.log('changeView.done');
            });

    }
    viewer.selectRegion = function (regionCode) {
        widget().selectRegion(region);
    }

    viewer.scale = function (options) {
        widget().resize(options);
    }

    viewer.loadViews = function (obj) {
        var _this = viewer;
        // Validate
        if (!obj)
            return false;
        obj = obj;
        if (!obj.Template)
            return false;
        if (!obj.Template.Views && obj.Template.Views.length == 0)
            return false;

        ui.Views = obj.Template.Views;

        var def = _.sortBy(ui.Views, function (x) { return x.SortOrder; })[0];
        def.Current = true;
        return _ajaxLoad(def.Id, { success: successLoad });
    }

    viewer.setInMenu = function (e, clickedRegion) {
        //show color menu if hidden
        if (!menu.isColorSelectorMenuVisible()) {
            menu.resetView();
            menu.showColorSelectorMenus();
        }

        //go set clicked region in treeview
        var key = clickedRegion.key;
        menu.selectRegionFromKey(key);

    }
    var _changing = false;
    viewer.changeView = function (view) {
        //console.log("viewer.changeView");

        if (_changing) {
            return false;
        }
        else {
            _changing = true;
            //set a timer to allow switching just in case something happens
            setTimeout("_changing = false;", 5000);
        }

        //occasionally clicking to fast can mark multiple views as "current" - this is to clean that up

        if (_.where(ui.Views, { Current: true }).length > 1) {
            for (var i = 0; i < ui.Views.length; i++) {
                ui.Views[i].Current = false;
            }
            view.Current = false;
        }

        var v = view,
            _this = viewer,
            w = widget();
        if (!v || v.Current) {
            _changing = false;
            return false;
        }


        var loaded,
            /* IE workaround. IE doesn't have outerHTML property on SVG element */
            svg = w.getSvg().parentNode.innerHTML, /*original :viewer.getSvg().outerHTML, */
                /* Get the current SVG content, to be stored back into the array. 
                This helps with maintaining the visual consistency when the view is loaded back in */
            current = _.findWhere(ui.Views, { Current: true });

        if (current != undefined) {
            current.Current = false;
            current.SvgContent = btoa(svg); /* convert to base64 just to maintain pattern with data return from server */
        }

        loaded = _.findWhere(ui.Views, { Id: parseInt(v.Id) });
        /* if the view has already been loaded, then we can pass it to the viewer widget, otherwise need to make an ajax call */
        if (loaded.SvgContent) {
            console.log("loaded.SvgContent");
            loaded.Current = true;
            w.changeView(loaded)
                .done(function () {
                    //mobile.setImageScale();
                    _changing = false;
                });
        }
        else {
            //LOADER.onResolve(
            mobile.loader.show()
            _ajaxLoad(v.Id, { success: successChange }).done(function () {

                //mobile.setImageScale();
                _changing = false;
                mobile.loader.hide();
            });//,
            //	'close');

        }
        return false;
    }

    viewer.initViewer = _initViewer;
    viewer.widget = widget;

    window._nec_ = nec;
})(jQuery, kendo, _, window);