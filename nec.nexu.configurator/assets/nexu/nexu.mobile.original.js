
window.MenuTypes = {
    ROOT: 0,
    GROUP: 1,
    REGION: 2,
    COLORLIST: 3,
    LOCATION: 4,
    HIERARCHY: 5,
    MATERIAL: 6,
    BACKGROUND: 7,
    LOGO: 8,
    LAYER: 9,
    THREAD: 10,
    RECOLORALL: 11,
    MATCH: 12,
    COLORWAY: 13,
    COLORGRAB: 14,
    SAVE: 15,
    SUMMARY: 16,
    PUBLISH: 17,
    HELP: 18,
    SHARE: 19,
    NONE: -1
}







var mobileFunction = function () {
    var drawer = $('#nexu-right-drawer'), elHeader = $('.nav-current a', drawer), lv, lvSub, lvColor, SELECTED = 'nexu-mob-selected',
        template = kendo.template($('#nexu-mobile-menu-template').html(), { useWithBlock: false }),
        el_menus = $('*[class^=nav-menu]', drawer), el_options = $('*[class^=nav-options]', drawer), maxHeight = 480;

    /* datasource to populate the options menu */
    var optionsArray = [
        {
            level: 0,
            DisplayName: 'Save',
            _type: MenuTypes.SAVE
        },
        {
            level: 0,
            DisplayName: 'Publish',
            _type: MenuTypes.PUBLISH
        },
        {
            level: 0,
            DisplayName: 'Summary',
            _type: MenuTypes.SUMMARY,
        },
        {
            level: 0,
            DisplayName: 'Share',
            _type: MenuTypes.SHARE,
        },
        {
            level: 0,
            DisplayName: 'Help',
            _type: MenuTypes.HELP,
        }
    ];

    lv_options = el_options.kendoListView({
        dataSource: optionsArray,
        template: template
    });

    var NavDirections = {
        NEXT: 0, PREV: 1
    };
    var type = window.MenuTypes;

    /* these functions are used to populated the datasource for the different configurator menus */
    function dsRoot() {
        var array = $.map(_nexu_.Template.RegionGroups.toJSON(), function (o) {
            if (o.Id == -1) {
                return {
                    level: 0,
                    DisplayName: o.DisplayName,
                    _type: MenuTypes.ROOT,
                    _childType: MenuTypes.LOCATION,
                    data: o
                };
            }
            return {
                level: 0,
                DisplayName: o.DisplayName,
                _type: MenuTypes.ROOT,
                _childType: MenuTypes.GROUP,
                data: o
            }
        });
        array.push({
            level: 0,
            DisplayName: 'BACKGROUND',
            _type: MenuTypes.ROOT,
            _childType: MenuTypes.BACKGROUND,
        });
        array.push({
            level: 0,
            DisplayName: 'COLORGRABBER',
            _type: MenuTypes.COLORGRAB,
            _childType: MenuTypes.ROOT,
        });
        return array;
    }
    function dsRegions(group, id) {
        var source = group;
        var array = $.map(source.Regions, function (r) {
            var region = _.find(_nexu_.Product.Regions, function (o) { return o.Region.Id == r.Id });
            return {
                level: 1,
                DisplayName: region.Region.DisplayName,
                code: region.Region.Code,
                _type: MenuTypes.REGION,
                _parentId: source.Id,
                _childType: MenuTypes.COLORLIST,
                data: region
            };
        });
        if (source.AllowRecolorAll) {
            array.push({
                level: 1,
                DisplayName: 'RECOLOR ALL',
                _type: MenuTypes.RECOLORALL,
                _parentType: MenuTypes.GROUP,
                _childType: MenuTypes.COLORLIST,
                data: { Material: { Hex: 'WHITE' } }
            });
        }
        if (source.AllMatching) {

        }
        return array;
    }
    function dsColorLists(region) {
        return $.map(_nexu_.ColorPicker.getData().data(), function (list) {
            return {
                level: 3,
                _type: MenuTypes.COLORLIST,
                DisplayName: list.DisplayName,
                _parentType: MenuTypes.REGION,
                _childType: MenuTypes.MATERIAL,
                data: list
            };
        });
    }
    function dsMaterials(list) {
        return $.map(list.Colors, function (color) {
            return { _type: MenuTypes.MATERIAL, DisplayName: color.DisplayName, level: 4, _childType: MenuTypes.NONE, data: color };
        });
    }
    function dsLocations() {
        var d = nexu_DesignLocationSelector.GetDataSource(true).view();
        return $.map(d, function (locat) {
            return {
                level: 1,
                _type: MenuTypes.LOCATION,
                DisplayName: locat.DisplayName,
                _parentType: MenuTypes.ROOT,
                _childType: MenuTypes.HIERARCHY,
                data: locat
            };
        });
    }
    function dsHierarchy(item) {
        var base = _nexu_.Hierarchy.ChildDataSource().options;

        if (item.parent()._type == MenuTypes.HIERARCHY) {
            /* parent hierarchy clicked */
            base.transport.parameterMap = function () {
                var id = item.Id,
                    location = 0,
                    template = 0;
                if (_nexu_.Selected.get('Location.Id')) {
                    location = _nexu_.Selected.get('Location.Id');
                }
                if (_nexu_.Template.Id) {
                    template = _nexu_.Template.Id;
                }
                return { parentId: id, locationId: location, templateId: template };
            }
        }
        base.schema.data = function (d) {
            return $.map(d, function (a) {
                return {
                    level: 2,
                    _type: MenuTypes.HIERARCHY,
                    DisplayName: a.DisplayName,
                    _parentType: MenuTypes.ROOT,
                    _childType: MenuTypes.HIERARCHY,
                    data: a
                };
            });
        }
        base.change = function (e) {
            if (e.sender.total() == 0) { /* No more children to navigate through, pass to logo selection */
                _nexu_.Hierarchy.element.trigger('select', item);
            }
        }
        return base;
    }
    function dsLayers(item) {
        return $.map(item.Layers, function (a, B) {
            if (a.Recolorable) {
                return {
                    _type: MenuTypes.LAYER,
                    DisplayName: a.DisplayName,
                    _parentType: MenuTypes.COLORWAY,
                    _childType: MenuTypes.THREAD,
                    data: a
                };
            }
        });
    }
    function dsThreads(item) {
        var ds = module_LogoRecolor.GetDataSource(item);
        ds.pageSize(200);
        return $.map(ds.view(), function (a) {
            return {
                _type: MenuTypes.THREAD,
                DisplayName: a.DisplayName,
                _parentType: MenuTypes.LAYER,
                _childType: MenuTypes.NONE,
                data: a
            };
        });
    }
    function dsBackground(item) {
        var ds = _nexu_.Background.getDataSource();
        return $.map(ds.view(), function (a) {
            return {
                _type: MenuTypes.BACKGROUND,
                DisplayName: a.DisplayName,
                _parentType: MenuTypes.ROOT,
                _childType: MenuTypes.NONE,
                data: a
            };
        });
    }
    function getDataItem(el, index) {
        var i = (typeof index === 'number') ? index : el.closest('li').index(),
            ds = el.closest('ul.k-listview').data('kendoListView').dataSource;
        return ds.at(i);
    }
    function setHeader(data, dir, ignore) {
        var navData = elHeader.data('nav'), name;
        if (dir == NavDirections.NEXT) {
            name = data.DisplayName
            navData.push(data);
        }
        if (dir == NavDirections.PREV) {
            navData.pop();
            name = navData[navData.length - 1].DisplayName;
        }
        if (!ignore) {
            elHeader.data('nav', navData);
        }
        elHeader.text(name);
    }

    function getDataSource(type, item) {
        /* gets the datasource for a nav menu based on type (usually _childType property) */
        switch (type) {
            case MenuTypes.ROOT:
                return dsRoot();
            case MenuTypes.GROUP:
                return dsRegions(item);
            case MenuTypes.REGION:
                return dsRegions(null, item._parentId);
            case MenuTypes.COLORLIST:
                return dsColorLists(item);
            case MenuTypes.LOCATION:
                return dsLocations();
            case MenuTypes.HIERARCHY:
                return dsHierarchy(item);
            case MenuTypes.MATERIAL:
                return dsMaterials(item);
            case MenuTypes.BACKGROUND:
                return dsBackground(item);
            case MenuTypes.LOGO:
                return null;
            case MenuTypes.LAYER:
                return dsLayers(item);
            case MenuTypes.THREAD:
                return dsThreads(item);
            case MenuTypes.MATCH:
                return dsThreads(item);
            case MenuTypes.COLORGRAB:
                return dsRoot();
            case MenuTypes.COLORWAY:
                return null;
        }
    }
    /* these functions handle specific logic for updating the viewmodel and performing business logic */
    function processRegion(item) {
        _nexu_.Selected.set('ChangeAll', false);
        _nexu_.Selected.set('Region', item);
    }
    function processLayer(item) {
        _nexu_.Selected.set('Layer', item.data);
    }
    function processThread(item) {
        _nexu_.Selected.set('Layer.Color', item.data);
    }
    function processMaterial(item) {
        var index = lv.element.data('selectedIndex');
        var di = getDataItem($(lv.element).children(':eq(' + index + ')'));
        lv.one('dataBound', function (e) {
            lv.element.children('li:eq(' + index + ')').addClass(SELECTED)
        }).one('itemChange', function (e) {
            e.sender.trigger('dataBound');
        });
        _nexu_.ColorPicker.ChangeColor(item.data);
    }
    function processColorGrabClick(item) {
        module_ColorGrabber.reset().setSize({
            height: document.documentElement.clientHeight,
            width: document.documentElement.clientWidth
        }).open().showStage(1);
    }
    function processBackground(item) {
        _nexu_.Background.change(item);
    }
    function processColorway(item) {
        lvSub.element.children('li').show(); /* make sure that when a logo is place the sub menu is visible */
        changeSubMenu(item);
        _nexu_.LogoWindow.Close();
    }
    function processItem(item) {
        /* this executes application specific changes/functions when an item is click from the mobile nav menu */
        switch (item._type) {
            case MenuTypes.ROOT:
                changeMainMenu(item);
                break;
            case MenuTypes.GROUP:
                changeMainMenu(item);
                break;
            case MenuTypes.REGION:
                processRegion(item.data);
                changeSubMenu(item);
                break;
            case MenuTypes.COLORLIST:
                changeColorMenu(item);
                break;
            case MenuTypes.LOCATION:
                nexu_DesignLocationSelector.Change(item.data);
                changeMainMenu(item);
                break;
            case MenuTypes.HIERARCHY:
                changeMainMenu(item);
                break;
            case MenuTypes.MATERIAL:
                processMaterial(item);
                break;
            case MenuTypes.BACKGROUND:
                processBackground(item.data);
                break;
            case MenuTypes.LOGO:
                break;
            case MenuTypes.LAYER:
                processLayer(item);
                changeColorMenu(item);
                break;
            case MenuTypes.THREAD:
                processThread(item);
                break;
            case MenuTypes.MATCH:
                break;
            case MenuTypes.COLORWAY:
                processColorway(item);
                break;
            case MenuTypes.RECOLORALL:
                _nexu_.Selected.set('ChangeAll', true);
                break;
            case MenuTypes.COLORGRAB:
                processColorGrabClick(item);
                break;
        }
    }
    function getIndexFromDataItem(item, source) {
        return source.indexOf(source.getByUid(item.uid));
    }

    /* these functions handle the navigation of the mobile configurator menu */
    function changeMainMenu(item) {
        var selectedIndex = getIndexFromDataItem(item, lv.dataSource);
        lv.element.data('selectedIndex', selectedIndex).children('li:not(.' + SELECTED + ')').hide();
        /* get new datasource to change list */
        if (item._childType != MenuTypes.NONE) {
            setHeader(item, NavDirections.NEXT);
            var newDs = kendo.data.DataSource.create(getDataSource(item._childType, item.data));
            lv.setDataSource(newDs);
        }
        else {
            setHeader(item, NavDirections.NEXT, true);
        }
    }
    function changeSubMenu(item) {
        lvSub.element.show();
        var selectedIndex = getIndexFromDataItem(item, lv.dataSource);
        lv.element.data('selectedIndex', selectedIndex).children('li:not(.' + SELECTED + ')').hide();
        if (item) {
            if (item._childType != MenuTypes.NONE) {
                var newDs = kendo.data.DataSource.create(getDataSource(item._childType, item.data));
                lvSub.setDataSource(newDs);
            }
        }
    }
    function changeColorMenu(item) {
        var selectedIndex = getIndexFromDataItem(item, lv.dataSource);
        lvSub.element.data('selectedIndex', selectedIndex).children('li:not(.' + SELECTED + ')').hide();
        lvColor.element.show();
        if (item) {
            if (item._childType != MenuTypes.NONE) {
                var newDs = kendo.data.DataSource.create(getDataSource(item._childType, item.data));
                lvColor.setDataSource(newDs);
            }
        }
    }
    function getLogoWindowOptions() {
        return {
            maxWidth: Infinity,
            maxHeight: Infinity,
            height: document.documentElement.clientHeight,
            width: document.documentElement.clientWidth,
            scroll: false
        };
    }

    /* UI and Layout sizing */
    var h = Math.min(document.documentElement.clientHeight, maxHeight);
    $('.mobileWidth').width(document.documentElement.clientWidth);
    $('.mobileHeight').height(h);
    var newH = Math.floor(h * 0.8);
    var newW = Math.floor(document.documentElement.clientWidth * 0.8);
    $.extend(_nexu_.View.Controller.Widget().resize({ height: newH, width: newW }).options, { maxHeight: newH, maxWidth: newW });
    $('#nexu-configurator').addClass('mobile');
    window.isMobileViewport = true;
    module_ColorGrabber.setMobile();
    _nexu_.Display.IsMobile = true;
    el_options.hide();


    /* Widget and event handler initialization */
    lv = drawer.appendTo('body').kendoMobileDrawer({
        container: $('#nexu-configurator-module'),
        position: 'right'
    })
        .on('click', 'a.nav-item', function (e) {
            var el = $(this), li = el.closest('li');
            li.closest('ul').children('li').removeClass(SELECTED).show();
            li.addClass(SELECTED)
            /* get the data item for the click link */
            var item = getDataItem($(this));
            /* change header and application */
            processItem(item);
            return false;
        })
        .on('click', 'h3 a', function (e) {
            /* header click to go back to root */
            lvSub.element.hide();
            lvColor.element.hide();
            elHeader.text('New Era By You');
            var newDs = kendo.data.DataSource.create(getDataSource(MenuTypes.ROOT));
            lv.setDataSource(newDs);
            //}
            return false;
        })
        .on(CLICK, 'a.mob-icon', function (e) {
            $(this).closest('.k-listview').children().removeClass(SELECTED).show();
            return false;
        })
        .find('ul.nav-menu').kendoListView({
            dataSource: dsRoot(),
            template: template,
        }).data('kendoListView');

    elHeader.data('nav', [{ DisplayName: 'New Era By You', _type: MenuTypes.NONE }]);
    lvSub = $('.nav-menu-sub').kendoListView({
        dataSource: new kendo.data.DataSource(),
        template: template,
    }).on(CLICK, 'a.mob-icon', function (e) {
        $(this).closest('.k-listview').children().removeClass(SELECTED).show();
        return false;
    }).data('kendoListView');

    lvColor = $('.nav-menu-colorlist').kendoListView({
        dataSource: new kendo.data.DataSource(),
        template: template,
    }).on(CLICK, 'a.mob-icon', function (e) {
        $(this).closest('.k-listview').children().removeClass(SELECTED).show();
        return false;
    }).data('kendoListView');

    var drawerWidget = drawer.data('kendoMobileDrawer');
    $('.nexu-mob-btn.menu-config').on(CLICK, function (e) {
        if (drawerWidget.visible) {
            if (el_menus.is(':visible')) {
                drawerWidget.hide();
            }
            else {
                el_menus.show();
                el_options.hide();
            }
        }
        else {
            el_options.hide();
            el_menus.show();
            drawerWidget.show();
        }
        return false;
    });
    $('.nexu-mob-btn.menu-options').on(CLICK, function (e) {
        if (drawerWidget.visible) {
            drawerWidget.hide();
        }
        else {
            el_options.show();
            el_menus.hide();
            drawerWidget.show();
        }
        return false;
    });

    return {
        SetLogo: function (type, item) {
            processItem(item);
            /* get new datasource to change list */
            if (item._childType != MenuTypes.NONE) {
                setHeader(item, NavDirections.NEXT);
                var newDs = kendo.data.DataSource.create(getDataSource(item._childType, item.data));
                lv.setDataSource(newDs);
            }
        },
        SetList: function (type, item) {
            processItem(item);
        },
        GetLogoCallOptions: function () {
            return getLogoWindowOptions();
        }
    }
};









