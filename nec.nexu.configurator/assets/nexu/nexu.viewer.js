;var module_viewer = function(){
    var divId = '#nexu-configurator-viewer',
        context = /*(window.isMobile) ? '#nexu-ui-mobile' :*/ '#nexu-configurator',
        LOADER = nexu_loader,
        defaultsAjax = {
            type: "GET",
            url: window.SVCURL + "TemplateViews/",
            contentType: "application/json; charset=utf-8",
            dataType: "json"
            ,complete: function () { console.log('Viewer Load Complete'); }
        };


    function widget(){
        return $(divId, context).data('kendoNexuView');
    }
    function setMobile() {

        var h = document.documentElement.clientHeight;
        var w = (document.documentElement.clientWidth - 40);
        var wid = widget();
        if (wid) {
            wid.options.maxWidth = w*.8;
            wid.options.maxHeight = h - 80;
            wid.resize({ width: w * .8 - 180, height: h - 80 });
        }

    }
    function setDesktop() {

        var wid = widget();
        if (wid) {
            wid.options.maxWidth = 0;
            wid.options.maxHeight = 400;
            /* Need to reset the SVG to default view */
            //wid.resize({ width: w - 180, height: h - 50 });
        }
    }
    function _initViewer(view) {
        $(divId, context).kendoNexuView({
            product: _nexu_.Product,
            view: view,
        }).on('regionSelected', $.proxy(_nexu_.Handlers.Viewer, _nexu_.Product));
    }
    function _ajaxLoad(id, options) {
        var a = $.extend({}, defaultsAjax,{ url:defaultsAjax.url+id }, options);
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
        $.extend(_.findWhere(_nexu_.View.Views, { Id: view.Id }), view);
        _nexu_.Selected.View = view.Id;
        // Pass the view message to the viewer.
        _initViewer(view);
        return widget();
    }
    function successChange (msg) {
        if (!msg && !msg)
            return;
        var view = msg,
            viewer = widget();
        view.Current = true;
        // Merge the returned view message into the model's view array.
        $.extend(_.findWhere(_nexu_.View.Views, { Id: view.Id }), view);
        viewer.changeView(view, _nexu_.Selected.Menu)
            .done(function () {
                //console.log("successChange, changeView");
                /* Re-enable the view carousel */
                _nexu_.View.Carousel.Enable();
            });
    }


    return{
        LoadViews: function (obj) {
            var _this=module_viewer;
            // Validate
            if (!obj)
                return false;
            obj = obj;
            if (!obj.Template)
                return false;
            if (!obj.Template.Views && obj.Template.Views.length == 0)
                return false;
            /* Get ID of the first view to load */
            var id = _.sortBy(obj.Template.Views, function (x) { return x.SortOrder; })[0].Id;
            // Store the views to model for binding to UI
            _nexu_.View.Views = kendo.observable(obj.Template.Views);
            // Return deferred
            return _ajaxLoad(id, { success: successLoad });
        },
        ChangeView: function (view) {
                var v = view,
                    _this = module_viewer,
                    viewer = widget();
                if (!v || v.Current)
                    return false;
				
					
                var loaded,
					/* IE workaround. IE doesn't have outerHTML property on SVG element */
                    svg = viewer.getSvg().parentNode.innerHTML, /*original :viewer.getSvg().outerHTML, */
						/* Get the current SVG content, to be stored back into the array. 
                        This helps with maintaining the visual consistency when the view is loaded back in */
                    current = _.findWhere(_nexu_.View.Views, { Current: true });
					
                current.set('Current',false);
                current.set('SvgContent',btoa(svg)); /* convert to base64 just to maintain pattern with data return from server */
                loaded = _.findWhere(_nexu_.View.Views, { Id: parseInt(v.Id) });
                /* if the view has already been loaded, then we can pass it to the viewer widget, otherwise need to make an ajax call */
                if (loaded.SvgContent) {
                    loaded.set('Current',true);
                    viewer.changeView(loaded, _nexu_.Selected.Menu)
                        .done(function () {
                            /* Re-enable the view carousel */
                            _nexu_.View.Carousel.Enable();
                        });
                }
                else {
                    LOADER.onResolve(
                        _ajaxLoad(v.Id, { success: successChange }),
                        'close');
                }
                return false;
        },
        InitViewer: function (view) {
            _initViewer(view);
        },
        Widget:function(){
            return widget();
        },
        goMobile: function () {
            setMobile();
        },
        goDesktop: function () {
            setDesktop();
        }
    }
}();