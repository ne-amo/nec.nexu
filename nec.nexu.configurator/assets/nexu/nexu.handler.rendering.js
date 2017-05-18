var handler_Rendering = (function ($, window, kendo, undefined) {

    function _getModels() {
        return (window._nexu_) ? window._nexu_ : window._nec_.nexu.models;
    }
    function _getUrl(){
        return (window.SVCURL) ? window.SVCURL : window._nexu_.nexu.config.SVCURL;
    }

    function prepareRegions(items) {
        var ret = $.map(items, function (o, i) {
            var r = {
                Region: {
                    Id: o.Region.Id,
                    Code: o.Region.Code
                },
                Material: {
                    Id: o.Material.Id,
                    Hex: o.Material.Hex,
                    BlendMode: o.Material.BlendMode,
                    Opacity: o.Material.Opacity
                }
            }
            if (o.Region.CanHavePattern && o.Material.Pattern)
                r.Material.Pattern = { Id: o.Material.Pattern.Id };
            return r;
        });
        return ret;
    };
    function prepareDesigns(items) {
        var ret = [];
        for (var i = 0; i < items.length; i++) {
            var a = items[i]
            if (!a.DesignColorway) {
                continue;
            }
            var fd = { Location: { Id: a.Location.Id } };
            var cw = {};
            cw.Design = {
                Id: a.DesignColorway.Design.Id,
                WidthPixel: a.DesignColorway.Design.WidthPixel,
                HeightPixel: a.DesignColorway.Design.HeightPixel
            };
            cw.Layers = $.map(a.DesignColorway.Layers, function (o) {
                return {
                    ImageDataId: o.ImageDataId,
                    Code: o.Code,
                    Color: (o.Color != null) ? {
                        Id: o.Color.Id,
                        Hex: o.Color.Hex,
                        BlendMode: o.Color.BlendMode,
                        Opacity: o.Color.Opacity
                    } : null
                };
            });
            fd.DesignColorway = cw;
            ret.push(fd);
        }
        return ret;
    };
    function prepareFinishedProduct() {
        var _nexu_ = _getModels()
        var dto = {};
        var fp = _nexu_.Product.toJSON();
        var templateId = _nexu_.Template.get('Id');
        dto.Template = { Id: templateId };
        dto.Regions = prepareRegions(fp.Regions);
        dto.Designs = prepareDesigns(fp.Logos);
        return dto;
    }
    function sendViews(productId) {
        var _nexu_ = _getModels();
        var views = $.map(_nexu_.Template.Views, function (obj) {
            if (!obj.IsDefault)
                return obj.Id
        }).join(',');
        return $.ajax({
            type: 'GET',
            url: _getUrl() + "FinishedProducts/" + productId +"/render/"+views,
            contentType: "application/json",
            dataType: "json"
        });
    }
    var apiUrl = _getUrl() + "finishedproducts/";
    return {
        GetUrl: function (viewId,download) {
            var h = prepareFinishedProduct();
            var s = kendo.stringify(h)
            var b64 = btoa(s);
            return apiUrl + ((download) ? 'download/' : 'view/') + viewId + "/?p=" + b64;
        },
        Download: function () {

        },
        SendViews: function (productId) {
            return sendViews(productId);
        }
    };
})(jQuery, window, kendo);