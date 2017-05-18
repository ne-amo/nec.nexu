(function ($) {

    function toggleFullScreen() {
        var doc = window.document;
        var docEl = doc.documentElement;

        var requestFullScreen = docEl.requestFullscreen || docEl.mozRequestFullScreen || docEl.webkitRequestFullScreen || docEl.msRequestFullscreen;
        var cancelFullScreen = doc.exitFullscreen || doc.mozCancelFullScreen || doc.webkitExitFullscreen || doc.msExitFullscreen;

        var elem = $('.mobile-container')[0];





        if (!doc.fullscreenElement && !doc.mozFullScreenElement && !doc.webkitFullscreenElement && !doc.msFullscreenElement) {
            requestFullScreen.call();
            //$('.mobile-container')[0]
        }
        else {
            cancelFullScreen.call(doc);
        }
    }
    $('.btn-fullscreen').kendoTouch({
        tap: function (e) {
            toggleFullScreen();
        }
    })



    var nec = window._nec_,
        data = nec.nexu.data,
        config = nec.nexu.config,
        events = nec.nexu.events,
        ui = nec.nexu.ui,
        k_Ds = kendo.data.DataSource;

    function _getGlobalObjectFromString(str) {
        var a = str.split('.'), obj = window;
        while (a.length > 0) {
            obj = obj[a.shift()];
        }
        return obj;
    }


    var pId = 0;
    var directLink = 0;
    var inspiredBy = 0;
    if (window.urlParams && window.urlParams.B) {
        pId = window.urlParams.B;
    }
    if (window.urlParams && window.urlParams.DL) {
        ui.DirectLink = window.urlParams.DL;
    }
    if (window.urlParams && window.urlParams.I) {
        ui.InspiredBy = window.urlParams.I;
    }
    if (window.urlParams && window.urlParams.m == ui.Modes.EDIT) {
        ui.Mode = ui.Modes.EDIT;
    }
    else {
        ui.Mode = ui.Modes.CREATE;
    }
    //ui.ProductId = pId;

    /* Mobile */
    (function (window) {
        var nec = window._nec_ || {};
        nec.nexu = nec.nexu || {};
        nec.nexu.mobile = nec.nexu.mobile || {};
        nec.nexu.models = nec.nexu.models || {};
        nec.nexu.ui = nec.nexu.ui || {};
        nec.nexu.data = nec.nexu.data || {};
        nec.nexu.data.rules = nec.nexu.data.rules || {};
        nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
        nec.nexu.mobile.controls.menu = nec.nexu.mobile.controls.menu || {};
        nec.nexu.mobile.controls.notification = nec.nexu.mobile.controls.notification || {};

        var mobile = nec.nexu.mobile, CHANGE = 'change', EMPTY = '', viewer = nec.nexu.mobile.viewer, menu = nec.nexu.mobile.controls.menu, models = nec.nexu.models;

        var drawer = $('.mobile-drawer'), configurator = $('.mobile-configurator'), container = $('.mobile-container'),
            collapsablemenu = $('.collapsable-menu'), themainmenu = $('#left-column, #right-column')
        menus = function () {
            var main = $('div[data-menu="main"]', drawer),
                header = $('.header', drawer),
                sub = $('.sub-header', drawer),
                options = $('div[data-menu="options"]', drawer).hide(),
                purchase = $('div[data-menu="purchase"]', drawer).hide();
            var blob = { main: main, options: options, purchase: purchase, header: header, sub: sub };
            show = function (str) {
                main.hide();
                sub.hide();
                options.hide();
                purchase.hide();
                var t = blob[str]
                if (t)
                    t.show();
                if (str == "main") {
                    sub.show();
                }
            }
            return $.extend({ show: show }, blob);
        }();

        // Set the height to the height of the view window
        //BT REMOVE
        //container.height(document.documentElement.clientHeight);
        //drawer.height(document.documentElement.clientHeight);

        /* View Model */
        var initialModel = {
            CurrentHeader: EMPTY,
            MobileHelperName: EMPTY,
            StyleDescription: EMPTY,
            PriceDescription: EMPTY,
            ShowHelper: false
        };
        mobile.model = kendo.observable(initialModel);

        /* Data Sources */

        /* Controls */
        var _makeMenu = function (selector, controlName) {
            var element = $(selector),
                template = kendo.template(element.first().html()),
                menu = element.kendoListView({
                    data: [],
                    selectable: 'single',
                    template: template
                }).data('kendoListView');
            mobile.controls = mobile.controls || {};
            mobile.controls[controlName] = menu;
        };

        var regionTreeView = $("#regions-treeview");

        mobile.loader = (function () {

            var el = $('#nxLoader');
            function show() {
                kendo.fx(el).zoom('in').play();
            }
            function hide() {
                kendo.fx(el).zoom('out').play();
            }

            return {

                show: show,
                hide: hide
            }

        })();

        mobile.isLandscape = function () {

            var landscape = false;
            if ($('html').hasClass('bp-portrait')) {
                $('.nx-mobile-portrait').removeClass('hidden');
                $('.nx-mobile-landscape').addClass('hidden');
                menu.resetView();

                //$("body").addClass("vertical");
            }
            else {
                $('.nx-mobile-portrait').addClass('hidden');
                $('.nx-mobile-landscape').removeClass('hidden');
                //$("body").removeClass("vertical");
                landscape = true;
            }
            window.setTimeout('_nec_.nexu.mobile.setImageScale();', 303);
            return landscape;
        }

        sendSelectedRegion = function (checkedItem) {
            var r = _.find(models.Product.Regions, function (o) { return o.Region.Code == checkedItem.id });
            if (r) {
                if (checkedItem.checked) {
                    viewer.widget().selectRegion(r.Region.Code, { clearSelections: false });
                }
                else {
                    viewer.widget().deselectSingle(r.Region.Code);
                }
            }
            //now check children
            if (checkedItem.hasChildren) {
                checkedItem.children.data().forEach(function (i) {
                    sendSelectedRegion(i);
                });
            }
        }

        expandCollapseTop = function (node) {
            //write code to expand or collapse all at level 2 //opposite of what you'd think
            if (node.hasClass("k-i-expand")) {
                _nec_.nexu.mobile.controls.menu.expandAllRegions();
                node.removeClass("k-i-expand").addClass("k-i-collapse");
            } else {
                _nec_.nexu.mobile.controls.menu.collapseAllRegions();
                node.removeClass("k-i-collapse").addClass("k-i-expand");
            }
        }

        mobile.init = function () {
            //move app to top of dom
            $(".app-wrapper").prependTo("body");

            mobile.isLandscape();

            if (nec.nexu.mobile.controls.notification.init) {
                nec.nexu.mobile.controls.notification.init();
            }

            mobile.controls.RegionsTreeView = regionTreeView.kendoTreeView({
                checkboxes: {
                    checkChildren: true
                },

                check: function (e) {
                    //e.preventDefault();
                    menu.onRegionTreeViewCheck();
                    var dataItem = regionTreeView.data('kendoTreeView').dataItem(e.node);
                    //console.log("checking :" + dataItem.id);

                    sendSelectedRegion(dataItem);
                },

                collapse: function (e) {
                    //prevent collapsing the root; code is reversing the out of the box functionality
                    if (e.node && e.node.innerHTML.indexOf("root") >= 0) {
                        e.preventDefault();

                        expandCollapseTop(e.sender.element.find(".k-top.k-bot .k-icon"));
                    }
                },

                expand: function (e) {
                    //prevent expanding the root; code is reversing the out of the box functionality
                    if (e.node && e.node.innerHTML.indexOf("root") >= 0) {
                        e.preventDefault();

                        expandCollapseTop(e.sender.element.find(".k-top.k-bot .k-icon"));
                    }
                },

                select: function (e) {
                    //console.log("treeview select event fired");
                    e.preventDefault();

                    //select checkbox
                    var checkbox = $(e.node).find(":checkbox");
                    checkbox.prop("checked", !checkbox.prop("checked")).trigger("change");
                },

                dataBound: function (e) {
                    e.sender.element.find("span.regiongroup").parent().addClass("regiongroup-label");
                    e.sender.element.find("span.regiongroup").parent().parent().parent().addClass("regiongroup-parent");
                    //console.log(e);

                    if (e.node && e.node.context.innerHTML.indexOf("root") >= 0) {
                        //e.sender.element.find(".k-top.k-bot .k-minus").removeClass("k-minus").addClass("k-plus");
                        e.sender.element.find(".k-top.k-bot .k-i-collapse").removeClass("k-i-collapse").addClass("k-i-expand");
                    }
                },

                dataSource: []
            }).data('kendoTreeView');

            mobile.controls.RegionDropdown = $('#regions-dropdown').kendoDropDownList({
                data: [],
                dataTextField: "DisplayName",
                dataValueField: "Id",
                dataSource: [],
                change: function (e) {
                    menu.syncRegionsControls();
                }
            }).data('kendoDropDownList');

            mobile.controls.ViewSelector = $('.view-selector', '.mobile-view-selector').kendoListView({
                data: [],
                selectable: 'single',
                template: '<div class="view-angle"><!--<img src="' + config.IMGPATH + '/#:ThumbnailPath#"/>-->#:DisplayName#</div>',
                change: nec.nexu.events.ViewerSelect,
                dataBound: function () {
                    this.select(this.element.children().first());
                    console.log("view selector bound");
                }

            }).data('kendoListView');

            mobile.controls.background = $('.changable-background');

            var loginDiv = document.createElement('div');
            loginDiv.id = "nx-login-window";
            document.body.appendChild(loginDiv);
            mobile.controls.login = $(loginDiv).kendoWindow({
                content: '/Login.aspx',
                title: 'New Era by You Login',
                iframe: true,
                draggable: false,
                height: document.documentElement.clientHeight,
                visible: false,
                modal: true,
                open: function (e) {
                    e.sender.element.closest('.k-window').addClass('force-border');
                    e.sender.maximize();
                },
                close: function (e) {
                    events.getUser(events.setUserId);
                }
            }).data('kendoWindow');


            data.ColorLists.read().done(function (e) {
                data.ColorLists.add({ DisplayName: 'Recent Colors', Id: -2, Colors: [] });

                //build color picker on initialize so wait for colors to be loaded
                mobile.controls.menu.init();
            });

            _makeMenu('.header', 'HeaderMenu');
            _makeMenu('.sub-header', 'SecondaryMenu');
            _makeMenu('.main-menu', 'MainMenu');
            nec.nexu.mobile.controls.HeaderMenu.setDataSource(k_Ds.create([{ DisplayName: 'Logos', SortOrder: 'hidden' }]));
            _bindEvents();
            nec.nexu.data.Threads.read();


            mobile.controls.colorgrabber.init('.mobile-colorgrabwindow');

            mobile.controls.summary.init();

            mobile.controls.backgroundwindow.init({
                data: data.Background,
                change: function (e) {
                    //e.sender.element.closest('.k-window-content').data('kendoWindow').close();
                    var item = e.sender.dataItem(e.sender.select());
                    if (!item)
                        return;
                    ui.Background = item;
                    mobile.controls.background.css('background-image', 'url(' + config.ASSETS + item.FilePath + ')');
                    //$('body').css('background-image', 'url(' + config.ASSETS + item.FilePath + ')');
                },
                close: function (e) {
                    mobile.resetMenu();
                }
            });

            mobile.controls.logowindow.init({
                change: function (e) {
                    //e.sender.element.closest('.k-window-content').data('kendoWindow').close();
                    mobile.Handlers.LogoForward(e);
                },
                close: function (e) {
                    mobile.resetMenu();
                }
            });


            window.scrollTo(0, $('.mobile-container').offset().top);

        };//end init

        mobile.showMainMenu = function () {
            //drawer.removeClass('hide');
            themainmenu.removeClass('hide');
            collapsablemenu.removeClass('hide');
        }

        mobile.hideMainMenu = function () {
            //drawer.addClass('hide');
            themainmenu.addClass('hide');
            collapsablemenu.addClass('hide');
        }

        mobile.resetMenu = function (baseItems) {
            var msg = nec.nexu.models;

            nec.nexu.mobile.controls.HeaderMenu.setDataSource(k_Ds.create([{ DisplayName: 'Logos', SortOrder: 'hidden' }]));
            mobile.controls.HeaderMenu.element.find('.col-xs-9').removeClass('col-xs-9').addClass('col-xs-12');

            nec.nexu.mobile.controls.HeaderMenu.setDataSource(k_Ds.create([{ DisplayName: 'Logos', SortOrder: 'hidden' }]));
            mobile.controls.HeaderMenu.element.find('.col-xs-9').removeClass('col-xs-9').addClass('col-xs-12');

            var parentItem = { Id: -1, SortOrder: '', DisplayName: 'Logos', Label: 'Choose embroidery', Locations: msg.Template.DesignLocations };

            var primaryNeeded = nec.nexu.data.rules.isPrimaryLocationNeeded(msg);


            ////logo locations at start
            var items = $.map(
                            _.where(msg.Template.DesignLocations, { Active: true}),
                            function (item) {
                                if (primaryNeeded) {
                                    if (item.IsPrimary) {
                                        return $.extend({}, item, { getParent: function () { return parentItem; } });
                                    }
                                }
                                else {
                                    return $.extend({}, item, { getParent: function () { return parentItem; } });
                                }
                            }
                        );

            _tileize(false);
            mobile.model.set('MobileHelperName', EMPTY);
            mobile.model.set('ShowHelper', false);
            mobile.controls.SecondaryMenu.setDataSource(k_Ds.create([]));
            mobile.controls.MainMenu.setDataSource(k_Ds.create(items));

            mobile.controls.MainMenu.unbind(CHANGE).bind(CHANGE, mobile.Handlers.LocationForward); //logo locations
        }
        mobile.setImageScale = function () {
            if ($("#nexu-configurator-viewer").hasClass("zoom")) {
                mobile.setImageScaleZoom();
            } else {
                if (viewer.widget() != undefined) {
                    viewer.widget().scale({ maxHeight: configurator.height() * .8, maxWidth: configurator.width() * .75 }).center();
                    console.log("rescale image");
                }
            }

        }

        mobile.setImageScaleZoom = function () {
            if (viewer.widget() != undefined) {
                viewer.widget().scale({ maxHeight: configurator.height() * 2.5, maxWidth: configurator.width() * 2.5 }).center();
            }
        }

        function _tileize(a) {
            if (a)
                mobile.controls.MainMenu.element.addClass('tiled');
            else
                mobile.controls.MainMenu.element.removeClass('tiled');
        }

        /* Event Binding */
        var _bindEvents = function () {
            mobile.controls.HeaderMenu.element.on('click', function (e) {
                mobile.resetMenu();
                return false;
            });
            mobile.controls.MainMenu.bind(CHANGE, mobile.Handlers.RegionGroupForward);
            mobile.controls.MainMenu.bind('dataBound', function (evt) {
            });
            mobile.controls.SecondaryMenu.bind(CHANGE, mobile.Handlers.GenericBack);

            data.Background.one(CHANGE, events.BackgroundInit);
            data.Background.bind(CHANGE, events.BackgroundChange);
            data.Background.read();
        }

        function _changeDrawer(t, dir) {
            if (dir === 'right' || (drawer.hasClass('drawer-open') && drawer.data('currenttarget') === t)) {
                configurator.addClass('col-xs-12').removeClass('col-xs-8')
                drawer.removeClass('drawer-open').hide();

                /* set the viewer width to the width of the window */
                viewer.scale({ width: configurator.width() });
            }
            else {
                tar = t || drawer.data('currenttarget');
                configurator.addClass('col-xs-8').removeClass('col-xs-12')
                drawer.addClass('drawer-open').show().data('currenttarget', tar);

                /* set the viewer width to the width of the column */
                viewer.scale({ width: configurator.width() });
            }
        }
        $('.btn-drawer').on(CLICK, function (e) {
            var m = $(e.target).data('targetmenu');
            //_changeDrawer(m);
            menus.show(m);
            return false;
        });

        $("#configurator-container").kendoTouch({
            enableSwipe: true,
            minXDelta: 150,
            maxDuration: 250,
            multiTouch: true,
            swipe: function (e) {
                var dir = e.direction;
                if (dir == 'right' || dir == 'left') {
                    //_changeDrawer(null, dir);
                    var changeBy = 1;
                    if (dir == 'left')
                    {
                        changeBy = -1;
                    }

                    //alert("change view");
                    nec.nexu.mobile.controls.menu.changeViewSelectorIndex(changeBy);
                }
            }
        });



        /* Data Bind */
        $('.mobile-bound-item').each(function (i, el) {
            var target = $(el).data('model'), model = _getGlobalObjectFromString(target)
            if (target && target.trim())
                kendo.bind(el, model);
        });

    })(window);


    function bindHeight() {
        //var height = Math.min(window.screen.availHeight, window.screen.availWidth);//(screen.Ratio < 1) ? window.screen.availWidth : window.screen.availHeight
        //var perc = 0.9;
        //$('html').attr('nx-height', height);
        //$('*[class*="bound-height"]').each(function (i, o) {
        //    var x = $(o);
        //    if (x.hasClass('bound-height=max')) {
        //        x.css('max-height', height*perc + 'px');
        //    }
        //    else {
        //        x.height(height*perc);
        //    }
        //});

    }

    window._nec_ = nec;


    window.onresize = function (e) {
        breakPoints.Set();
        bindHeight();
        mobile.isLandscape();
        mobile.setImageScale();

    }



    //nec.nexu._disableScroll();

    $(".go-button").click(function (e) {

        var productId = 94; // default ID
        productId = $.isNumeric($(this).data("pid")) ? $(this).data("pid") : productId;
        activateApp();
        nec.nexu.mobile.load(productId);
    });

    activateApp = function () {
        $("body").addClass("nexuapp");
        $("html").addClass("appmode");
    };

    nec.nexu.mobile.init();

    nec.nexu.mobile.load = function (productId) {

        ui.ProductId = productId;

        breakPoints.Set();
        bindHeight();

        //setup style and price for binding
        kendo.bind('#product-details', mobile.model);

        /* Load the product */
        data.LoadProduct(productId)
            .then(events.ProductLoad)
            .then(events.ProductDone)
            .then(function () { kendo.bind('#product-details', _nec_.nexu.mobile.model); $(".app-wrapper").addClass("loaded") })
            .done(events.DeepLinkDone);
    }



    //if we have an id to load from URL then run it
    var pid = getQueryStrings()['b'];
    if ($.isNumeric(pid))
    {
        activateApp();
        nec.nexu.mobile.load(pid);
    }


})(jQuery);
