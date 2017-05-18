
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

function mobileFunction() {

    var drawer = $('#nexu-right-drawer'), CLICK = window.CLICK, scroller,
        el_menus = $('*[class^=nav-menu]', drawer), el_options = $('*[class^=nav-options]', drawer), maxHeight = 480;

    var colorLists = _nexu_.ColorPicker.getData().view();

    var kTemplate_Group = kendo.template($('#mobile_template_group').html());
    var kTemplate_Background = kendo.template($('#mobile_template_bg').html());
    var kTemplate_Layer = kendo.template($('#mobile_template_layer').html());
    var kTemplate_Region = kendo.template('<li><div><a class="nx-prev icon icon-20w-close mob-icon" href="\\#"></a> <a href="\\#" data-item-id="#= data.Id #" data-nav-type="region" class="nx-item nx-region">#: data.DisplayName #</a></div><ul class="nx-sub"></ul></li>');
    var kTemplate_ColorList = kendo.template($('#mobile_template_colorlist').html());
    var kTemplate_Color = kendo.template($('#mobile_template_basic').html());
    var kTemplate_Thumb = kendo.template($('#mobile_template_mob_thumbnail').html());
    var template = _nexu_.Template, groups = $.map(template.RegionGroups, function (obj) {
        if(obj.Id != -1)
            return $.extend(obj, { ClassId: 'nx-grouplist' });
        else
            return $.extend(obj, { ClassId: 'nx-logos' });
    }), html_ColorList = kendo.render(kTemplate_ColorList, colorLists);

    function getColorLists() {
        return _nexu_.ColorPicker.getData().view();
    }
    function getColorListHtml() {
        return kendo.render(kTemplate_ColorList, getColorLists());
    }

    var hierarchyBase = {
        transport: {
            read: {
                url: window.SVCURL + 'hierarchies/',
                type: 'GET',
                contentType: "application/json",
                dataType: "json"
            },
            parameterMap: function (model) {
                var id = 0,
                    location = 0,
                    template = 0;
                if (model && model.Id) {
                    id = model.Id;
                }
                if (_nexu_.Selected.get('Location.Id')) {
                    location = _nexu_.Selected.get('Location.Id');
                }
                if (_nexu_.Template.Id) {
                    template = _nexu_.Template.Id;
                }
                return { parentId: id, locationId: location, templateId: template };
            }
        },
        schema: {
            model:{ id: "Id" },
            parse: function (data) {
                return data;
            },
            data: function (data) {
                return data;
            },
            total: function (data) {
                return data.length;
            }
        }
    };
    var hierarchyFirst = new kendo.data.DataSource(
            $.extend(true,{},hierarchyBase,{
                schema: {
                    data: function (data) {
                        return $.map(data, function (obj) {
                            return $.extend({}, obj, { ClassId: 'nx-hierarchy-1' });
                        });
                    }
                }
            })
        );
    var hierarchySecond = new kendo.data.DataSource(
            $.extend(true,{}, hierarchyBase, {
                schema: {
                    data: function (data) {
                        return $.map(data, function (obj) {
                            return $.extend({}, obj, { ClassId: 'nx-hierarchy-2' });
                        });
                    }
                }
            })
        );

    var SELECTED = 'nx-selected';
    function scrollTop() {
        scroller.scrollTo(0, 0);
    }
    function navigateDown(li) {
        var sub = $('ul.nx-sub', li).first(), container = li.closest('ul');
        sub.show();
        container.children().not(li).hide();
        li.addClass(SELECTED);
    }
    function navigateUp(li) {
        var sub = $('ul.nx-sub', li).first(), container = li.closest('ul');
        sub.hide().empty();
        $('.nx-label').text('');
        container.children().not(li).show();
        $('.' + SELECTED, container).removeClass(SELECTED);
        scrollTop();
    }

    function dsLocations() {
        var d = nexu_DesignLocationSelector.GetDataSource(true).view();
        return $.map(d, function (locat) {
            return {
                Id:locat.Id,
                DisplayName: locat.DisplayName,
                ClassId: 'nx-location'
            };
        });
    }
    function dsLogoOptions(location) {
        var usedLocation = _nexu_.LocationMenu.IsLocationUsedById(location);
        if (usedLocation){
            var arr = [{ DisplayName: 'Edit', Id: usedLocation.Id, ClassId: 'nx-logo-edit' }];
            if (usedLocation.Removable) 
                arr.push({ DisplayName: 'Remove', Id: usedLocation.Id, ClassId: 'nx-logo-remove' });
            return arr;
        }
        else {
            /* go to hierarchy? */
            return hierarchyFirst;
        }
    }
    function processLocationClick(id) {

    }
    function applyColorway(sub, colorway) {
         var dsOptions = $.map(_nexu_.Selected.Logo.DesignColorway.Layers, function (a, B) {
            if (a.Recolorable) {
                return $.extend(a, { ClassId: 'nx-layer' });
            }
        });
        sub.kendoListView({
            dataSource: new kendo.data.DataSource({
                data: dsOptions,
                schema: {
                    model: { id: 'ImageDataId' }
                }
            }),
            template: kTemplate_Layer
        });
    }

    function processThread(layer,color) {
        var layer = _.findWhere(_nexu_.Selected.Logo.DesignColorway.Layers, { ImageDataId: layer.ImageDataId });
        if (layer)
            layer.set('Color',color);
    }
    function processMaterial(color) {
        _nexu_.ColorPicker.ChangeColor(color);
        return false;
    }
    function processRegion(item) {
        _nexu_.Selected.set('ChangeAll', false);
        _nexu_.Selected.set('Region', item);
    }
    function processGroupAndMatch(group) {
        _nexu_.Display.set('ChangeAll', group.AllowRecolorAll);
        _nexu_.Display.set('MatchToFabric', group.AllowMatching);
        if (group.ForceGroup) {
            _nexu_.Selected.set('ForcedGroup', $.map(group.Regions, function (obj) { return obj.Id }));
        }
        else {
            _nexu_.Selected.set('ForcedGroup', null);
        }
    }

    var listContainer = $('#nx_mobileList');

    //var BackgroundDataItem = {
    //    DisplayName: 'Background',
    //    Id: -2,
    //    ClassId: 'nx-background'
    //};
    //var ColorGrabberDataItem = {
    //    DisplayName: 'Color Grabber',
    //    Id: -3,
    //    ClassId: 'nx-colorgrabber'
    //};
    //groups.push(BackgroundDataItem);
    //groups.push(ColorGrabberDataItem);

    listContainer.html(kendo.render(kTemplate_Group, groups));

    function processGroupNavigation(li) {
        var li = $(li), sub = $('ul.nx-sub', li), id = parseInt($('a.nx-item', li).data('itemId'));
        if (!id || id < 1) {
            if (id == -1) {

            }
            return;
        }
        var html = getColorListHtml();
        var group = _.findWhere(groups, { Id: id });
        if (group) {
            if (group.length == 1 || group.ForceGroup) {
                /* Add Color Lists */
                sub.html(html)//html_ColorList);
            }
            else {
                sub.html(kendo.render(kTemplate_Region, group.Regions));
                sub.children('li').each(function () {
                    /* Add Color Lists */
                    var sub = $('ul.nx-sub', this);
                    sub.html(html);//html_ColorList);
                });
            }
            if (group.AllowMatching) {
                /* Add the option to match fabric */
                sub.append(kendo.render(kTemplate_Group, [{ DisplayName: 'Match To Fabric', Id: 0, ClassId: 'nx-matchFabric' }]));
            }
        }
    }

    listContainer.children('li').each(function () {
        processGroupNavigation(this);
    })
    .on(CLICK, 'a.nx-matchFabric', function (e) {
        /* Handle match Click */
        _nexu_.ColorPicker.MatchFabrics();
        return false;
    })
    .on(CLICK, 'a.nx-colorlist', function (e) {
        /* Handle ColorList Click */
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li), lists = getColorLists(),
        colors = $.map(_.findWhere(lists/*colorLists*/, { Id: id }).Colors, function (a) {
            return $.extend(a, { ClassId: 'nx-colors' });
        });
        sub.html(kendo.render(kTemplate_Color, colors));
        navigateDown(li);
        $('.nx-label', li).text("Select a color...");
        return false;
    })
    .on(CLICK, 'a.nx-grouplist', function (e) {
        var li = $(e.target).closest('li');
        var id = parseInt($('a.nx-item', li).data('itemId')),
            group = _.findWhere(groups, { Id: id });
        /* set grouping and match settings */
        processGroupAndMatch(group);
        processGroupNavigation(li);
        $('ul.nx-sub', listContainer).hide();
        navigateDown(li);
        scrollTop();
        return false;
    })
    .on(CLICK, 'a.nx-region', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId'));
        navigateDown(li);
        var region = _.find(_nexu_.Product.Regions, function (fr) {
            return fr.Region.Id == id;
        });
        processRegion(region);
        scrollTop();
        return false;
    })
    .on(CLICK, 'a.nx-prev', function (e) {
        var li = $(e.target).closest('li'), sub = $('ul.nx-sub', li).first(), container = li.closest('ul');
        navigateUp(li);
        return false;
    })
    .on(CLICK, '.nx-colors div', function (e) {
        var li = $(e.target).closest('li'), parent = li.closest('ul'), container = parent.closest('li');
        var listId = parseInt($('a.nx-item', container).data('itemId')), itemId = parseInt(li.data('itemId'));
        var color = _.findWhere(_.findWhere(colorLists, { Id: listId }).Colors, { Id: itemId });
        $('.nx-label', container).text(color.DisplayName);
        processMaterial(color);
        return false;
    })
    .on(CLICK, 'a.nx-logos', function (e) {
        var li = $(e.target).closest('li'), sub = $('ul.nx-sub', li).first(), container = li.closest('ul');
        /* Logos */
        sub.html(kendo.render(kTemplate_Group, dsLocations()));
        navigateDown(li);
        return false;
    })
    .on(CLICK, 'a.nx-background', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li);
        var ds = _nexu_.Background.getDataSource();
        sub.kendoListView({
            dataSource: ds,
            template: kTemplate_Background
        });
        navigateDown(li);
        return false;
    })
    .on(CLICK, 'a.nx-bg', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), container = li.closest('ul')
        var ds = container.data('kendoListView').dataSource;
        var item = ds.get(id);
        _nexu_.Background.change(item);
        return false;
    })
    .on(CLICK, 'a.nx-colorgrabber', function (e) {
        module_ColorGrabber.reset().setSize({
            height: document.documentElement.clientHeight,
            width: document.documentElement.clientWidth
        }).open().showStage(1);
        return false;
    })
    .on(CLICK, 'a.nx-location', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li).first();
        processLocationClick(id);
        nexu_DesignLocationSelector.ChangeById(id);
        var data = dsLogoOptions(id);
        sub.kendoListView({
            dataSource: data,
            template: kTemplate_Group,
            autoBind: true
        });
        navigateDown(li);
        return false;
    })
    .on(CLICK, 'a.nx-logo-edit', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li).first();
        var code = _nexu_.Selected.get('Logo.Location.Code');
        var logo = _.find(_nexu_.Product.Logos, function (o, i) { return o.Location.Code == code });
        var selColorway = _nexu_.Selected.get('Logo.DesignColorway');
        if (!selColorway) {
            _nexu_.Selected.Logo.DesignColorway = logo.DesignColorway;
        }
        if (logo && logo.DesignColorway) {
            applyColorway(sub, _nexu_.Selected.Logo.DesignColorway);
        }
        return false;

    })
    .on(CLICK, 'a.nx-hierarchy-1', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li).first();
        sub.kendoListView({
            dataSource: hierarchySecond,
            template: kTemplate_Group,
            autoBind: false
        });
        hierarchySecond.read({ Id: id });
        navigateDown(li);
        return false;
    })
    .on(CLICK, 'a.nx-hierarchy-2', function (e) {
        var li = $(e.target).closest('li'), sub = $('ul.nx-sub', li).first(), container = li.closest('ul'), id = parseInt($('a.nx-item', li).data('itemId'));
        var ds = container.data('kendoListView').dataSource
        var item = ds.get(id);
        /* use existing logic */
        _nexu_.Handlers.HierarchyChange(null, item);
        /* modify the configuration of the existing datasource so that the class field and schema are set */
        var dsOptions = _nexu_.LogoCarousel.GetDataSource().options;
        $.extend(true, dsOptions,
        {
            schema: {
                model: { id: 'Id' },
                data: function (data) {
                    return $.map(data, function (d) {
                        return $.extend(d, { ClassId: 'nx-logo-thumbnail' });
                    });
                }
            }
        });
        sub.kendoListView({
            dataSource: new kendo.data.DataSource(dsOptions),
            template: kTemplate_Thumb
        });
        navigateDown(li);
        return false;
    })
    .on(CLICK, 'a.nx-logo-thumbnail', function (e) {
        var li = $(e.target).closest('li'), id = parseInt($('a.nx-item', li).data('itemId')), sub = $('ul.nx-sub', li).first(), container = li.closest('ul');
        var item = container.data('kendoListView').dataSource.get(id);
        /* use existing logic to set the design colorway */
        _nexu_.LogoCarousel.SetDesign(item);
        /* the colorway is loaded async from the server, we must subscribe to the controller to fire the layers load once*/
        _nexu_.Selected.one('change', function (e) {
            if (e.field == 'Logo.DesignColorway') {
                _nexu_.LogoRecolor.SetPrimary();
                applyColorway(sub, _nexu_.Selected.Logo.DesignColorway);
                navigateDown(li);
                scrollTop();
            }
        });
        return false;
    })
    .on(CLICK, 'a.nx-layer', function (e) {
        var li = $(e.target).closest('li'), sub = $('ul.nx-sub', li).first(), container = li.closest('ul'), id = parseInt($('a.nx-item', li).data('itemId'));
        var layer = container.data('kendoListView').dataSource.get(id);
        _nexu_.Selected.set('Layer', layer);
        var ds = _nexu_.LogoRecolor.GetDataSource(layer);
        ds.pageSize(1000);
        var d = ds.view();
        sub.kendoListView({
            dataSource: new kendo.data.DataSource({
                data: d,
                schema: {
                    model: { id: 'Id' },
                    data: function (d) {
                        return $.map(d, function (a) {
                            return $.extend(a, { ClassId: 'nx-thread' });
                        });
                    }
                }
            }),
            template: kTemplate_Color
        });
        navigateDown(li);
        li.addClass('removePadding');
        return false;
    })
    .on(CLICK, '.nx-thread div', function (e) {
        var li = $(e.target).closest('li'), parent = li.closest('ul'), container = parent.closest('li')
        , layerId = parseInt(container.find('a.nx-layer').data('itemId'))
        , layersDs = container.closest('ul').data('kendoListView').dataSource
        , layer = layersDs.get(layerId)
        , listId = parseInt($('a.nx-item', container).data('itemId')), itemId = parseInt(li.data('itemId'));
        var thread = parent.data('kendoListView').dataSource.get(itemId);
        processThread(layer, thread);
        return false;
    });

    var optionsContainer = $('#nx_optionsList')
    .on(CLICK, 'a.nx-save', function (e) {
        _nexu_.Handlers.Save.CLICK.call(this, e);
        return false;
    })
    .on(CLICK, 'a.nx-order', function (e) {
        _nexu_.Handlers.Save.Order.call(this, e);
        return false;
    })
    .on(CLICK, 'a.nx-publish', function (e) {
        window_Publish.Open();
        return false;
    })
    .on(CLICK, 'a.nx-summary', function (e) {
        module_summary.open();
        return false;
    })
    .on(CLICK, 'a.nx-share', function (e) {
        return false;
    })
    .on(CLICK, 'a.nx-help', function (e) {
        return false;
    })


    /* UI and Layout sizing */
    var h = Math.min(document.documentElement.clientHeight, maxHeight);
    $('.mobileWidth').width(document.documentElement.clientWidth);
    $('.mobileHeight').height(h+16);
    var newH = Math.floor(h * 0.8);
    var newW = Math.floor(document.documentElement.clientWidth * 0.8);
    $.extend(_nexu_.View.Controller.Widget().resize({ height: newH, width: newW }).options, { maxHeight: newH, maxWidth: newW });
    $('#nexu-configurator').addClass('mobile');
    window.isMobileViewport = true;
    module_ColorGrabber.setMobile();
    _nexu_.Display.IsMobile = true;
    el_options.hide();

    $('ul.nx-sub', listContainer).hide();


    drawer.appendTo('body').kendoMobileDrawer({
        container: $('#nexu-configurator-module'),
        position: 'right'
    });

    scroller = $('.km-content', drawer).data('kendoMobileScroller');


    var drawerWidget = drawer.data('kendoMobileDrawer');

    //drawerWidget.bind('beforeShow', function (e) {
    //    scroller.reset();
    //});

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

        },
        SetList: function (type, item) {

        },
        GetLogoCallOptions: function () {

        }
    }



};