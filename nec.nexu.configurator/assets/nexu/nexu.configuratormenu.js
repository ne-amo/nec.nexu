(function () {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
        localText = (function () {
            return {
                display: 'Logos',
                label: 'Choose embroidery',
            }
        })(),
        LOGOS = { Id: -1, DisplayName: localText.display, Label: localText.label },
        CHANGE = "change",
        CLICK = 'click';
    var NexuConfiguratorMenu = Widget.extend({

        init: function (element, options) {
            var that = this;
            kendo.ui.Widget.fn.init.call(that, element, options);
            that.template = kendo.template(that.options.template || '#if(data.Id == -1){# <li class="local_text" data-tutorial="one">#= data.DisplayName #</li> #} else{# <li><div class="prog"></div> #= data.DisplayName #</li> # }#');
                //"<li>#= data.DisplayName #</li>");
            if (that.options.model != null)
                that.options.dataSource = that.options.model.RegionGroups;
            that._dataSource();
        },
        options: {
            name: "NexuConfiguratorMenu",
            autoBind: true,
            template: ""
        },
        refresh: function () {
            var that = this,
                view = that.dataSource.view(),
                html = $(kendo.render(that.template, view)),
				ul = $(document.createElement('ul')).html(html).on(CLICK, $.proxy(that._click, that));
            //nexu_translator.Translate(window.userLang, ul);
            that.element.html(ul);
        },
        _dataSource: function () {
            var that = this;
            that.dataSource = kendo.data.DataSource.create(that.options.dataSource);
            that.dataSource.bind(CHANGE, function () {
                that.refresh();
            });
            if (that.options.autoBind) {
                that.dataSource.fetch();
                if (window.nexu_dictionary) {
                    var dn = 
                        (window.nexu_dictionary[LOGOS.DisplayName.toUpperCase().replace(' ','_')]) ? window.nexu_dictionary[LOGOS.DisplayName.toUpperCase().replace(' ','_')] : LOGOS.DisplayName;
                    var lb = (window.nexu_dictionary[LOGOS.Label.toUpperCase().replace(' ','_')]) ? window.nexu_dictionary[LOGOS.Label.toUpperCase().replace(' ','_')] : LOGOS.Label;
                    $.extend(LOGOS, { DisplayName: dn,Label: lb});
                }
                that.dataSource.insert(that.options.model.SortOrderLogo-1, LOGOS);
            }
        },
        unselect: function () {
            this.element.find('li').removeClass('active');
        },
        _click: function (e) {
            var t = $(e.target),
                that = this;
            if (t.is('li')) {
                that.unselect();
                _nexu_.Selected.set('Menu', that.dataSource.at(t.index()));
                t.addClass('active');
            }
            return false;
        }
    });
    ui.plugin(NexuConfiguratorMenu);
})(jQuery);


var mobile_ConfiguratorList = function (jQuery, kendo, _, window, undefined) {
    var
        localText = (function () {
            return {
                display: 'Logos',
                label: 'Choose embroidery',
            }
        })(),
        LOGOS = { Id: -1, DisplayName: localText.display, Label: localText.label },
        CHANGE = "change",
        CLICK = 'click',
        MenuTypes = {
            LOGO : 1,
            LOCATION : 2,
            REGION : 3,
            GROUP : 4,
            HIERARCHY : 5,
            COLORLIST: 6,
            COLOR : 7
        };

    function getLocationSource() {
        return _nexu_.LocationMenu.GetDataSource();
    }

    function getDataSource() {
        var hSource = [],
            ColorLists = _nexu_.ColorPicker.getData();
        for (var i = 0; i < _nexu_.Template.RegionGroups.length; i++) {
            var grp = _nexu_.Template.RegionGroups[i];

            var child = {
                level: 1,
                DisplayName: grp.DisplayName,
            };
            var sub;

            if (grp.Id == -1) {
                /* Logos */
                var locations = getLocationSource().data().toJSON();
                for (var x = 0; x < locations.length; x++) {
                    locations[x]._type = MenuTypes.LOCATION;
                    locations[x].level = 2;
                    locations[x].sub = function () {
                        return module_hierarchy.ChildDataSource().one('change', function (e) {
                            e.sender.data($.map(e.sender.data(), function (d) {
                                return $.extend(d, { _type: MenuTypes.HIERARCHY });
                            }));
                        });
                    }
                }
                sub = locations;
                $.extend(child, { _type: MenuTypes.LOGO });
            }
            else if (grp.Regions) {
                /* Regions */
                sub = $.map(grp.Regions, function (o, i) {
                    return {
                        level: 2,
                        DisplayName: o.DisplayName,
                        isRecolorAll: false,
                        code: o.Code,
                        _type : MenuTypes.REGION,
                        sub: $.map(ColorLists.data(), function (a, b) {
                            return {
                                /*__data: a,*/
                                level: 3,
                                _type: MenuTypes.COLORLIST,
                                DisplayName: a.DisplayName,
                                sub: $.map(a.Colors,function(color){
                                    return $.extend(color, { _type: MenuTypes.COLOR });
                                })
                            };
                        })
                    }
                });
                $.extend(child, { _type: MenuTypes.GROUP});
            }
            /* add sub array to node */
            $.extend(child, { sub: sub });

            if (grp.AllowRecolorAll) {
                var all = {
                    level: 2,
                    DisplayName: "All",
                    isRecolorAll: true,
                    sub: $.map(ColorLists.data(), function (a, b) {
                        return {
                            level: 3,
                            DisplayName: a.DisplayName,
                            sub: a.Colors
                        };
                    })
                };
                child.sub.push(all);
            }
            hSource.push(child);
        }
        return new kendo.data.HierarchicalDataSource({
            data: hSource,
            schema: {
                model: {
                    children: {
                        schema: {
                            data: 'sub',
                            model: {
                                children: {
                                    data: 'sub'
                                }
                            }
                        }
                    },
                    hasChildren: function (item) {
                        return (item.sub && item.sub.length > 0);
                    }
                }
            }
        });
    }

    function getTemplate() { return kendo.template($('#mobile_template_basic').html()) };

    function __change(options) {
        var ds = options.dataSource, children, depth = _nexu_.Mobile.get('MenuDepth');
        if (options.direction == 'prev') {
            // If we're at a level 1, return to root.
            if (depth == 1) {
                children = ds;
                _nexu_.Mobile.set('MenuDepth', 0);
                _nexu_.Mobile.set('MenuTitle', _nexu_.Template.DisplayName);
                _nexu_.Mobile.set('ShowBack', false);
            }
            else {
                /* Other wise, get the previous item. Since its array and zero-index, do depth-1 */
                var pData = _nexu_.Mobile.MenuMap[depth-2];
                children = pData.children;
                _nexu_.Mobile.set('MenuTitle', pData.label);
                _nexu_.Mobile.set('MenuDepth', depth-1);
            }
            /* Always remove the last item from the array */
            _nexu_.Mobile.MenuMap.pop();
        }
        if (options.direction == 'next') {
            var selected = options.item, children = [];

            if (selected._type == MenuTypes.LOCATION) {
                _nexu_.LocationMenu.Change(selected);
                children = selected.sub();
            }
            else {
                if (selected.hasChildren) {
                    /* get children datasource */
                    children = selected.children;
                    /* store in navigation array */
                    _nexu_.Mobile.MenuMap.push(selected);
                    _nexu_.Mobile.set('MenuDepth', depth + 1);

                    if (selected._type == MenuTypes.REGION) {
                        /* if it's a region, set it to the main model */
                        if (selected.code) {
                            var region;
                            for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
                                if (_nexu_.Product.Regions[i].Region.Code == selected.code) {
                                    region = _nexu_.Product.Regions[i];
                                    break;
                                }
                            }
                            _nexu_.Selected.set('Region', region);
                        }
                    }

                    if (selected._type == MenuTypes.LOCATION) {
                        _nexu_.LocationMenu.Change(selected);
                    }

                    /* set view model properties */
                    _nexu_.Mobile.set('MenuTitle', selected.label);
                    _nexu_.Mobile.set('ShowBack', true);
                    _nexu_.Mobile.set('PreviousUid', selected.uid);
                }
            }
        }
        /* set widget data, refreshes the menu */
        options.widget.setDataSource(children);
    }
    function getHeadline(){
        return $('.menu-mobile-headline', '#nexu-ui-left');
    }

    function initList(element) {
        var ds = getDataSource(),
            headline = getHeadline(),
            ui = _nexu_.Selected;
        var w = $(element).kendoListView({
            dataSource: ds,
            template: getTemplate(),
            selectable: true,
            dataBound: function (e) {
                //var tHeight = $('.mob-menu-title', '.mob-right').outerHeight();
                //if (e.sender.element.outerHeight() > _nexu_.Mobile.Height)
                //    e.sender.element.parent().height(_nexu_.Mobile.Height - tHeight).mCustomScrollbar({ axis: 'y', autoHideScrollbar: true });
            },
            change: function (e) {
                var source = ds;
                var uid = e.sender.select().data('uid');
                var di = source.getByUid(uid);
                if (!di) {
                    di = source.at(e.sender.select().index());
                }
                if (di) {
                    //var children = di.children;
                    if (typeof (di.isRecolorAll) === typeof (true))
                        _nexu_.Display.set('ChangeAll', di.isRecolorAll);
                    if (di.Hex) {
                        if (_nexu_.Display.get('ChangeAll')) {
                            for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
                                _nexu_.Product.Regions[i].set('Material', di);
                            }
                        }
                        else {
                            _nexu_.Selected.Region.set('Material', di);
                        }
                        _nexu_.Mobile.set('MenuTitle', di.DisplayName);

                    }
                    else {
                        __change({
                            direction: 'next',
                            item: di,
                            widget: e.sender,
                            dataSource: source
                        });
                    }
                }
                else {

                }
            }
            
        }).data('kendoListView');

        headline.on('click', function (e) {
                __change({
                    direction: 'prev',
                    widget: w,
                    dataSource: ds
                });
            return false;
        })
        _nexu_.Mobile.MenuMap = [];
        _nexu_.Mobile.set('MenuDepth', 0);
        _nexu_.Mobile.set('MenuTitle',_nexu_.Template.DisplayName);
    }

    return {
        widget:null,
        init: function () {
            $('#nexu-ui-left').insertAfter('#nexu-ui-mid');
            var el = $('.menu-mobile', '#nexu-ui-left');
            if(!el.data('kendoListView'))
                initList(el);
        }
    }
}(jQuery,kendo,_,window);