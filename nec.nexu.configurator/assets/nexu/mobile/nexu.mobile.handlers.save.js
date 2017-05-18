; (function (window, undefined) {
    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.models = nec.nexu.models || {};
    nec.nexu.ui = nec.nexu.ui || {};
    nec.nexu.data = nec.nexu.data || {};
    nec.nexu.mobile.handlers = nec.nexu.mobile.handlers || {};

    var config = nec.nexu.config,
        ui = nec.nexu.ui,
        handlers = nec.nexu.mobile.handlers,
        models = nec.nexu.models,
        notify = nec.nexu.mobile.controls.notification,
        LEVELS = { WARN: 1, FAIL: 2, PASS: 3 },
        LOADER = nexu_loader,
        FUNCTION = typeof (function () { });

    function _getModels() {
        return nec.nexu.models;
    }
    function _getSelected() {
        return nec.nexu.ui.Selected;
    }


    (function () {

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
            var m = _getModels();
            var dto = {};
            var fp = m.Product.toJSON();
            var templateId = m.Template.get('Id');



            var hierarchyId = m.SelectedHierarchy.get('Id');
            var backgroundId = ui.Background.Id;
            dto.MaterialId = fp.MaterialId;
            dto.DisplayName = fp.DisplayName;
            dto.Creator = (fp.Creator.Id) ? fp.Creator : { Id: 1 };
            dto.Template = { Id: templateId };
            dto.Hierarchy = { Id: hierarchyId };
            dto.BackgroundImage = { Id: ui.Background.Id };
            dto.Published = ui.Published;
            dto.Public = ui.Public;
            dto.Keywords = ui.Keywords.data().toJSON();
            dto.Regions = prepareRegions(fp.Regions);
            dto.Designs = prepareDesigns(fp.Logos);
            if (m.Product.Id && ui.Mode == ui.Modes.EDIT)
                dto.Id = fp.Id;
            return dto;
        }
        function getMaterialId() {
            //console.log("getMaterialId");
            return $.ajax({
                type: 'GET',
                url: config.SVCURL + 'MaterialId',
                contentType: "application/json",
                dataType: "json",
                success: function (msg) {
                    _getModels().Product.MaterialId = msg;
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
            var _nexu_ = _getModels();
            var ret = { Level: LEVELS.PASS, items: [], errors: [] };

            //console.log("product", _nexu_.Product);
            var msgtemplate = "<div class=\"row details-row\">" +
    "<div class=\"col-xs-6 region-color-details\">" +
        //"<div class=\"row\">" +
        "<div class=\"-col-xs-12 region-name\">{0}</div>" +
    "</div>" +
    "<div class='col-xs-6 color-details'>" +
        "<div class=\"color-swatch\" style=\"background-color: #{1}\">&nbsp;</div>" +
        "<div class=\"-col-xs-12 color-swatch-name\" >{2}</div>" +
    "</div>" +
    "<div class=\"col-xs-12 message-block\">" +
        //"<div class=\"row\">" +
            "<div class=\"-col-xs-12 {3}\">{4}{5}</div>" +
        //"</div>" +
    "</div>" +
"</div>";



            for (var i = 0; i < _nexu_.Product.Regions.length; i++) {

                if (_nexu_.Product.Regions[i].UserAssigned) {

                    ret.Level = LEVELS.PASS;
                    ret.items.push({
                        Level: LEVELS.WARN,
                        Id: _nexu_.Product.Regions[i].Region.Code,
                        Message: String.format(
                            msgtemplate,
                            $.trim(_nexu_.Product.Regions[i].Region.DisplayName),
                            $.trim(_nexu_.Product.Regions[i].Material.Hex),
                            $.trim(_nexu_.Product.Regions[i].Material.DisplayName),
                            "pass",
                            "",
                            "")
                    });
                }
                else {
                    ret.Level = LEVELS.WARN;
                    ret.items.push({
                        Level: LEVELS.WARN,
                        Id: _nexu_.Product.Regions[i].Region.Code,
                        Message: String.format(
                            msgtemplate,
                            $.trim(_nexu_.Product.Regions[i].Region.DisplayName),
                            $.trim(_nexu_.Product.Regions[i].Material.Hex),
                            $.trim(_nexu_.Product.Regions[i].Material.DisplayName),
                            "warn",
                            "WARNING: ",
                            _nexu_.Product.Regions[i].Region.DisplayName + " has not been customized. The default color will be used.")
                    });
                }
            }

           


            if (invalidPrimaryLogo(_nexu_.Product.Logos)) {
                ret.Level = LEVELS.FAIL
                var err = { Message: 'A required logo has not been selected.' };
                ret.items.push(err);
                ret.errors.push(err);
            }
            else {
                msgtemplate = "<div class=\"row details-row\">" +
               "<div class=\"col-xs-6 region-logo-details\">" +
                   "<div class=\"region-name\">{0}</div>" +
               "</div>" +
               "<div class='col-xs-6 logo-details'>" +
                   "{1}" +
               "</div>" +
           "</div>";
                for (var i = 0; i < _nexu_.Product.Logos.length; i++) {
                    var l = _nexu_.Product.Logos[i];


                    if (l.DesignColorway != null) {
                        var imagePath = typeof (l.DesignColorway.Design.OfficialImage) == "object" ? l.DesignColorway.Design.OfficialImage.FilePath : l.DesignColorway.Design.OfficialImage;
                        ret.items.push({
                            Id: l.Location.Id,
                            Message: String.format(
                                msgtemplate,
                                $.trim(l.Location.DisplayName),

                                String.format("<img src='{0}?h={1}' />", imagePath, 75)
                                )
                        });
                    }
                }

            }

            return ret;
        }
        function isNew() {
            return _getModels().Product.Id;
        }
        function isUnordered() {
            return _getModels().Product.MaterialId;
        }
        function formatOrderUrl() {
            var _nexu_ = _getModels();
            return orderUrl.format(_nexu_.Product.MaterialId, _nexu_.Template.StyleCode);
        }
        function _getType() {
            if (_getModels().Product.Id && ui.Mode == ui.Modes.EDIT)
                return 'PUT';
            else
                return 'POST'
        }
        function _getUrl() {
            var m = _getModels();
            if (m.Product.Id && ui.Mode == ui.Modes.EDIT)
                return config.SVCURL + "FinishedProducts/" + m.Product.Id;
            else
                return config.SVCURL + "FinishedProducts/";
        }
        function _defaultSuccess(msg) {

        }
        function _defaultError() {
            notify.show({
                message: 'Something went wrong.....',
                showDecline: false
            });
        }

        function prepAndSend(completeFunction) {
            var dto = prepareFinishedProduct(),
                type = _getType(),
                url = _getUrl();
            LOADER.onResolve(function () {
                return $.ajax({
                    type: type,
                    url: url,
                    contentType: "application/json",
                    dataType: "json",
                    data: kendo.stringify(dto),
                    success: _defaultSuccess,
                    error: _defaultError
                })
                .then(function (data) {
                    _getModels().Product.Id = data.Id;

                    notify.show({
                        message: 'Creating Images...',
                        showClose: false,
                        showConfirm: false,
                        showDecline: false
                    });
                    return handler_Rendering.SendViews(data.Id);
                })
            }(), 'close', {
                modal: true,
                open: function () {
                    notify.show({
                        message: '<div>Saving your cap...</div>',
                        showDecline: false,
                        showConfirm: false
                    })
                },
                close: function () {
                    var id = _getModels().Product.Id;
                    if (typeof completeFunction === FUNCTION) {
                        completeFunction.call();
                    }
                    else {
                        notify.show({
                            message: '<div>Your cap number is: ' + id + '. <a href="/nexu/product-detail/?id=' + id + '">Click here to view.</a></div>',
                            showDecline: false
                        })
                    }


                }
            })




        }

        function namePrompt(show) {
            if (show)
                $('.mobile-save-name').slideDown();
            else
                $('.mobile-save-name').hide();
        }

        function validateHandle(opts) {

            var options = $.extend({}, {
                order: false,

                pass: function () { },
                warn: notify.show,
                fail: notify.show
            }, opts);

            var validator = validate();

            var summaryImg = "";
            for (var i = 0; i < models.Template.Views.length; i++) {
                var viewId = models.Template.Views[i].Id,
                    url = handler_Rendering.GetUrl(viewId, false);
                url += String.format("&n=true&h={0}&w={0}", $(window).height() - 150);
                var cssClass = '';
                if (i == 0) {
                    cssClass += 'first show';
                }
                else if (i == models.Template.Views.length - 1) {
                    cssClass += 'last';
                }
                summaryImg += (String.format("<li id='cap{2}' class='{1}' data-index='{2}'><img src='{0}'/></li>", url, cssClass, i));
            }
            summaryImg = "<ul class='summary-images'>" + summaryImg + "</ul>";



            var messageHtml = '';
            for (var i = 0; i < validator.items.length; i++) {
                messageHtml += validator.items[i].Message;
            }

            var capPreview = $("<div class='row region-details'></div>");
            var leftArrow = $("<i class='fa fa-chevron-left'></i>");
            leftArrow.on("click", function () { changeImage(-1); });

            capPreview.append($("<div class=\"col-xs-1\"></div>").append(leftArrow));
            capPreview.append("<div class=\"col-xs-10\">" + summaryImg + "</div>");
            var rightArrow = $("<i class='fa fa-chevron-right'></i>");
            rightArrow.on("click", function () { changeImage(1); });
            capPreview.append($("<div class=\"col-xs-1\"></div>").append(rightArrow));

            capPreview.kendoTouch({
                enableSwipe: true,
                minXDelta: 150,
                maxDuration: 250,
                multiTouch: true,
                swipe: function (e) {
                    var dir = e.direction;
                    if (dir == 'right' || dir == 'left') {
                        var changeBy = 1;
                        if (dir == 'left') {
                            changeBy = -1;
                        }
                        changeImage(changeBy);
                    }
                }
            });

            var detailsTitle = $(String.format("<div class=\"col-xs-12 details-title\">{0}</div>", "CAP SUMMARY"));
            detailsTitle.on("click", function () {
                var container = $(this).parent();
                container.toggleClass("show");
                var dbody = container.find(".details-body");
                if (dbody.hasClass("mCustomScrollbar")) {
                    dbody.mCustomScrollbar("destroy");
                }

                var scrollOptions = $.extend({}, { contentTouchScroll: 100, mouseWheelPixels: 50, scrollbarPosition: "inside", autoHideScrollbar: true, scrollInertia: 100, mouseWheel: { scrollAmount: 50 } });
                detailsBody.mCustomScrollbar(scrollOptions);
            });
            var detailsBody = $(String.format("<div class=\"col-xs-12 details-body\">{0}</div>", messageHtml));
            detailsBody.mCustomScrollbar("update");
            capPreview.append($("<div class=\"row text-details\"></div>").append(detailsTitle).append(detailsBody));


            var html = capPreview;

            if (validator.Level == LEVELS.PASS) {
                options.confirm.call(this, {
                    title: "CAP SUMMARY",
                    message: html,
                    showConfirm: true,
                    showClose: true,
                    showDecline: true,
                    showExclamation: false,
                    declineText: "<i class='fa fa-chevron-left' aria-hidden='true'></i> BACK",
                    confirmText: "PLACE ORDER <i class='fa fa-chevron-right' aria-hidden='true'></i>",
                    cssClass: "fixed",
                    confirmCallback: function () {
                        //options.pass.call(this, options.order);
                    }

                });
            }
            else if (validator.Level == LEVELS.WARN) {

                options.warn.call(this, {
                    title: "CAP SUMMARY: WARN",
                    message: html,
                    cssClass: "fixed",
                    showExclamation: false,
                    declineText: "<i class='fa fa-chevron-left' aria-hidden='true'></i> BACK",
                    confirmText: "PLACE ORDER <i class='fa fa-chevron-right' aria-hidden='true'></i>",
                });
            }
            else {
                //show errors show can't continue until they are resolved
                html = "<strong>PLEASE CORRECT ERRORS BELOW</strong><BR/><BR/>";
                validator.errors.forEach(function (e) {
                    html += "<div>" + e.Message + "</div>";
                });
                options.fail.call(this, {
                    title: "CAP SUMMARY: FAIL",
                    message: html,
                    showDecline: false,
                    closeText: "<i class='fa fa-chevron-left' aria-hidden='true'></i> BACK",
                });
            }

            changeImage = function (changeBy) {
                var crnt = $("ul.summary-images li.show");

                $("ul.summary-images li").removeClass("show");
                if (crnt.hasClass("last") && changeBy == 1) {
                    $("ul.summary-images li.first").addClass("show");
                }
                else if (crnt.hasClass("first") && changeBy == -1) {
                    $("ul.summary-images li.last").addClass("show");
                }
                else {
                    $("#cap" + (crnt.data("index") + changeBy)).addClass("show");
                }
            };
        }

        function saveHandle(options) {
            var opts = $.extend({}, {
                pass: function (o) {

                    var callback;
                    if (opts.order) {
                        callback = function () {
                            var h = formatOrderUrl();
                            window.location.href = h;
                        };
                    }
                    prepAndSend(callback);
                    //namePrompt(o.order);
                },
                warn: function (o) {
                    notify.show({
                        message: o.message,
                        showConfirm: true,
                        showClose: false,
                        showDecline: true,
                        showExclamation: o.showExclamation,
                        declineText: o.declineText,
                        confirmText: o.confirmText,
                        closeText: o.closeText,
                        cssClass: "fixed",
                        confirmCallback: function () {
                            prepAndSend();
                        }
                    });
                },
                confirm: function (o) {
                    notify.show({
                        message: o.message,
                        showConfirm: true,
                        showClose: false,
                        showDecline: true,
                        showExclamation: o.showExclamation,
                        declineText: o.declineText,
                        confirmText: o.confirmText,
                        closeText: o.closeText,
                        cssClass: "fixed",
                        confirmCallback: function () {
                            prepAndSend();
                        }
                    });
                }
            }, options);
            //console.log("saveHandle:validate Handle");
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
                    notify.show({
                        message: o.message,
                        showDecline: false,
                        confirm: function () {
                            prepAndSend();
                        }
                    });
                }
            }, options);
            //console.log("publishHandle:validate Handle");
            validateHandle(opts);
        }

        handlers.showName = namePrompt;
        handlers.save = saveHandle;

    })();


    window._nec_ = nec;
})(window);