var handler_Save = (function ($, window, kendo, _, undefined) {
    var LOADER = nexu_loader, FUNCTION = typeof (function () { }),
        orderUrl = window.SHOP, LEVELS={WARN:1, FAIL:2, PASS:3};

    function prepareRegions(items) {
        var ret = $.map(items, function (o, i) {
            return {
                Region: { Id: o.Region.Id },
                Material: { Id: o.Material.Id }
            }
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
            cw.Design = { Id: a.DesignColorway.Design.Id };
            cw.Layers = $.map(a.DesignColorway.Layers, function (o) {
                return {
                    ImageDataId: o.ImageDataId,
                    Color: (o.Color != null) ? { Id: o.Color.Id } : null
                };
            });
            fd.DesignColorway = cw;
            ret.push(fd);
        }
        return ret;
    };
    function prepareFinishedProduct() {
        var dto = {};
        var fp = _nexu_.Product.toJSON();
        var templateId = _nexu_.Template.get('Id');
        var hierarchyId = _nexu_.Selected.get('Hierarchy.Id');
        var backgroundId = _nexu_.Selected.get('Background.Id');
        dto.MaterialId = fp.MaterialId;
        dto.DisplayName = fp.DisplayName;
        dto.Creator = (fp.Creator.Id) ? fp.Creator : { Id: 1 };
        dto.Template = { Id: templateId };
        dto.Hierarchy = { Id: hierarchyId };
        dto.BackgroundImage = {Id: _nexu_.Background.Current.Id};
        dto.Published = _nexu_.Published;
        dto.Public = _nexu_.Public;
        dto.Keywords = _nexu_.Keywords.data().toJSON();
        dto.Regions = prepareRegions(fp.Regions);
        dto.Designs = prepareDesigns(fp.Logos);
        if (_nexu_.Product.Id && _nexu_.Mode == _nexu_.Modes.EDIT)
            dto.Id = fp.Id;
        return dto;
    }
    function getMaterialId() {
        return $.ajax({
                type: 'GET',
                url: window.SVCURL+'MaterialId',
                contentType: "application/json",
                dataType: "json",
                success: function (msg) {
                    _nexu_.Product.MaterialId = msg;
                }
        });
    }

    function invalidPrimaryLogo(logos) {
        var primaries = $.map(logos, function (o) { if (o.Location.IsPrimary) { return o; } });
        var fails = true;
        for (var i = 0; i < primaries.length; i++) {
            /* if primary location has a colorway, pass validation */
            if (primaries[i].DesignColorway) {
                fails = false;
                break;
            }
        }
        return fails;
    }

    function validate() {
        var ret = {Level:LEVELS.PASS, items:[]};
        for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
            if (_nexu_.Product.Regions[i].UserAssigned)
                continue;
            else {
                ret.Level = LEVELS.WARN;
                ret.items.push({ Level: LEVELS.WARN, Message: _nexu_.Product.Regions[i].Region.DisplayName + ' has not been customized. The default color will be used.' });
            }
        }
        if (invalidPrimaryLogo(_nexu_.Product.Logos)) {
            ret.Level = LEVELS.FAIL
            ret.items.push({ Message: 'A required logo has not been selected.' });
        }
        return ret;
    }
    function isNew(){
        return _nexu_.Product.Id;
    }
    function isUnordered(){
        return _nexu_.Product.MaterialId;
    }
    function formatOrderUrl() {
        return orderUrl.format(_nexu_.Product.MaterialId, _nexu_.Template.StyleCode);
    }
    function _getType() {
        if (_nexu_.Product.Id && _nexu_.Mode == _nexu_.Modes.EDIT)
            return 'PUT';
        else
            return 'POST'
    }
    function _getUrl() {
        if (_nexu_.Product.Id && _nexu_.Mode == _nexu_.Modes.EDIT)
            return window.SVCURL + "FinishedProducts/" + _nexu_.Product.Id;
        else
            return window.SVCURL + "FinishedProducts/";
    }
    function _defaultSuccess(msg) {

    }
    function _defaultError() {
        module_prompt.open({
            title: "Oops!",
            message: '<div>Something went wrong.....</div>',
            showDecline: false
        });
    }

    function prepAndSend(completeFunction) {
        var dto = prepareFinishedProduct(),
            type = _getType(),
            url = _getUrl();
        
        //$.get('/nexu.token').done(function (token) {
            LOADER.onResolve(function () {
                return $.ajax({
                    type: type,
                    url: url,
                    contentType: "application/json",
                    dataType: "json",
                    //beforeSend:function(xhr){
                    //    if(token)
                    //        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    //},
                    data: kendo.stringify(dto),
                    success: _defaultSuccess,
                    error: _defaultError
                })
                .then(function (data) {
                    _nexu_.Product.Id = data.Id;
                    module_prompt.open({
                        title: "Saved",
                        message: '<div>Creating Images...</div>',
                        showDecline: false,
                        showConfirm: false
                    });
                    return handler_Rendering.SendViews(data.Id);
                })
            }(), 'close', {
                modal: true,
                open: function () {
                    module_prompt.open({
                        title: "Progress",
                        message: '<div>Saving your cap...</div>',
                        showDecline: false,
                        showConfirm: false
                    })
                },
                close: function () {
                    var id = _nexu_.Product.Id;
                    if (typeof completeFunction === FUNCTION) {
                        completeFunction.call();
                    }
                    else {
                        module_prompt.open({
                            title: "Success",
                            message: '<div>Your cap number is: ' + id + '. <a href="/nexu/product-detail/?id=' + id + '">Click here to view.</a></div>',
                            showDecline: false
                        })
                    }


                }
            })
        //});
    }

    function namePrompt(order) {
        var promptHtml = $('#nexu-save-prompt').html()
        module_prompt.open({
            title: "Save Your Cap",
            message: promptHtml,
            confirm: function () {
                var displayname = $("#inp_ProductName", module_prompt.element()).val();
                _nexu_.Product.DisplayName = displayname;
                var callback;
                if (order) {
                    callback = function () {
                        var h = formatOrderUrl();
                        window.location.href = h;
                    };
                }
                prepAndSend(callback);
            }
        });
    }

    function validateHandle(opts) {

        var options = $.extend({}, {
            order: false,

            pass: function (){},
            warn: module_prompt.open,
            fail: module_prompt.open
        }, opts);


        var validator = validate();
        if (validator.Level == LEVELS.PASS) {

            options.pass.call(this, options.order);
        }
        else if (validator.Level == LEVELS.WARN) {
            var html = '';
            for (var i = 0; i < validator.items.length; i++) {
                html += '<p>' + validator.items[i].Message + '</p>';
            }
            options.warn.call(this, { message: html });
        }
        else {
            var html = '';
            for (var i = 0; i < validator.items.length; i++) {
                html += '<p>' + validator.items[i].Message + '</p>';
            }
            options.fail.call(this, {
                title: "Sorry",
                message: html,
                showDecline: false
            });
        }
    }

    function saveHandle(options) {
        var opts = $.extend({}, {
            pass: function (o) {
                namePrompt(o.order);
            },
            warn: function (o) {
                module_prompt.open({
                    title: "Attention!",
                    message: o.message,
                    showDecline: false,
                    confirm: function () {
                        prepAndSend();
                    }
                });
            }
        }, options);
        validateHandle(opts);
    }

    function publishHandle(options) {
        var opts = $.extend({}, {
            pass: function (o) {
                var callback;
                if (o.order) {
                    callback = function () {
                        var h = formatOrderUrl();
                        window.location.href = h;
                    };
                }
                prepAndSend(callback);
            },
            warn: function (o) {
                module_prompt.open({
                    title: "Attention!",
                    message: o.message,
                    showDecline: false,
                    confirm: function () {
                        prepAndSend();
                    }
                });
            }
        }, options);
        validateHandle(opts);
    }


    return {
        CLICK: function (e) {
            /*  Save click */
            //_click({ order: false });
            saveHandle({ order: false });
            return false;
        },
        Save: function () {
            /*  publish click */
            publishHandle({ order: false });
            //_click({ publish: true });

        },
        Order: function (e) {
            /*  place order click */
            getMaterialId().done(
                saveHandle({ order: true })
                //_click({ order: true })
            );
            return false;
        }
    };
})(jQuery, window, kendo, window._);
