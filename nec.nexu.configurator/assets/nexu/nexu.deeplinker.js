;var module_DeepLink = (function ($,kendo,_,window,undefined) {
    var SVC = window.SVCURL,
        kLV = 'kendoListView';

    function _getLogoObject(data) {
        return $.map(_nexu_.Product.Logos, function (logo) {
            if (data.DesignLocations.indexOf(logo.Location.Id) > -1) {
                return logo;
            }
        })[0];
    }
    function _handleColorList(data) {
        var mod = _nexu_.ColorPicker.UI.elMenu.find('.menu'),
        /* if specific ProductRegionGroup is supplied assign that group, otherwise use the default (first),
            since different templates have different groups, we have to find one that exists in the supplied array.  */
            group = (data.ProductRegionGroups) ?
            _.filter(_nexu_.Template.RegionGroups, function (g) {
                return data.ProductRegionGroups.indexOf(g.Id) > -1;
            })[0] : _nexu_.Template.RegionGroups[0],
            ds = mod.data('kendoListView').dataSource,
            listItem = mod.children().eq(ds.indexOf(_.findWhere(ds.data(), {
                Id: data.ColorListId
            })));
            if (group)
                _nexu_.Selected.set('Menu', group);
            if (listItem)
                mod.data('kendoListView').select(listItem);
    }
    function _handleMaterials(data) {
        var _n = _nexu_;
        var mod = _n.ColorPicker.UI.elMenu.find('.menu'),
            /* if specific ProductRegionGroup is supplied assign that group, otherwise use the default (first),
                since different templates have different groups, we have to find one that exists in the supplied array.  */
            group = (data.ProductRegionGroups) ?
                _.filter(_n.Template.RegionGroups, function (g) {
                    return data.ProductRegionGroups.indexOf(g.Id) > -1;
                })[0] : _n.Template.RegionGroups[0];
            list = {
                DisplayName: data.DisplayName, Colors: data.Materials
            },
            ds = mod.data(kLV).dataSource;
            ds.add(list);
            var listItem = mod.children().eq(ds.data().length - 1);
            if (group)
                _n.Selected.set('Menu', group);
            if (listItem)
                mod.data(kLV).select(listItem);
    }
    function _handleSingleDesign(data) {
        /* for a specific location */
        if (data.DesignLocations) {
            var L = _getLogoObject(data);
            if (L) {
                function setLocation(e) {
                    _nexu_.Selected.set('Location', L.Location);
                    _nexu_.Selected.set('Logo', L);
                    _nexu_.View.Controller.Widget().clearLogo(location.Code);
                }
                _nexu_.Selected.set('Logo.DesignColorway', data.Designs[0].DefaultColorway);
                L.set('DesignColorway', data.Designs[0].DefaultColorway);
            }
        }
    }
    function _handleMultipleDesigns(data) {
        /* for a specific location */
        if (data.DesignLocations) {
            var L = _getLogoObject(data);
            if (L) {
                /* its possible that the location has a preferred view, so we have to check for that and load the view if necessary */
                /* the deferred allows us to call our direct link action after the view is loaded */
                var def = $.Deferred();
                def.done(function () {
                    /* get the appearence attributes from the view */
                    var o = _nexu_.View.Controller.Widget().getLogoOffset(L.Location.Code);
                    scale = (o.matrix) ? Math.floor(parseFloat(o.matrix.split(',')[0]) * 100) : 1,
                    args = {
                        //dataSource: data.Designs,
                        centerX: o.left,
                        centerY: o.top,
                        imageScale: scale
                    }
                    /* build designs datasource */
                    var designs = $.map(data.Designs, function (d) {
                        return $.extend({},d,
                            {
                                OfficialImage: d.OfficialImage.FilePath,
                                ThumbnailImage: d.OfficialImage.ThumbnailPath,
                                DefaultColorwayId: (d.DefaultColorway) ? d.DefaultColorway.Id : null,
                                OfficialColorwayId: (d.OfficialColorway) ? d.OfficialColorway.Id : null,
                            });
                    });
                    /* set viewstate and call logo carousel */
                    _nexu_.Selected.set('Location', L.Location);
                    _nexu_.Selected.set('Logo', L);
                    _nexu_.LogoCarousel.SetData(designs);
                    _nexu_.LogoCarousel.Open(args);
                });
                if (L.Location.TemplateViewId) {
                    /* load the view and resolve the deferred when its drawn */
                    $(_nexu_.View.Controller.Widget().element).one('drawn', function (e) {
                        def.resolve();
                    });
                    var v = _.findWhere(_nexu_.View.Views, { Id: L.Location.TemplateViewId });
                    _nexu_.Selected.set('View', L.Location.TemplateViewId);
                    _nexu_.View.Controller.ChangeView(v);
                }
                else {
                    /* no need to change view, resolve the deferred and continue */
                    def.resolve();
                }
            }
        }
    }
    function _handleHierarchy(data) {
        if (data.DesignLocations) {
            var L = _getLogoObject(data);
            if (L) {
                var def = $.Deferred();
                def.done(function () {
                    _nexu_.Selected.set('Location', L.Location);
                    _nexu_.Selected.set('Logo', L);
                    _nexu_.Selected.set('Hierarchy', { Id: data.DesignHierarchyId } );
                    _nexu_.LogoCarousel.call();
                });
                if (L.Location.TemplateViewId && L.Location.TemplateViewId != _nexu_.Selected.View) {
                    $(_nexu_.View.Controller.Widget().element).one('drawn', function (e) {
                        def.resolve();
                    });
                    var v = _.findWhere(_nexu_.View.Views, { Id: L.Location.TemplateViewId });
                    _nexu_.Selected.set('View', L.Location.TemplateViewId);
                    _nexu_.View.Controller.ChangeView(v);
                }
                else {
                    /* no need to change view, resolve the deferred and continue */
                    def.resolve();
                }

            }
        }
    }

    function _parse(data, deffered) {
        if (!data)
            return;
        /* Loading a specific product for the user to customize */
        if (data.ProductId) {
        }
        /* Loading a specific colorlist for a specific region group */
        if (data.ColorListId) {
            _handleColorList(data);
        }
        /* Loading specific materials */
        if (data.Materials.length) {// && data.ProductRegionGroups) {
            _handleMaterials(data);
        }
        /* Loading a specific design  */
        if (data.Designs.length && data.Designs.length == 1) {
            _handleSingleDesign(data);
        }
        /* Loading a group of designs */
        if (data.Designs.length && data.Designs.length > 1) {
            _handleMultipleDesigns(data);
        }
        /* Loading a hierarchy for a specific location */
        if (data.DesignHierarchyId && data.DesignLocations) {
            _handleHierarchy(data);
        }
    }

    function _ajax(id, deffered) {

        var url = SVC + "ConfiguratorDirectLinks/"+ "uid/" + id;
        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                _parse(msg);
            },
            error: function () {

            }
            , complete: function () { console.log('DirectLink Load Complete'); }
        });


    }

    return {
        Process: function (id) {
            if (id)
                return _ajax(id);
            else
                return $.Deferred().resolve();
        },
		Load : function (id) {
            var __this = this;
            $.ajax({
                type: "GET",
                url: SVCURL + "GetDirectLink",
                data: { id: id },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg && msg.d) {
                        var obj = $.parseJSON(msg.d);
                        __this.Parse(obj);
                    }
                }
            });
        }
	}
})(jQuery, window.kendo, _, window);