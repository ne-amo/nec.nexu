
/* Event Binding */
; (function ($, kendo, _, window, undefined) {

    /* global namespacing */
    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.models = nec.nexu.models || {};
    nec.nexu.ui = nec.nexu.ui || {};
    nec.nexu.data = nec.nexu.data || {};
    nec.nexu.config = nec.nexu.config || {};
    nec.nexu.events = nec.nexu.events || {};

    /* local shorthand variables */
    var data = nec.nexu.data,
		config = nec.nexu.config,
		mobile = nec.nexu.mobile,
		models = nec.nexu.models,
		ui = nec.nexu.ui,
		events = nec.nexu.events,
		k_Ds = kendo.data.DataSource;

    /* private methods */
    function _event_Product_Load(msg) {
        var def = $.Deferred();

        if (msg && msg.Template) {
            /* Set the template to the model and initialize left menu */
            msg.Template.Views = _.sortBy(msg.Template.Views, function (x) {
                return x.SortOrder;
            });
            mobile.model.set('PriceDescription', msg.Template.PriceDescription);
            mobile.model.set('StyleDescription', msg.Template.DisplayName);
            models.Template = kendo.observable(msg.Template);
            models.Product = kendo.observable({
                Regions: $.map(msg.Regions, function (o, i) {
                    return $.extend(o, {
                        UserAssigned: false
                    });
                }),
                Logos: $.map(models.Template.DesignLocations, function (o, i) {
                    var cw = $.map(msg.Designs, function (a) {
                        if (a.Location.Id == o.Id)
                            return a.DesignColorway;
                    })[0];
                    return {
                        Location: o,
                        DesignColorway: cw
                    }
                })
            });
            if (msg.Hierarchy)
                models.SelectedHierarchy = kendo.observable(msg.Hierarchy);
            models.Product.set('DisplayName', msg.DisplayName);
            if (ui.Mode == ui.Modes.EDIT) {
                models.Product.set('Id', msg.Id);
                models.Product.set('MaterialId', null);
            }
            if (msg.Keywords) {
                for (var i = 0; i < msg.Keywords.length; i++) {
                    ui.Keywords.data().push(msg.Keywords[i]);
                }
            }
            if (msg.Hierarchy)
                ui.Selected.set('Hierarchy', msg.Hierarchy);
        }
        //draw default menu for this product.
        nec.nexu.mobile.resetMenu();

        def.resolve(msg);
        return def;
    }
    events.ProductLoad = _event_Product_Load;

    function _event_Product_Load_Done(data) {
        models.Product.Creator = kendo.observable({});
        
        // Taken from "ValidatePrimaryLogo" function in desktop nexu.main.js file
        var exists = false;
        for (var i = 0; i < models.Product.Logos.length; i++) {
            if (models.Product.Logos[i].Location.IsPrimary && models.Product.Logos[i].DesignColorway) {
                exists = true;
                break;
            }
        }
        if (exists) {
            ui.Selected.NeedPrimary = false;
        }
        else {
            ui.Selected.NeedPrimary = true;
        }
        
        if (ui.InspiredBy)
            models.Product.InspiredByFinishedProductId = ui.ProductId;

        return mobile.viewer.loadViews(data).done(function () {
            mobile.loader.hide()
        });
    }
    events.ProductDone = _event_Product_Load_Done;


    function _setMatching() {
        for (var i = 0; i < models.Product.Regions.length; i++) {
            var r = models.Product.Regions[i];
            if (r.Region.MatchingRegion) {
                ui.Matching[r.Region.Id] = true;
            }
        }
    }
    function _event_Deep_Link_Done(data) {
        // Load view carousel buttons

        /* Initialize direct link referral, this should probably changed to a deferred. */
        if (urlParams && urlParams.direct) {
            //@@TODO set up referrer
            _nexu_.Referrer.Load(urlParams.direct);
        }

        //treeview setup; enables expand/collapse on single click of labels
        var treeview = $("#regions-treeview");
        treeview.on("click", ".k-in", function (e) {
            //mobile.controls.RegionsTreeView.toggle($(e.target).closest(".k-item"));
        });

        //view-selector data//
        var sdafgds = models.Template.Views.toJSON();
        mobile.controls.ViewSelector.setDataSource(k_Ds.create(sdafgds));

        //regions-list data//
        //single regions only
        //var gfys = models.Template.RegionGroups[0].Regions.toJSON();

        //regions-dropdown data//
        var gfys = new Array();
        for (var i = 0; i < models.Template.RegionGroups.length; i++) {
            if (models.Template.RegionGroups[i].Active) {
                //dont want groups name on for now
                //gfys.push(models.Template.RegionGroups[i].toJSON());
            
                //now loop through regions
                for (var x = 0; x < models.Template.RegionGroups[i].Regions.length; x++) {
                    if (models.Template.RegionGroups[i].Regions[x].Active) {
                        gfys.push(models.Template.RegionGroups[i].Regions[x].toJSON());
                    }
                }
            }
        }
        mobile.controls.RegionDropdown.setDataSource(k_Ds.create(gfys));


        //mobile.controls.RegionsTreeView.setDataSource(k_Ds.create({
        //    id: 0, text: "All", expanded: true, spriteCssClass: "rootfolder", items: [
        //        {
        //            id: 6, text: "Panels", expanded: false, spriteCssClass: "regiongroup", items: [
        //                { id: "LFW", dataid: 11, text: "Left Front", spriteCssClass: "panel" },
        //                { id: "LMW", dataid: 12, text: "Left Middle", spriteCssClass: "panel" },
        //                { id: "LBW", dataid: 13, text: "Left Back", spriteCssClass: "panel" },
        //                { id: "RFW", dataid: 16, text: "Right Front", spriteCssClass: "panel" },
        //                { id: "RMW", dataid: 15, text: "Right Middle", spriteCssClass: "panel" },
        //                { id: "RBW", dataid: 14, text: "Right Back", spriteCssClass: "panel" }
        //            ]
        //        },
        //        {
        //            id: 7, text: "Visor", expanded: false, spriteCssClass: "regiongroup", items: [
        //                { id: "Visor", dataid: 17, text: "Visor", spriteCssClass: "panel" },
        //                { id: "Undervisor", dataid: 18, text: "Undervisor", spriteCssClass: "panel" },
        //            ]
        //        },
        //        {
        //            id: 8, text: "Eyelets", expanded: false, spriteCssClass: "regiongroup", items: [
        //                { id: "EyeletLFW", dataid: 19, text: "Left Front", spriteCssClass: "eyelet" },
        //                { id: "EyeletLMW", dataid: 20, text: "Left Middle", spriteCssClass: "eyelet" },
        //                { id: "EyeletLBW", dataid: 21, text: "Left Back", spriteCssClass: "eyelet" },
        //                { id: "EyeletRFW", dataid: 24, text: "Right Front", spriteCssClass: "eyelet" },
        //                { id: "EyeletRMW", dataid: 23, text: "Right Middle", spriteCssClass: "eyelet" },
        //                { id: "EyeletRBW", dataid: 22, text: "Right Back", spriteCssClass: "eyelet" }
        //            ]
        //        },
        //        {
        //            id: 9, text: "Stitching", expanded: false, spriteCssClass: "regiongroup", items: [
        //                { id: "FrontStitch", dataid: 25, text: "Front", spriteCssClass: "stitching" },
        //                { id: "MidStitch", dataid: 26, text: "Middle", spriteCssClass: "stitching" },
        //                { id: "BackStitch", dataid: 27, text: "Back", spriteCssClass: "stitching" },
        //                { id: "VisorStitch", dataid: 28, text: "Visor", spriteCssClass: "stitching" },
        //                { id: "UnderStitch", dataid: 29, text: "Undervisor", spriteCssClass: "stitching" }
        //            ]
        //        },
        //        {
        //            id: 10, text: "Button", expanded: false, spriteCssClass: "regiongroup", items: [
        //                { id: "Button", dataid: 30, text: "Button", spriteCssClass: "panel" }
        //            ]
        //        }
        //    ]
        //}));


        //mobile.controls.RegionsTreeView.setDataSource(k_Ds.create(nec.nexu.data.TreeViewRegions()));
        mobile.controls.RegionsTreeView.setDataSource(nec.nexu.data.TreeViewRegions());


        _setMatching(); //???
    }
    events.DeepLinkDone = _event_Deep_Link_Done;

    function _event_Viewer_Select(e) {
        //console.log("_event_Viewer_Select");
        var di = e.sender.dataSource.at(e.sender.select().index()),
            v = _.findWhere(ui.Views, { Id: di.Id });
        nec.nexu.mobile.viewer.changeView(v);
    }
    events.ViewerSelect = _event_Viewer_Select;

    function _background_ds_Init(event) {
        ui.Background = event.sender.data()[0];
        //mobile.controls.background.css('background-image', 'url(' + config.ASSETS + event.items[0].FilePath + ')');
    }
    events.BackgroundInit = _background_ds_Init;

    function _background_ds_Change(event) {
        /*
        Whenever the background datasource, we check the currently selected background to ensure it is still in the list of available backgrounds.
        If it's not available, then we alert the user, and switch to the default.
        */
        var currentId;
        if (!ui.Background)
            return;
        currentId = ui.Background.Id;
        var isAllowed = ($.map(event.items, function (o) { return o.Id; }).indexOf(currentId) > -1);
        if (isAllowed) /* is available for use, exit function */
            return;

        ui.Background = event.items[0];

        mobile.controls.background.css('background-image', 'url(' + config.ASSETS + event.items[0].FilePath + ')');

        mobile.controls.notification.show({
            message: 'The background you had selected was an exclusive offering and is no longer available to use.'
        });
    }
    events.BackgroundChange = _background_ds_Change;

    function _user_login(event) {
        mobile.controls.login.open();
        return false;
    }
    events.Login = _user_login;

    // this should move to nec.nexu.common
    function getUser(callback) {
        $.get('/nexu.token').done(function (token) {
            if (token && token != '') {
                $.ajax({
                    url: config.SVCURL + '/account/UserInfo',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    }
                }).done(function (data) {
                    callback.call(this, data);
                });
            }
            else {
                callback.call(this, null);
            }
        });
    };
    events.getUser = getUser;

    function setUserId(data) {
        models.Product.Creator.unbind('change', userSetDispatcher);
        models.Product.Creator.bind('change', userSetDispatcher);
        if (data) {
            models.Product.set('Creator.Id', data.Id);
            ui.Selected.set('CreatorNickname', data.Nickname);
        }
        else {
            models.Product.set('Creator.Id', 1);
            ui.Selected.set('CreatorNickname', '');
        }

    }
    events.setUserId = setUserId;

    function userSetDispatcher(e) {
        if (e.field === 'Id') {
            //_nexu_.ColorPicker.CheckUserColorlist();
        }
    }



    function _save(e) {
        nec.nexu.mobile.handlers.showName(true);
        return false;
    }
    events.Save = _save;

    function _confirmSave(e) {
        nec.nexu.mobile.handlers.save();
        return false;
    }
    events.ConfirmSave = _confirmSave;

    function _summary(e) {
        nec.nexu.mobile.handlers.showName(false);
        nec.nexu.mobile.controls.summary.open({ regions: models.Product.Regions, logos: models.Product.Logos });
        return false;
    }
    events.Summary = _summary;

    function _share(e) {
        nec.nexu.mobile.handlers.showName(false);
        mobile.controls.notification.show({
            message: 'Share Clicked'
        });
        return false;
    }
    events.Share = _share;

    function _help(e) {
        nec.nexu.mobile.handlers.showName(false);
        mobile.controls.notification.show({
            message: 'Help Clicked'
        });
        return false;
    }
    events.Help = _help;



    window._nec_ = nec;

})(jQuery, kendo, _, window);
