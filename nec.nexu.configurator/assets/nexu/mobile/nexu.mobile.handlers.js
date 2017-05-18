; (function (window, undefined) {
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
    nec.nexu.mobile.controls.colorpicker = nec.nexu.mobile.controls.colorpicker || {};


    function _getGlobalObjectFromString(str) {
        var a = str.split('.'), obj = window;
        while (a.length > 0) {
            obj = obj[a.shift()];
        }
        return obj;
    }

    var data = nec.nexu.data, config = nec.nexu.config, k_Ds = kendo.data.DataSource, mobile = nec.nexu.mobile, CHANGE = 'change', EMPTY = '', ui = nec.nexu.ui,
        models = nec.nexu.models, colorSelectorMenuVisible = false, logoSelectorMenuVisible = false;

    mobile.Handlers = mobile.Handlers || {};
    /* utility function -- turns the main menu into list or tiles (for color swatches) */
    function _tileize(a) {
        if (a)
            mobile.controls.MainMenu.element.addClass('tiled');
        else
            mobile.controls.MainMenu.element.removeClass('tiled');
    }
    /* generic function to execute common functions. called by individual handlers. rebuilds the main menu and updates handlers and UI */
    function _changeMainMenu(helperMessage, showHelper, dataSrc, bind) {
        //console.log("changeMainMenu");
        mobile.model.set('MobileHelperName', helperMessage);
        mobile.model.set('ShowHelper', showHelper);
        mobile.controls.MainMenu.element.empty();
        mobile.controls.MainMenu.setDataSource(dataSrc);
        mobile.controls.MainMenu
            .unbind(CHANGE)
            .bind(CHANGE, bind);
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
    /* utility method -- checks if a logo location has a conflict. 
    @@TODO not integrated at the moment, apply to the location selection handler 2015.08.21 -amo */
    function _CheckConflict(selected) {
        var returnVal = null;
        if (selected.SharedLocations && selected.SharedLocations.length) {
            var logos = _nec_.nexu.models.Product.Logos;
            for (var i = 0; i < logos.length; i++) {
                if (selected.SharedLocations.indexOf(logos[i].Location.Id) > -1 && logos[i].DesignColorway) {
                    returnVal = logos[i].Location;
                    break;
                }
            }
        }
        return returnVal;
    }
    function _IsLocationUsed(location) {
        var f = _.find(models.Product.Logos, function (d) {
            return d.Location.Id == location.Id;
        });
        if (!f)
            return false;
        if (f.DesignColorway != null) {
            return location;
        }
        return null;
    }

    function _RemoveLogo(code) {
        nec.nexu.mobile.viewer.widget().clearLogo(code);
        for (var i = 0; i < nec.nexu.models.Product.Logos.length; i++) {
            if (nec.nexu.models.Product.Logos[i].Location.Code == code) {
                nec.nexu.models.Product.Logos[i].set('DesignColorway', null);
                break;
            }
        }
    }

    /* Secondary Menu Change Handler 
     
     Generic method to handle the "Back" click of an item in the secondary menu.
     The appropriate backfunction is stored as a string in the data item.
     when the back is clicked, we remove the selected item (and all items below it in the list),
     then execute the appropriate function to repopulate the main menu

     The {xxx}Back function listed in the main menu section handle most of the logic of the "backwards" navigation
   */
    mobile.Handlers.GenericBack = function (e) {
        var el = e.sender.select(), item = e.sender.dataItem(el), funcStr = item.BackFunction,
        removeIndex = el.index(), ds = e.sender.dataSource;

        // Remove any items listed after the selected item
        for (var i = e.sender.dataSource.total() - 1 ; i >= removeIndex; i--) {
            e.sender.dataSource.remove(e.sender.dataSource.at(i));
        }

        // call the appropriate back function
        var func = _getGlobalObjectFromString(funcStr);
        if (func && typeof func === 'function')
            func.call(item);
    }



    /* Main Menu Change Handlers */
    function _auto_match(sourceId, material) {
        for (var i = 0; i < models.Product.Regions.length; i++) {
            if (models.Product.Regions[i].Region.MatchingRegion == sourceId && ui.Matching[models.Product.Regions[i].Region.Id]) {
                models.Product.Regions[i].set('Material', material);
                models.Product.Regions[i].UserAssigned = true;
            }
        }
    }

    /* --- Color Behavior 
    
        recolors the cap.... eventually 2015.08.21 -amo
    */
    mobile.Handlers.ColorClick = function (e) {
        var item = e.sender.dataItem(e.sender.select());
        mobile.model.set('MobileHelperName', item.DisplayName);
        mobile.model.set('ShowHelper', true);


        if (ui.Selected.ChangeAll) {
            /* all regions */
            for (var i = 0; i < models.Product.Regions.length; i++) {
                models.Product.Regions[i].set('Material', item);
                models.Product.Regions[i].UserAssigned = true;
            }
        }
        else if (ui.Selected.get('ForcedGroup')) {
            /* usually used for eyelets region */
            var regionIds = ui.Selected.get('ForcedGroup');
            for (var i = 0; i < models.Product.Regions.length; i++) {
                if (regionIds.indexOf(models.Product.Regions[i].Region.Id) > -1) {
                    models.Product.Regions[i].set('Material', item);
                    models.Product.Regions[i].UserAssigned = true;
                    ui.Matching[models.Product.Regions[i].Region.Id] = false;
                }
            }

        }
        else {
            /* Single region */
            var r = $.map(models.Product.Regions, function (obj) {
                if (obj.Region.Id == ui.Selected.Region.Id)
                    return obj;
            })[0];
            if (r) {
                r.set('Material', item);
                r.set('UserAssigned', true);
            }
            _auto_match(ui.Selected.Region.Id, item);
            if (ui.Selected.Region.MatchingRegion) {
                ui.Matching[ui.Selected.Region.Id] = false;
            }

        }

        var recents = nec.nexu.data.ColorLists.get(-2);
        if (!_.findWhere(recents.Colors, { Id: item.Id }))
            recents.Colors.push(item);

    }


    /* --- Color List (fabric type) Navigation 
    
        Loads the list of colors associated with that list
    */
    mobile.Handlers.ColorListForward = function (e) {
        console.log("ColorListForward");
        // Get Data
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.ColorListBack' }));
        // Set UI
        mobile.model.set('MobileHelperName', 'Choose a color...');
        mobile.model.set('ShowHelper', true);
        // Turn Main Menu into Tile Menu
        _tileize(true);
        // Make the color swatches
        // @@TODOD pattern logic
        mobile.controls.MainMenu.one('dataBound', function (evt) {
            evt.sender.element.children().each(function (index, div) {
                var d = evt.sender.dataItem(div);
                $(div).css('background-color', '#' + d.Hex.replace('#', EMPTY)).text(EMPTY);
                //$(div).find('.col-xs-10').css('background-color', '#' + d.Hex.replace('#', EMPTY)).text(EMPTY);
            });
        });
        // Set Data
        mobile.controls.MainMenu.setDataSource(k_Ds.create(item.Colors));
        // Set Behavior
        mobile.controls.MainMenu
            .unbind(CHANGE, mobile.Handlers.ColorListForward)
            .bind(CHANGE, mobile.Handlers.ColorClick);
    }
    mobile.Handlers.ColorListBack = function () {
        _tileize(false);
        _changeMainMenu(
            'Choose a palette...',
            true,
            nec.nexu.data.ColorLists,
            mobile.Handlers.ColorListForward);
    }


    /* --- Panel (Region) Navigation 
        
        region click brings up a list of color lists (fabric types)
    */
    mobile.Handlers.PanelForward = function (e) {
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.PanelBack' }));

        if (item.IsSelectAll) {
            ui.Selected.ChangeAll = true;
            ui.Selected.ForcedGroup = null;
        }
        else if (item.IsForcedGroup) {
            ui.Selected.ChangeAll = false;
            ui.Selected.ForcedGroup = item.RegionIds;
        }
        else {
            ui.Selected.ChangeAll = false;
            ui.Selected.ForcedGroup = null;
            ui.Selected.set('Region', item);
        }

        _tileize(false); // List style for menu
        _changeMainMenu('Choose a palette...', true, nec.nexu.data.ColorLists, mobile.Handlers.ColorListForward);
    }
    mobile.Handlers.PanelBack = function () {
        _tileize(false);
        var group = this.getParent(),
            items = $.map(group.Regions, function (o, i) { return $.extend({}, o, { getParent: function () { return group; } }) });
        if (group.AllowRecolorAll) {
            items.splice(0, 0, { DisplayName: 'All', IsSelectAll: true });
        }
        if (group.ForceGroup) {
            items = [];
            items.push(
            {
                DisplayName: 'All',
                IsForcedGroup: true,
                IsSelectAll: false,
                RegionIds: $.map(group.Regions, function (o) { return o.Id }),
                getParent: function () { return group; }
            });
        }
        _changeMainMenu(EMPTY, false, k_Ds.create(items), mobile.Handlers.PanelForward);
    }



    /* --- Thread Behavior 
        
        recolors the logo.... eventually 2015.08.21 -amo
    */
    mobile.Handlers.ThreadForward = function (e) {
        var item = e.sender.dataItem(e.sender.select());
        mobile.model.set('MobileHelperName', item.DisplayName);
        mobile.model.set('ShowHelper', true);

        //@@TODO ViewModel modification
        ui.Selected.set('Layer.Color', item);

        if (!ui.RecentThreads.get(item.Id))
            ui.RecentThreads.add(item);

    }



    mobile.Handlers.getMatchingThreadsFilter = function (all) {
        /* Helper function for when "Matching colors" selected */
        var filters = [],
			regions = models.Product.Regions.toJSON();
        /* Loop through the regions and get the Ids of matching Design colors */
        for (var i = 0; i < regions.length; i++) {
            var region = regions[i].Material;
            if (region.MatchingDesignColorIds) {
                /* depending on how many matching colors there are, we have to send add an extra loop to filter. */
                if (region.MatchingDesignColorIds.length == 1) {
                    //var obj = _.findWhere(all, { Id: region.MatchingDesignColorIds[0] });
                    //if (obj)
                    filters.push({ field: 'Id', operator: 'eq', value: /*obj.Id*/region.MatchingDesignColorIds[0] });
                }
                else {
                    //var obj = _.find(all, { Id: region.MatchingDesignColorIds[0] });
                    //if (obj) {
                    //	for (var o in obj) {
                    //		filters.push({ field: 'Id', operator: 'eq', value: o.Id });
                    //	}
                    //}
                }
            }
        };
        return filters;
    }
    /* --- Thread List Navigation 
        when a thread list is selected, load the color grid for the design colors
    */
    mobile.Handlers.ThreadListForward = function (e) {
        console.log("ThreadListForward");
        var item = e.sender.dataItem(e.sender.select());
        //mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.ThreadListBack' }));
        var ds = nec.nexu.data.Threads;
        ds.filter(null);

        if (item.Id == 1) {
            // Matching Colors Only
            var filters = mobile.Handlers.getMatchingThreadsFilter();//ds.data().toJSON());
            ds.filter({ logic: 'or', filters: filters })
        }
        if (item.Id == 3) {
            // Recently used
            ds = ui.RecentThreads;
        }
        /* set the background color of each item in the list */
        //mobile.controls.MainMenu.one('dataBound', function (evt) {
        //    evt.sender.element.children().each(function (index, div) {
        //        var d = evt.sender.dataItem(div);
        //        $(div).css('background-color', '#' + d.Hex.replace('#', EMPTY)).text(EMPTY);
        //    });
        //});

        //_tileize(true); // tile mode
        //changeMainMenu('Select a Color...', true, k_Ds.create(ds), mobile.Handlers.ThreadForward);

        //show in color menu
        //var threadColors = data.getThreadColorCollection();

        var view = ds.view();
        mobile.controls.colorpicker.init(
            {
                dataSource: [$.extend(item, { Colors: view })], 
                select: function (e) {
                    var item = e.data;
                    mobile.model.set('MobileHelperName', item.DisplayName);
                    mobile.model.set('ShowHelper', true);

                    //@@TODO ViewModel modification
                    ui.Selected.set('Layer.Color', item);

                    if (!ui.RecentThreads.get(item.Id))
                        ui.RecentThreads.add(item);
                }
            });
    
        mobile.controls.menu.showColorList();

        //create function to populate color from dataset; for now testing with this
        $("#mobile-color-selector").addClass("show");
        $("#mobile-logo-selector").removeClass("show");
    }
    mobile.Handlers.ThreadListBack = function () {
        var colorLists = [
            { DisplayName: 'All Colors', Id: 0 },
            { DisplayName: 'Matching Colors', Id: 1 }
        ];
        if (ui.RecentThreads.total() > 0) {
            colorLists.push({ DisplayName: 'Recent Colors', Id: 3 });
        }
        _tileize(false);
        _changeMainMenu('Select thread list...', true, k_Ds.create(colorLists), mobile.Handlers.ThreadListForward);
    }

    /* --- Layer Navigation 
        
        when a design layer is selected, load the thread lists. which are hardcoded at the moment to be all or matching.
    */
    mobile.Handlers.LayerForward = function (e) {
        console.log("mobile.Handlers.LayerForward");
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.LayerBack' }));
        ui.Selected.set('Layer', item);
        var colorLists = [
            { DisplayName: 'All Colors', Id: 0 },
            { DisplayName: 'Matching Colors', Id: 1 }
        ];
        if (ui.RecentThreads.total() > 0) {
            colorLists.push({ DisplayName: 'Recent Colors', Id: 3 });
        }
        _tileize(false);
        _changeMainMenu('Select thread list...', true, k_Ds.create(colorLists), mobile.Handlers.ThreadListForward);
    }
    mobile.Handlers.LayerBack = function () {
        console.log("mobile.Handlers.LayerBack");
        _tileize(false);
        _changeMainMenu(
            'Modify the Logo',
            true,
            k_Ds.create({
                data: ui.Selected.get('Logo.DesignColorway.Layers'),
                filter: { field: 'Recolorable', operator: 'eq', value: true },
                sort: { field: 'SortOrder', dir: 'asc' }
            }),
            mobile.Handlers.LayerForward)
    }



    /* --- Logo Navigation 
    
    This event comes from the Listview in the Logo Browser Window 
    Load the default colorway from the webservice and populate the layers list
    using generic Ajax call do to type of return object from service. might want to make a method that just returns the list of layers.
    Also may need to incorporate design recolor rules
    */
    mobile.Handlers.LogoForward = function (e) {
        console.log("mobile.Handlers.LogoForward");
        var item = e.sender.dataItem(e.sender.select());
        if (!item) /* sometime the listview change event is called multiple times and cannot be selected */
            return;

        //remove previous
        _.where(mobile.controls.SecondaryMenu.dataSource.data(), { IsLogo: true }).forEach(function (i) {
            mobile.controls.SecondaryMenu.dataSource.remove(i);
        });
        // add current logo
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.LogoBack', IsLogo: true }));

        $.ajax($.extend(nec.nexu.data.ColorwayConfiguration, { data: { designId: item.Id } })
        ).done(function (colorway) {

            //nec.nexu.models.
            //@@TODO wire up to observable
            // get selected design
            // get selected location from models.Product
            // set ui.Selected.DesignColorway to the colorway object
            ui.Selected.Logo.set('DesignColorway', $.extend({}, colorway, { Design: item }));


            _changeMainMenu(
             'Modify the Logo', true,
             k_Ds.create({ /* this datasource needs to only display layers marked as recolorable */
                 data: ui.Selected.Logo.get('DesignColorway.Layers'),
                 filter: { field: 'Recolorable', operator: 'eq', value: true },
                 sort: { field: 'SortOrder', dir: 'asc' }
             }),
             mobile.Handlers.LayerForward);
        });

    }
    mobile.Handlers.LogoBack = function (e) {
        console.log("mobile.Handlers.LogoBack");
        _openLogoWindow();
        _changeMainMenu('Choose a logo...', true, k_Ds.create([]), $.noop);
    }

    /* Team Navigation 

    When a team is selected, the intended behavior is to open the logo window  overlay for the user to select a logo from.
    The logo window module is also used in the desktop version of the site, but needs to be modified for the mobile app.
    The _openLogoWindow() function handles the necessary modifications for the logo window
    The TeamForward handler opens the window and clears the existing Main Menu. Also stores selected team to the app model "SelectedHierarchy" property.
    */
    function _openLogoWindow(opts) {

        //console.log("_openLogoWindow");

        var item = nec.nexu.models.SelectedHierarchy;

        var dsConf = $.extend({}, nec.nexu.data.DesignsConfiguration);
        $.extend(dsConf, {
            transport: $.extend({}, dsConf.transport, {
                parameterMap: function () {
                    return { hierarchyId: item.Id, locationId: nec.nexu.models.SelectedLocation.Id };
                }
            })
        });
        mobile.controls.logowindow.open({ data: dsConf });

        function old() {


            var options = $.extend({
                windowTitle: 'New Era by You',
                listChange: function (e) {
                    module_LogoWindow.Close();
                    mobile.Handlers.LogoForward(e);
                    //e.stopPropagation();
                },
                listBound: function (e) {
                    //once the list is drawn, set the tiles to square size and normalize vertial alignment
                    e.sender.element.children('li').each(function (index, ele) {
                        var element = $(ele), w = element.width() - 24;
                        element.css('line-height', w + 'px');
                        $('img', element).css({ 'max-height': w + 'px' });
                    });
                },
                listTemplate:
                    '<li style="display:inline-block; width:25%; vertical-algin:middle;">' +
                    '<div style="width:100%;"><div style="padding:12px;">' +
                    '<img style="max-width:100%;" src="#: OfficialImage #?h=132" title="#: DisplayName #" /></div></div></li>'
            }, opts);

            var item = nec.nexu.models.SelectedHierarchy; // Required.
            options.windowTitle = (item) ? item.DisplayName : options.windowTitle;

            /* build the data source to populate the pop-ups listview */
            var dsConf;
            if (options.dataSource) {
                dsConf = options.dataSource;
            } else {

                dsConf = $.extend({}, nec.nexu.data.DesignsConfiguration);
                $.extend(dsConf, {
                    transport: $.extend({}, dsConf.transport, {
                        parameterMap: function () {
                            return { hierarchyId: item.Id, locationId: nec.nexu.models.SelectedLocation.Id };
                        }
                    })
                });
            }
            /* custom options for the listview */
            var listOptions = {
                template: options.listTemplate,
                change: options.listChange,
                dataBound: options.listBound
            }

            // open the window, overriding a lot of the default module properties
            module_LogoWindow.Open({
                title: options.windowTitle,
                data: k_Ds.create(dsConf),
                listViewOptions: listOptions,
                //maxWidth: window.outerWidth,
                //maxHeight: document.documentElement.clientHeight,
                //height: document.documentElement.clientHeight,
                //minWidth: window.innerWidth - 2,
                //width: window.innerWidth - 2,
                //position: { top: 40, left: 0 },
                modal: true,
                winOverrides: {
                    open: function (evt) {
                        evt.sender.fullScreen();
                        //evt.sender.element.closest('.k-window').addClass('nexu-mobile-window nx-scrollable');
                    },
                    close: function (evt) {
                        //evt.sender.destroy();
                    }
                }
            });


        }


    } /* end _openLogoWindow */


    function change_Hierarchy(options) {
        //console.log("change_Hierarchy");
        var opts = $.extend({
            message: 'You are about to change the team. This will remove existing logos from the cap. Continue?',
            showConfirm: true,
            showDecline: true,
            confirmCallback: function () {
                for (var i = 0; i < models.Logos.length; i++) {
                    var l = models.Logos[i];
                    if (l.DesignColorway != null && l.Location.Removeable) {
                        _RemoveLogo(l.Location.Code);
                    }
                }

                if (options.confirmCallback) {
                    options.confirmCallback.call();
                }
            },
            declineCallback: function () {
                if (options.decline) {
                    options.decline.call();
                }
            }
        }, options);
        var currentH = models.SelectedHierarchy;
        //console.log("currentH", currentH);
        //console.log("options", options);


        if (currentH && currentH.Id != options.HierarchyId) {
            mobile.controls.notification.show(opts);
        }
        else {
            opts.confirmCallback.call();
        }
    }


    mobile.Handlers.TeamForward = function (e) {
        console.log("mobile.Handlers.TeamForward");
        if (!e.sender)
            return;
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.TeamBack' }));

        change_Hierarchy({
            confirmCallback: function () {
                nec.nexu.models.SelectedHierarchy = item;
                _openLogoWindow();
                _changeMainMenu('Choose a logo...', true, k_Ds.create([]), $.noop);
                // pass empty function as "forward" action. Logo window will handle callback when a logo is picked
            },
            HierarchyId: item.Id
        });
    }

    /* back function repopulates the hierarchy list. need to call datasource */
    mobile.Handlers.TeamBack = function (e) {
        console.log("mobile.Handlers.TeamBack");
        var selectedLoc = ui.Selected.get('Logo.Location') || { Id: 0 },
            selectedCat = nec.nexu.models.SelectedHierarchy.ParentHierarchy || { Id: 0 };
        var hc = nec.nexu.data.HierarchyConfiguration,
            ds = k_Ds.create(
                $.extend({}, hc, {
                    transport: $.extend({}, hc.transport, {
                        parameterMap: function (model) {
                            return { parentId: selectedCat.Id, locationId: selectedLoc.Id, templateId: nec.nexu.models.Template.Id };
                        }
                    })
                }));
        _tileize(false); // List style for menu
        _changeMainMenu('Choose a team...', true, ds, mobile.Handlers.TeamForward);
    }


    /* --- Category (League) Navigation
    
    Once the user selects a league, change the menu to display relevant children for that hierarchy
    Similiar to the location selection, we build a new hierarchy datasource, but pass the parent id of the selected league
    There may be issues there are no children for a hierarchy. @@TODO 2015.08.21 -amo
    */
    mobile.Handlers.CategoryForward = function (e) {
        console.log("mobile.Handlers.CategoryForward");
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.CategoryBack' }));

        var selectedLoc = ui.Selected.get('Logo.Location') || { Id: 0 };
        var hc = nec.nexu.data.HierarchyConfiguration,
            ds = k_Ds.create(
                $.extend({}, hc, {
                    transport: $.extend({}, hc.transport, {
                        parameterMap: function (model) {
                            return { parentId: item.Id, locationId: selectedLoc.Id, templateId: nec.nexu.models.Template.Id };
                        }
                    })
                }));

        _tileize(false);
        _changeMainMenu(
            'Choose a team...',
            true,
            ds,
            mobile.Handlers.TeamForward);

    }
    mobile.Handlers.CategoryBack = function (e) {
        console.log("mobile.Handlers.CategoryBack");
        _tileize(false);
        var selectedLoc = ui.Selected.get('Logo.Location') || { Id: 0 };
        var hc = nec.nexu.data.HierarchyConfiguration, ds = k_Ds.create(
            $.extend({}, hc, {
                transport: $.extend({}, hc.transport, {
                    parameterMap: function (model) {
                        return { parentId: 0, locationId: selectedLoc.Id, templateId: nec.nexu.models.Template.Id };
                    }
                })
            }));
        _changeMainMenu('Choose a Cateogry...', true, ds, mobile.Handlers.CategoryForward);
    }



    /* --- Location Navigation
    
    Selecting a logo location, moves the user to selecting a "league"
    We create a new datasource from the existing "hierarcy" service
    For root hierarchy, we need to pass location and template
    */
    mobile.Handlers.EditLogo = function (e) {
        var item = e.sender.dataItem(e.sender.select());

        if (item.Id === -1) {
            //Edit
            _tileize(false);
            _changeMainMenu(
                'Modify the Logo',
                true,
                k_Ds.create({
                    data: ui.Selected.get('Logo.DesignColorway.Layers'),
                    filter: { field: 'Recolorable', operator: 'eq', value: true },
                    sort: { field: 'SortOrder', dir: 'asc' }
                }),
                mobile.Handlers.LayerForward)
        }
        if (item.Id === -2) {
            //Replace
            console.log("Replace");
            _setLocation(nec.nexu.models.SelectedLocation);
        }
        if (item.Id === -3) {
            //Remove
            _RemoveLogo(nec.nexu.models.SelectedLocation.Code);
            nec.nexu.models.SelectedLocation = null;
            mobile.resetMenu();
        }

    }
    function _setLocation(item, edit) {
        //console.log("_setLocation");
        var ds, forwardFunc, showHelp = false, helpText = EMPTY;

        //mobile.viewer.changeView(_.findWhere(ui.Views, { Id: item.TemplateViewId }));
        nec.nexu.mobile.controls.menu.changeViewSelectorId(item.TemplateViewId);

        /* Get the logo from the product model so the view updates */
        var boundLogo = $.map(models.Product.Logos, function (obj) {
            if (obj.Location.Id == item.Id)
                return obj;
        });
        if (boundLogo.length) {
            ui.Selected.set('Logo', boundLogo[0]);
        }
        nec.nexu.models.SelectedLocation = item;

        if (edit) {
            var opts = [];
            opts.push({ DisplayName: 'Edit', Id: -1 });
            if (item.CanChoose)
                opts.push({ DisplayName: 'Replace', Id: -2 });
            if (item.Removable)
                opts.push({ DisplayName: 'Remove', Id: -3 });

            ds = k_Ds.create(opts); //[{ DisplayName: 'Edit', Id: -1 }, { DisplayName: 'Replace', Id: -2 }]);
            forwardFunc = mobile.Handlers.EditLogo;
        }
        else {
            var hc = nec.nexu.data.HierarchyConfiguration;
            ds = k_Ds.create(
                $.extend({}, hc, {
                    transport: $.extend({}, hc.transport, {
                        parameterMap: function (model) {
                            return { parentId: 0, locationId: item.Id, templateId: nec.nexu.models.Template.Id };
                        }
                    })
                }));
            helpText = 'Choose a category...';
            showHelp = true;
            forwardFunc = mobile.Handlers.CategoryForward;
        }
        _tileize(false); // List style for menu
        _changeMainMenu(helpText, showHelp, ds, forwardFunc);
    }
    mobile.Handlers.LocationForward = function (e) {
        var item = e.sender.dataItem(e.sender.select());
        mobile.controls.SecondaryMenu.dataSource.add($.extend({}, item, { BackFunction: '_nec_.nexu.mobile.Handlers.LocationBack' }));

        var conflict = _CheckConflict(item);

        if (conflict) {

            mobile.controls.notification.show({
                message: 'If you proceed, the current ' + conflict.DisplayName + ' will be removed. Proceed?',
                showConfirm: true,
                showDecline: true,
                confirmCallback: function () {
                    _RemoveLogo(conflict.Code);
                    console.log("LocationForward(1)");
                    _setLocation(item);
                }
            });

        }
        else {
            if (_IsLocationUsed(item)) {
                console.log("LocationForward(2)");
                _setLocation(item, true);
            }
            else {
                //console.log("LocationForward(3)");
                //console.log(item.TemplateViewId);
                _setLocation(item);
            }
        }

    }
    mobile.Handlers.LocationBack = function () {
        /* reset the menu to location selection */
        _tileize(false);
        var primaryNeeded = _IsPrimaryLocationNeeded(),
            group = this.getParent(),
            items = $.map(group.Locations, function (o, i) {
                if (primaryNeeded) {
                    if (o.IsPrimary) {
                        return $.extend({}, o, { getParent: function () { return group; } });
                    }
                }
                else {
                    return $.extend({}, o, { getParent: function () { return group; } });
                }
            });

        _changeMainMenu(EMPTY, false, k_Ds.create(items), mobile.Handlers.LocationForward);
    }


    function _background_select(e) {
        e.sender.element.closest('.k-window-content').data('kendoWindow').close();
        var item = e.sender.dataItem(e.sender.select());

        if (!item)
            return;
        ui.Background = item;
        //mobile.controls.background.css('background-image', 'url(' + config.ASSETS + item.FilePath + ')');
    }

    /* --- Region Groups Navigation 

    This is considered the "root" menu
    when a region group is clicked, the "main menu" should repopulate with the associated regions.
    additional logic needs to be applied if the group allows "Recolor All" functionality or the "Force Group" option.
    Also, the "Logos" option is somewhat static and has unique behavior for repopulating the main menu
    */
    mobile.Handlers.RegionGroupForward = function (e) {
        var item = e.sender.dataItem(e.sender.select());
        // Add item to header per design doc.
        mobile.controls.HeaderMenu.dataSource.data([item]);
        mobile.controls.HeaderMenu.refresh();
        mobile.controls.HeaderMenu.element.find('.col-xs-12').removeClass('col-xs-12').addClass('col-xs-9');

        var items = [], nextFunc;

        if (item.Id === -1) {
            /* Logo Logic */
            var primaryNeeded = _IsPrimaryLocationNeeded();
            items = $.map(item.Locations, function (o, i) {
                if (primaryNeeded) {
                    if (o.IsPrimary) {
                        return $.extend({}, o, { getParent: function () { return item; } });
                    }
                }
                else {
                    return $.extend({}, o, { getParent: function () { return item; } });
                }
            });
            nextFunc = mobile.Handlers.LocationForward;
        }
        else if (item.Id === -5) {
            // Logo Locations

            nec.nexu.mobile.controls.backgroundwindow.open();
        }
        else if (item.Id === -2) {
            // Background

            nec.nexu.mobile.controls.backgroundwindow.open();
        }
        else if (item.Id === -3) {
            // Color Grabber

            mobile.controls.colorgrabber.open();

        }
        else {


            //BT - 2016/10/17 use static method
            mobile.Handlers.showStaticColorSelector();


            return false;

            /* This logic is specific to the recolorable fabric parts of the cap */
            items = $.map(item.Regions, function (o, i) { return $.extend({}, o, { getParent: function () { return item; } }) });
            if (item.AllowRecolorAll & !item.ForceGroup) {
                items.splice(0, 0, { DisplayName: 'All', IsSelectAll: true, getParent: function () { return item; } });
            }
            if (item.ForceGroup) {
                items = [];
                items.push({
                    DisplayName: 'All',
                    IsForcedGroup: true,
                    IsSelectAll: false,
                    RegionIds: $.map(item.Regions, function (o) { return o.Id }),
                    getParent: function () { return item; }
                });
            }
            nextFunc = mobile.Handlers.PanelForward;
        }

        _tileize(false);
        _changeMainMenu(EMPTY, false, k_Ds.create(items), nextFunc);
        return false;

    }

    //
    mobile.Handlers.showStaticColorSelector = function () {
        mobile.controls.menu.setLogoSelectorVisibile(false);
        mobile.controls.menu.setColorSelectorVisibile(true);
    }

    mobile.Handlers.showStaticLogoSelector = function () {
        mobile.controls.menu.setColorSelectorVisibile(false);
        mobile.controls.menu.setLogoSelectorVisibile(true);
    }

    window._nec_ = nec;
})(window);