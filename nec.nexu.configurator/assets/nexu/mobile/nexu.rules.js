; (function ($, kendo, _, window, undefined) {

    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.data = nec.nexu.data || {};
    nec.nexu.data.rules = nec.nexu.data.rules || {};
    nec.nexu.ui = nec.nexu.ui || {};
    nec.nexu.config = nec.nexu.config || {};

    var config = nec.nexu.config, rules = nec.nexu.data.rules;

    /* Helper methods to deal with original global object before namespaced structure */
    function _getGlobal() {
        if (window._nexu_)
            return window._nexu_;
        else
            return nec.nexu;
    }
    function _getModel() {
        if (window._nexu_)
            return window._nexu_;
        else
            return nec.nexu.models;
    }
    function _getUi() {
        if (window._nexu_)
            return window._nexu_;
        else
            return nec.nexu.ui;
    }

    var defaults_ajax_ColorwayCheck = {
        type: "POST",
        url: config.SVCURL + "designs/checkcolorway/",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    };

    function _helper_FailDifferentiation() {
        /* see if there are rules */
        if (!_getUi().ColorwayRules)
            return false;
        var rule = _getUi().ColorwayRules.ColorDifferentiationRules[0];
        if (!rule)
            return false;
        /* get the unique number of colors in colorway */
        var colorway = $.map(_getElement_LayerList().data('kendoGrid').dataSource.data().toJSON(), function (l) {
            if (l.Recolorable)
                return l.Color.Id;
        });
        /* if it fails, return the rule */
        return (_.uniq(colorway).length < rule.RequiredColors) ? rule : false;
    }

    function _helper_FailColorway() {
        var obj = _getUi().Selected.Logo.DesignColorway,
            colorway = {
                Layers: $.map(obj.Layers, function (layer) {
                    if (layer.Recolorable && layer.Color)
                        return {
                            ImageDataId: layer.ImageDataId,
                            Color: {
                                Id: layer.Color.Id
                            }
                        }
                })
            },
            call = $.extend({},
                defaults_ajax_ColorwayCheck,
                {
                    url: defaults_ajax_ColorwayCheck.url + obj.Design.Id,
                    data: kendo.stringify(colorway)
                }
            );
        return $.ajax(call)
    }

    function _helper_FindLayerRule(dataId) {
        if (!_getUi().ColorwayRules)
            return null;
        var rule = _.findWhere(_getUi().ColorwayRules.DesignColorRules, { ImageDataId: dataId });
        if (!rule)
            return null;
        return {
            logic: rule.Combination.toLowerCase(),
            filters: $.map(rule.Values, function (obj, ind) {
                return { field: 'Id', operator: obj.Operator, value: obj.DesignColorId };
            })
        }
    }

    function _parseLayerRulesForDataSource(source, layer) {
        var rule = _helper_FindLayerRule(layer.ImageDataId);
        if (rule) {
            var filter = {
                logic: rule.logic,
                filters: rule.filters
            }
            /* These rules can be an inclusion or exclusion. They are stored on the model in proper Kendo format */
            source.filter(filter);
        } else {
            /* No rules, clear the filters. */
            source.filter(null);
        }
    }

    /* utility method */
    /* Determines if the the primary logo location has been used on the cap. */
    function _IsPrimaryLocationNeeded() {
        if (!nec.nexu.models.Product ||
            !nec.nexu.models.Template ||
            !nec.nexu.models.Product.Logos
        ) // Cannot find data to process
            return true;
        var locationIds = $.map(nec.nexu.models.Template.DesignLocations, function (o, i) {
            if (o.IsPrimary)
                return o.Id;
        });
        var needed = true;
        for (var i = 0; i < nec.nexu.models.Product.Logos.length; i++) {
            if (locationIds.indexOf(nec.nexu.models.Product.Logos[i].Location.Id) > -1) {
                if (nec.nexu.models.Product.Logos[i].DesignColorway) {
                    needed = false;
                    break;
                }
            }
        }
        return needed;
    }

    /* utility method */
    /* Determines if the the primary logo location has been used on the cap. */
    function _isPrimaryLocationNeeded(model) {
        if (!model ||
            !model.Product ||
            !model.Template ||
            !model.Product.Logos
        ) // Cannot find data to process
            return true;
        var locationIds = $.map(model.Template.DesignLocations, function (o, i) {
            if (o.IsPrimary)
                return o.Id;
        });
        var needed = true;
        for (var i = 0; i < model.Product.Logos.length; i++) {
            if (locationIds.indexOf(model.Product.Logos[i].Location.Id) > -1) {
                if (model.Product.Logos[i].DesignColorway) {
                    needed = false;
                    break;
                }
            }
        }
        return needed;
    }

    rules.isPrimaryLocationNeeded = _isPrimaryLocationNeeded;
    rules.parseLayersForSource = _parseLayerRulesForDataSource;
    rules.findLayerRule = _helper_FindLayerRule;
    rules.failColorway = _helper_FailColorway;
    rules.failDifferentiation = _helper_FailDifferentiation;

    window._nec_ = nec;

})(jQuery, kendo, _, window);