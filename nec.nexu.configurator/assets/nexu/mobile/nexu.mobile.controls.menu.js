///
///nec.nexu.mobile.controls.menu
///

//build the nav for nexu - 1st level will mainly be static
//color picker is separate from nav elements

; (function ($, kendo, _, window) {

    /* global namespaces */
    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.ui = nec.nexu.ui || {};
    models = nec.nexu.models = nec.nexu.models || {};
    mobile = nec.nexu.moile = nec.nexu.mobile || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    menu = nec.nexu.mobile.controls.menu = nec.nexu.mobile.controls.menu || {};
    cp = nec.nexu.mobile.controls.colorpicker = nec.nexu.mobile.controls.colorpicker || {};
    viewer = nec.nexu.moile.viewer;

    var barMenu = $('.collapsable-menu');

    var mainMenuTemplate = "";
    var seconaryMenuTemplate = "";

    var colorSelectorMenuVisible = false;
    var logoSelectorMenuVisible = false;

    var selectedRegions = [];

    /*public properties*/
    menu.colorMenu = $("#colorpicker-menu");
    menu.logoMenu = $("#logo-picker");
    menu.logoList = $("#mobile-logo-selector");
    menu.colorList = $("#mobile-color-selector");
    menu.regionMenu = $("#region-picker");
    menu.closeSelector = $("#close-selectors");

    /**
     * public methods
     * ****************
     * */
    menu.init = function () {
        setCustomScroller();
        bindMenus();
    }
    menu.resetView = function () {
        menu.hideSelectorMenus();
        menu.regionMenu.removeClass("show");
        menu.logoMenu.removeClass("show");

        mobile.showMainMenu(); //main buttons
        if ($("#nexu-configurator-viewer").hasClass("zoom")) {
            menu.toggleZoom();
        }

    }
    menu.hideSelectorMenus = function () {
        $(".selector-menu").removeClass("show");
        colorSelectorMenuVisible = false;
        logoSelectorMenuVisible = false;
    }

    menu.showColorSelectorMenus = function () {
        mobile.hideMainMenu();
        menu.regionMenu.addClass("show");
        menu.closeSelector.addClass("show");
        cp.init({ dataSource: nec.nexu.data.ColorLists.data(), select: colorRegion });
        menu.showColorList();
        menu.updateCustomScrollbar();
        logoSelectorMenuVisible = false;
        colorSelectorMenuVisible = true;
    }
    menu.showColorList = function () {
        menu.colorList.addClass("show");
    }

    menu.showLogoSelectorMenus = function () {
        mobile.hideMainMenu();
        menu.logoMenu.addClass("show");
        menu.logoList.addClass("show");
        menu.closeSelector.addClass("show");
        menu.updateCustomScrollbar();
        colorSelectorMenuVisible = false;
        logoSelectorMenuVisible = true;
    }

    setCustomScroller = function () {

        var scrollOptions = $.extend({}, { contentTouchScroll: 100, mouseWheelPixels: 50, scrollbarPosition: "outside", autoHideScrollbar: true, scrollInertia: 100, mouseWheel: { scrollAmount: 50 } });

        if (!nec.nexu.ui.isMobile()) {
            $(function () {
                //setup scroll regions
                menu.colorList.mCustomScrollbar(scrollOptions);
                menu.logoMenu.mCustomScrollbar(scrollOptions);
                menu.regionMenu.mCustomScrollbar(scrollOptions);
                menu.logoList.mCustomScrollbar(scrollOptions);
            });
        }
    }
    menu.setCustomScrollbar = setCustomScroller;
    menu.updateCustomScrollbar = function () {
        menu.logoMenu.mCustomScrollbar("update");
        menu.regionMenu.mCustomScrollbar("update");
        menu.logoList.mCustomScrollbar("update");
        menu.colorList.mCustomScrollbar("update");
    }

    menu.selectRegionFromKey = function (key, includeLinkedRegions) {

        includeLinkedRegions = includeLinkedRegions != null ? includeLinkedRegions : true;

        var isChecked = setCheckbox(key);

        if (includeLinkedRegions) {
            var checkedRegion = _.find(models.Product.Regions, function (obj) { return obj.Region.Code == key })
            var matchingRegions = _.filter(models.Product.Regions, function (obj) { return obj.Region.MatchingRegion == checkedRegion.Region.Id; });

            matchingRegions.forEach(function (r) {
                setCheckbox(r.Region.Code, isChecked);
            });

        }
        mobile.controls.RegionsTreeView.updateIndeterminate();
    }

    setCheckbox = function(key, checkIt)
    {
        var regiontree = mobile.controls.RegionsTreeView;
        var getitem = regiontree.dataSource.get(key);
        regiontree.findByUid(getitem.uid);
        var selectitem = regiontree.findByUid(getitem.uid);
        regiontree.select(selectitem);

        var checkbox = $(selectitem).find(":checkbox");
        //checkbox.click();
        //regiontree.expand(selectitem.parent());

        if (checkIt == null)  //determine what to do from current state
        {
            checkIt = !checkbox.prop("checked");//toggle it
        }

        if (checkIt) {
            checkbox.prop("checked", "checked");
        }
        else {
            checkbox.removeAttr("checked");        
	}

        return checkIt;
    }

    menu.getSelectedRegions = function () {

        return viewer.widget().getSelectedRegions();
    }

    // function that gathers IDs of checked nodes
    function checkedRegionsTreeViewNodeIds(nodes, checkedNodes, selectedIds) {
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].checked && !nodes[i].hasChildren) {
                selectedIds.push(nodes[i].id);
                checkedNodes.push(nodes[i]);
            }

            if (nodes[i].hasChildren) {
                checkedRegionsTreeViewNodeIds(nodes[i].children.view(), checkedNodes, selectedIds);
            }
        }
    }


    // show checked node IDs on datasource change
    menu.onRegionTreeViewCheck = function (e) {
        //console.log(e);

        //determine the one node that was checked?
        //if is top then have to get all chidl

        //console.log("onRegionTreeViewCheck called");
        var checkedNodes = [],
            selectedIds = [],
            treeView = mobile.controls.RegionsTreeView,
            message;

        checkedRegionsTreeViewNodeIds(treeView.dataSource.view(), checkedNodes, selectedIds);

        if (checkedNodes.length > 0) {
            message = "" + selectedIds.join(",");
        } else {
            message = "";
        }

        selectedRegions = checkedNodes;
        $("#regions-treeview-results").html(message);
        //console.log(selectedRegions);
        //console.log(selectedIds);
    }

    function onErrorShow(e) {
        if (e.sender.getNotifications().length == 1) {
            var element = e.element.parent(),
                eWidth = element.width(),
                eHeight = element.height(),
                wWidth = $(window).width(),
                wHeight = $(window).height(),
                newTop, newLeft;

            newLeft = Math.floor(wWidth / 2 - eWidth / 2);
            newTop = Math.floor(wHeight / 2 - eHeight / 2);

            e.element.parent().css({ top: newTop, left: newLeft });
        }
    }
    menu.isColorSelectorMenuVisible = function () {
        return colorSelectorMenuVisible;
    }

    menu.isLogoSelectorMenuVisible = function () {
        return logoSelectorMenuVisible;
    }

    menu.setColorSelectorVisibile = function (Visible) {
        if (Visible) {
            menu.showColorSelectorMenus();
            mobile.hideMainMenu();
        }
        else {
            //wipe out any selected regions
            menu.resetSelectedRegions();

            menu.hideSelectorMenus();
            //menu.hideColorSelectorMenus();
            mobile.showMainMenu();
        }
        colorSelectorMenuVisible = Visible;

        //check that cap fits properly.
        mobile.setImageScale();
    }

    menu.toggleZoom = function (i) {
        var icon = $(".zoom-button i");

        if (icon.hasClass("fa-expand")) {
            icon.removeClass("fa-expand").addClass("fa-compress");
            menu.zoomViewer();
        } else {
            icon.removeClass("fa-compress").addClass("fa-expand");
            menu.resetZoom();
        }
    }

    menu.zoomViewer = function (i) {
        $("#nexu-configurator-viewer").addClass("zoom");
        mobile.setImageScaleZoom();
    }

    menu.resetZoom = function (i) {
        $("#nexu-configurator-viewer").removeClass("zoom");
        mobile.setImageScale();
    }

    menu.setLogoSelectorVisibile = function (Visible) {
        if (Visible) {
            menu.showLogoSelectorMenus();
            mobile.hideMainMenu();
        }
        else {
            menu.hideSelectorMenus();

            //menu.hideLogoSelectorMenus();
            mobile.showMainMenu();
        }
        logoSelectorMenuVisible = Visible;

        //check that cap fits properly.
        mobile.setImageScale();
    }

    menu.resetSelectedRegions = function (i) {
        //console.log("resetSelectedRegions");
        menu.treeviewClearAll();
        viewer.widget().clearSelections();
    }

    menu.treeviewClearAll = function (i) {
        $("#regions-treeview .k-checkbox-wrapper input").prop("checked", false).trigger("change");
    }

    menu.expandAllRegions = function (i) {
        mobile.controls.RegionsTreeView.expand(".k-item");
    }

    menu.collapseAllRegions = function (i) {
        mobile.controls.RegionsTreeView.collapse(".k-item");
    }

    menu.resetAll = function () {
        var product = nec.nexu.moile.Product;
        moile.viewer.loadViews(product);
    }

    menu.changeViewSelectorIndex = function (changeBy) {
        var viewlist = $('.view-selector', '.mobile-view-selector').data("kendoListView");
        var max = viewlist.dataSource.data().length;
        var x = viewlist.select().index();
        var newX = viewlist.select().index() + changeBy;

        if (newX == max) {
            newX = 0;
        } else if (newX < 0) {
            newX = max - 1;
        }

        viewlist.select(viewlist.element.children()[newX]);
        viewlist.trigger("change");
    }

    menu.changeViewSelectorName = function (name) {
        var viewlist = $('.view-selector', '.mobile-view-selector').data("kendoListView");
        var children = viewlist.element.children();
        var currentIndex = viewlist.select().index();
        var matchIndex = -1;

        for (var x = 0; x < children.length; x++) {
            var dataitem = viewlist.dataSource.view()[x];
            //console.log(dataitem);

            if (dataitem.DisplayName == name) {
                matchIndex = x;
                break;
            };
        };

        if (matchIndex != -1 && matchIndex != currentIndex) {
            viewlist.select(viewlist.element.children()[matchIndex]);
            viewlist.trigger("change");
        }
    }

    menu.changeViewSelectorId = function (id) {
        var viewlist = $('.view-selector', '.mobile-view-selector').data("kendoListView");
        var children = viewlist.element.children();
        var currentIndex = viewlist.select().index();
        var matchIndex = -1;

        for (var x = 0; x < children.length; x++) {
            var dataitem = viewlist.dataSource.view()[x];
            //console.log(dataitem);

            if (dataitem.Id == id) {
                matchIndex = x;
                break;
            };
        };

        if (matchIndex != -1 && matchIndex != currentIndex) {
            viewlist.select(viewlist.element.children()[matchIndex]);
            viewlist.trigger("change");
        }
    }

    /* potentially can scrap */
    menu.changeRegionsDropdownIndex = function (changeBy) {
        var dropdownlist = $("#regions-dropdown").data("kendoDropDownList");
        var max = dropdownlist.dataSource.data().length;
        var x = dropdownlist.select()
        var newX = dropdownlist.select() + changeBy;

        if (newX == max) {
            newX = 0;
        } else if (newX < 0) {
            newX = max - 1;
        }

        dropdownlist.select(newX);
        dropdownlist.trigger("change");
    }

    menu.syncRegionsControls = function () {
        var dropdownlist = $("#regions-dropdown").data("kendoDropDownList");
        var regionlist = $('#regions-menu').data("kendoListView");

        var matchId = dropdownlist.value();
        var children = regionlist.element.children();

        for (var x = 0; x < children.length; x++) {
            var listId = regionlist.dataSource.view()[x].Id;

            if (listId == matchId) {
                regionlist.clearSelection();
                regionlist.select(children[x]);
                break;
            }
        }
    }
    /*end potentially can scrap*/
    /*end public methods*/

    /**
     * private methods
     *****************
     */


    colorRegion = function (e) {
        var color = e.data;
        //color regions
        var selectedRegions = menu.getSelectedRegions();

        if (selectedRegions.length == 0) {
            //display 
            //title: "Nothing Selected to Color!",
            //message: "Please select something on the cap and choose a color again.",
            //dismissText: "DISMISS <i class='fa fa-chevron-right' aria-hidden='true'></i>"
            nec.nexu.mobile.controls.notification.show({
                message: 'Nothing Selected to Color! Please select something on the cap and choose a color again.',
                showDecline: false,
                showConfirm: false,
                showClose: true,
                confirmCallback: function () {
                    //nothing
                }
            });

        } else {
            for (var i = 0; i < selectedRegions.length; i++) {

                selectedRegions[i].set('Material', color);
                selectedRegions[i].UserAssigned = true;
            }

        }

    }


    bindMenus = function () {

        //Hamburger menu - Not in use at this time.
        var viewModel = kendo.observable({
            open: function (e) {
                barMenu.addClass("open");
            },
            close: function (e) {
                barMenu.removeClass("open")
            },
            test: function (e) {
                alert("this is a test");
                this.close();
            },
            exit: function (e) {
                var answer = confirm("Are you sure you are done");
                if (answer) {
                    $("body").removeClass("nexuapp");
                    $("html").removeClass("appmode");
                    this.close();
                }
            }
        });
        kendo.bind(barMenu, viewModel);

        /*Left buttons*/
        var mainbuttonsMenuLeftViewModel = kendo.observable({
            showLogos: function (e) { menu.setLogoSelectorVisibile(true); },
            showColors: function (e) { menu.setColorSelectorVisibile(true); }
        });
        kendo.bind($("#buttonsMenuLeft"), mainbuttonsMenuLeftViewModel);

        /*Right buttons*/
        var mainbuttonsMenuRightViewModel = kendo.observable({
            quit: function (e) {

                nec.nexu.mobile.controls.notification.show({
                    message: 'Are you sure you want to exit?  Your changes will be lost.',
                    showDecline: true,
                    showConfirm: true,
                    confirmCallback: function () {
                        $("body").removeClass("nexuapp");
                        $(".app-wrapper").removeClass("loaded");
                        $("html").removeClass("appmode");
                        mobile.loader.show();
                        $('svg').fadeOut(500);
                    }
                });
            },
            ConfirmSave: nec.nexu.events.ConfirmSave
        });
        kendo.bind($("#buttonsMenuRight"), mainbuttonsMenuRightViewModel);
    }

    /* end private methods */


    $('.selector-menu-closer').click(function () {
        menu.resetView();
        menu.resetSelectedRegions();
        //clear selections
        mobile.resetMenu();
        //reset logo ul list
        var logoList = $(".mobile-logo-list").data("kendoListView");
        logoList.setDataSource(kendo.data.DataSource.create([]));
    });


    /******************************/
    //add to global namespace
    window._nec_ = nec;

})(jQuery, kendo, _, window);