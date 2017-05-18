    
    (function ($) {
        var kendo = window.kendo,
            ui = kendo.ui,
            Widget = ui.Widget,
		    CHANGE = "change",
            CLICK = 'click',
            OPEN = 'open',
            CLOSE = 'close',
            SELECT = 'select',
            defaultTemplate = '<div class="nexu-list-item" data-scale="lineHeight,height,width">'+
                    '<span class="img-wrap">'+
                    '<img style="max-width:{0}%; height:auto;" alt="#: DisplayName #" src="#: OfficialImage #" />'+
                    '</span></div>';

        var LogoCarousel = Widget.extend({
            _current:0,
            init: function (element, options) {
                var that = this;
                kendo.ui.Widget.fn.init.call(that, element, options);
                that.template = kendo.template(that.options.template || defaultTemplate.replace('{0}',that.options.imageScale));
                if(!that.options.autoOpen){
                    that.element.hide();
                }
                that.options.enabled = true;
                that._dataSource();
                that._elLabel = $('.nexu-counter',that.element);
            },
            options: {
                name: "LogoCarousel",
                autoBind: true,
                size:460,
                imageScale:85,
                centerX: 230,
                centerY: 230,
                borderWidth: 0,
                position: null,
                effect: 'easeOutBack',
                enabled:true
            },
            events:[
            SELECT,OPEN,CLOSE
            ],
            refresh: function () {
                if (this.options.enabled ) {
                    var that = this,
                        view = that.dataSource.view(),
                        html = kendo.render(that.template, view);
                    nexu_loader.Show();
                    imagesLoaded(that.element, function () {
                        nexu_loader.Hide();
                    });
                    /* set the width of the outer list to allow for horizontal scrolling, and render html */
                    that.list = $('.nexu-list', that.element).width(that.options.size * view.length).html(html);
                    that._format();
                    that._wire();
                    that._current = 0; /* Reset current carousel position */
                    that.goto(0, 0);
                    that.element.show();
                }
            },
            current: function () {
                return this.dataSource.at(this._current);
            },
            goto: function (index,speed) {
                var that = this,
                    x = that.options.size * index,
                    s = (typeof speed === typeof 1) ? speed : 1000;
                that._changeLabel();
                that.list.animate({ left: (index > that._current) ? x : -x }, s, that.options.effect);
                return this;
            },
            next: function () {
                if (this._current !== this.dataSource.total() - 1)
                    this.goto(++this._current);
                return this;
            },
            prev: function () {
                if (this._current !== 0)
                    this.goto(--this._current);
                return this;
            },
            isOpen:function(){
                return this.__isOpen;
            },
            setDataSource: function (dataSource) {
                this.options.dataSource = dataSource;
                this._dataSource();
                /*
                if (this.options.autoBind) {
                    dataSource.fetch();
                }
                */
            },
            open:function(data){
                var that = this;
                /* when opening, its possible to resize and reposition, as well as change the data. */
                if(data){
                    $.extend(that.options,{autoBind:true, autoOpen:true},data);
                    /* image scale should be supplied from the view widget. it's different for every logo position */
                    if(data.imageScale || data.template){
                        that.template = kendo.template(that.options.template || defaultTemplate.replace('{0}',that.options.imageScale));
                    }
                    /* refresh datasource if supplied */
                    if (data.dataSource) {
                        that._dataSource();
                    }
                    else {
                        that.dataSource.read();
                    }
                }
                that.__isOpen = true;
                //that.element.show();
            },
            close: function () {
                var that = this;
                if(that.__isOpen){
                    that.__isOpen = false;
                    that.element.hide().trigger('close');
                }
                return that;
            },
            enable:function(val){
                var bool = true;
                if (typeof val === typeof bool) {
                    this.options.enabled = val;
                }
            },            
            _total:0,
            _dataSource: function () {
                var that = this;
                that.dataSource = kendo.data.DataSource.create(that.options.dataSource);
                that.dataSource.bind(CHANGE, function () {
                    that._total = this.total();
                    that.refresh();
                });
                if (that.options.autoBind) {
                    that.dataSource.fetch();
                }
            },
            _select:function(e){
                var that = this,item = that.current();
                that.element.trigger(SELECT,item);
                return false;
            },
            _viewAll:function(e){
                this.element.trigger('view');
                return false;
            },
            _wire : function(){
                var that = this;
                /* bind events */
                that.element.off(CLICK)
                    .on(CLICK,'.nexu-left',$.proxy(that.prev,that))
                    .on(CLICK,'.nexu-right',$.proxy(that.next,that))
                    .on(CLICK,'.nexu-close',$.proxy(that.close,that))
                    .on(CLICK,'.nexu-list-item', $.proxy(that._select,that))
                    .on(CLICK,'.nexu-view-all', $.proxy(that._viewAll,that))
                    .on(CLICK,'.nexu-select', $.proxy(that._select,that))
                    .swipe({
                        swipeLeft: $.proxy(that.next, that),
                        swipeRight: $.proxy(that.prev, that)
                    });
                return that;
            },
			center:function(opts){
				var that = this,
                    op = that.options,
                    o = $(that.element);
				o.css({top:op.centerY-(op.size/2),left:op.centerX-(op.size/2)});
			},
            _format:function(h){
                /* this function is necessary to apply dynamic sizing to the absolute positioned elements */
                var that = this,
                    op = that.options,
                    o = $(that.element);
                if (that.dataSource.total() == 1) {
                    $('.nexu-left,.nexu-right,.nexu-view-all', o).hide();
                }
                else {
                    $('.nexu-left,.nexu-right,.nexu-view-all', o).show();
                }
                /* Position the widget */
                o.css({top:op.centerY-(op.size/2),left:op.centerX-(op.size/2)});
                /* apply css styling for scaling */
                o.find('[data-scale]').andSelf().each(function(i,o){
                    var el = $(o), attrs = el.data('scale').split(','), sz = that.options.size,
                    css = attrs.reduce(function(o, v, i) { 
                        o[v] = (v==='lineHeight') ? sz+'px':sz; return o; 
                    }, {});
                    el.css(css);
                });
                /* set position of close button */
                var cl = that._closeLocation();
                $('.nexu-close',o).css(cl);
                return that;
            },
            _closeLocation: function () {
                /* ugh math */
                /*
                    top = y = radius*sin(angle in radians) + circle-center;
                    left = x = radius*cos(angle in radians) + circle-center;
                */
                var that = this,
                    pos = that.element.offset(),
                    cY = pos.top + (that.options.size / 2),
                    cX = pos.left + (that.options.size / 2), 
                    a = (315 * Math.PI / 180),
                    r = (that.options.size / 2),
                    left = cX + (r * Math.cos(a)),
                    top = cY + (r * Math.sin(a));
                return { left: left - pos.left, top: top - pos.top };
            },
            _changeLabel:function(){
                var that=this;
                that._elLabel.text(that._current+1+' of '+that._total);
                return that;
            }
        });

        ui.plugin(LogoCarousel);

    })(jQuery);


var module_LogoCarousel = function ($,kendo,_,window,undefined) {
    var dsColorway = {
        type: "GET",
        url: SVCURL + "designs/defaultcolorway/",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    },
	LOADER = nexu_loader
    EL_ID = '#nexu-logo-wheel',
	ds_Designs = {
	    transport: {
	        read: {
	            url: window.SVCURL + 'designs/',
	            type: 'GET',
	            contentType: "application/json",
	            dataType: "json"
	        },
	        parameterMap: function () {
	            var h = _nexu_.Selected.get('Hierarchy');
	            var l = _nexu_.Selected.get('Location');
	            if (h && h.Id && l && l.Id)
	                return { hierarchyId: h.Id, locationId: l.Id };
	        }
	    },
	    schema: {
	        parse: function (msg) {
	            if (msg && msg.d && typeof msg.d === "string")
	                $.parseJSON(msg.d)
	            if (msg && msg.d)
	                return msg.d;
	            if (msg)
	                return msg;
	            return [];
	        },
	        data: function (data) {
	            return (data && typeof data == typeof []) ? data : [];
	        }
	    }
	},
    defaults_Carousel = { autoBind: false, autoOpen: false, dataSource: new kendo.data.DataSource(ds_Designs) },
	kLC = 'kendoLogoCarousel';
	
    function getElement(){
        return $(EL_ID);
    }
    function getWidget(){
        return $(EL_ID).data(kLC);
    }
    function onSelect(e,d){
        LOADER.onResolve(
			$.ajax(
				$.extend(dsColorway, {
				    data: { designId: d.Id },
				    success: function (data) {
				        if (data && data.d) {
				            var x = $.parseJSON(data.d);
				            x.Layers = $.map(x.Layers, function (l) {
				                if (l.Color && l.Color.TextColor)
				                    l.Color.TextColor = l.Color.TextColor.replace('#', '');
				                return l;
				            });
				            _nexu_.Selected.set('Logo.DesignColorway', x);
				        }
				        else if (data) {
				            data.Layers = $.map(data.Layers, function (l) {
				                if (l.Color && l.Color.TextColor)
				                    l.Color.TextColor = l.Color.TextColor.replace('#', '');
				                return l;
				            });
				            _nexu_.Selected.set('Logo.DesignColorway', $.extend(data, { Design: d }));
				        }
				    }
				})
			), 'close');
    }
    function onView(e,options) {
        var d = getWidget().dataSource;
        _nexu_.LogoWindow.Open($.extend(options,{ data: d }));
        return false;
    }
    function _call() {
        var code = _nexu_.Selected.get('Location').Code;
        if (!code)
            return;
        var o = _nexu_.View.Controller.Widget().getLogoOffset(code);
        var scale = (o.matrix) ? Math.floor(parseFloat(o.matrix.split(',')[0]) * 100) : 1,
        args = {
            centerX: o.left,
            centerY: o.top,
            imageScale: scale
        }
        _nexu_.LogoCarousel.Open(args);
    }
    function _init(){
        getElement().kendoLogoCarousel(defaults_Carousel)
			.on('select', onSelect)
			.on('view',onView);
    }
	
    return {
        Element:function(){
            return getElement();
        },
        widget: function () {
            return getWidget();
        },
        init: function (h) {
            _init();
            return this;
        },
        SetSelect: function (handler) {
            if (handler)
                getElement().off('select').on('select', handler);
        },
        SetView: function (handler) {
            if (handler)
                getElement().off('view').on('view', handler);
            return this;
        },
        SetClose: function (handler) {
            if (handler)
                getElement().off('close').on('close', handler);
            return this;
        },
        GetDataSource:function(){
            return getWidget().dataSource;
        },
        SetData: function(data){
            if(data)
                getWidget().setDataSource(data);
            return this;
        },
        ResetData:function(){
            getWidget().setDataSource(ds_Designs);
            return this;
        },
        Open: function (data) {
            getWidget().open(data);
            return this;
        },
        Close: function () {
            getWidget().close();
            return this;
        },
        SetDesign:function(item){
            onSelect(null, item);
            return this;
        },
        call: function (options) {
            if (window.isMobileSize) {
                getWidget().enable(false);
                onView(null,options);
                return this;
            }
            else {
                _call();
                return this;
            }
        }
    }
}(jQuery, kendo, _, window)

var nexu_DesignLocationSelector = (function ($, kendo, _, window, undefined) {
    var element = '#nexu-logo-location-menu',
        configurator = $('#nexu-configurator-module'),
        //template = '<li>#: DisplayName # <span class="glyphicon glyphicon-plus"></span> <span class="glyphicon glyphicon-pencil"></span> <span class="glyphicon glyphicon-remove"></span> </li>';
        template = '<li>#: DisplayName #</li>';

    function _getElement() {
        return $(element);
    }
    function _getList() {
        return $('ul', element);
    }
    function _getWidget() {
        return _getList().data('kendoListView');
    }
    function _center() {
        var el = _getElement();
        var c = $('#nexu-configurator-module'), w = c.width(), h = c.height(); //document.documentElement.clientWidth, h = document.documentElement.clientHeight;
        el.css({
            left: (w / 2) - (el.outerWidth() / 2) + 20 + 'px',
            top: (h / 2) - (el.outerHeight() / 2) + 20 + 'px'
        });
        return el;
    }
    function _checkPrimary() {
        if (_nexu_.Selected.NeedPrimary) {
            _getWidget().dataSource.filter({ field: 'IsPrimary', value: true, operator: 'eq' });
        }
        else {
            _getWidget().dataSource.filter(null);
        }
    }
    function _checkConflict(selected) {
        var returnVal = null;
        if (selected.SharedLocations && selected.SharedLocations.length) {
            var logos = _nexu_.Product.Logos;
            for (var i = 0; i < logos.length; i++) {
                if (selected.SharedLocations.indexOf(logos[i].Location.Id) > -1 && logos[i].DesignColorway) {
                    returnVal = logos[i].Location;
                    break;
                }
            }
        }
        return returnVal;
    }
    function _open() {
        _checkPrimary();
        _center().show();
    }
    function _close() {
        _getElement().hide();
    }
    function _change(e, item) {
        var data;
        if (item != null)
            data = item;
        if(e != null)
            data = e.sender.dataSource.getByUid(e.sender.select().data('uid'));

        var conflict = _checkConflict(data);
        if (conflict) {
            _nexu_.Prompt.open({
                message: 'If you proceed, the current '+conflict.DisplayName+' will be removed. Proceed?',
                confirm: function () {
                    _nexu_.View.Controller.Widget().clearLogo(conflict.Code);

                    var f = _.find(_nexu_.Product.Logos, function (d) {
                        return d.Location.Id == conflict.Id;
                    });
                    if (f)
                        f.DesignColorway = null;

                    _setLocation(data);
                }
            });
        }
        else {
            _setLocation(data);
        }

        if (item == null) {
            /* if the hierarchies arent loaded, read them in */
            _nexu_.Hierarchy.Open();
            _close();
        }
    }
    function _setLocation(data) {
        //console.log("_setLocation-start");

        var logo = _.filter(_nexu_.Product.Logos, function (logo) {
            return logo.Location.Id == data.Id
        });

        /* load the associated view */
        if (data.TemplateViewId) {
            _nexu_.Selected.set('View', data.TemplateViewId);
            //nec.nexu.mobile.controls.menu.changeViewSelectorId(data.TemplateViewId);
        }
        /* set the logo object */
        _nexu_.Selected.set('Location', data);

        /* set the logo object */
        if (logo.length > 0) {
            _nexu_.Selected.set('Logo', logo[0]);
        }

        /* if the hierarchies arent loaded, read them in */
        if (!_nexu_.Selected.get('Hierarchy')) {
            _nexu_.Hierarchy.Read();
        }
    }
    function _init(options) {
        var d = $.extend({}, { data: [], change: _change, template:template }, options);
        _getList().kendoListView({
            dataSource: kendo.data.DataSource.create(d.data),
            selectable: true,
            template: d.template,
            change:d.change
        });
        _close();
    }
    function _IsLocationUsed(location) {
        var f = _.find(_nexu_.Product.Logos, function (d) {
            return d.Location.Id == location.Id;
        });
        if (!f)
            return false;
        if (f.DesignColorway != null) {
            return location;
        }
        return null;
    }

    return {
        Init:function(options){
            _init(options);
            return this;
        },
        Click: function (e) {
            _open();
            return false;
        },
        ChangeById: function (id) {
            var data = _.find(_getList().data('kendoListView').dataSource.data(), function (d) {
                return d.Id == id;
            });
            var conflict = _checkConflict(data);
            if (conflict) {
                _nexu_.Prompt.open({
                    message: 'If you proceed, the current ' + conflict.DisplayName + ' will be removed. Proceed?',
                    confirm: function () {
                        _nexu_.View.Controller.Widget().clearLogo(conflict.Code);

                        var f = _.find(_nexu_.Product.Logos, function (d) {
                            return d.Location.Id == conflict.Id;
                        });
                        if (f)
                            f.DesignColorway = null;

                        _setLocation(data);
                    }
                });
            }
            else {
                _setLocation(data);
            }
        },
        Change: function (data) {
            var conflict = _checkConflict(data);
            if (conflict) {
                _nexu_.Prompt.open({
                    message: 'If you proceed, the current ' + conflict.DisplayName + ' will be removed. Proceed?',
                    confirm: function () {
                        _nexu_.View.Controller.Widget().clearLogo(conflict.Code);

                        var f = _.find(_nexu_.Product.Logos, function (d) {
                            return d.Location.Id == conflict.Id;
                        });
                        if (f)
                            f.DesignColorway = null;

                        _setLocation(data);
                    }
                });
            }
            else {
                _setLocation(data);
            }
            return this;
        },
        Open: function () {
            _open();
        },
        Close: function () {
            _close();
        },
        GetDataSource: function (checkPrimary) {
            if(checkPrimary)
                _checkPrimary();
            var w = _getList().data('kendoListView');
            return w.dataSource;
        },
        IsLocationUsed: function (location) {
            if (!location)
                return false;
            return _IsLocationUsed(location);
        },
        IsLocationUsedById: function (id) {
            var location = _.find(_getList().data('kendoListView').dataSource.data(), function (d) {
                return d.Id == id;
            });
            return _IsLocationUsed(location);
        }
    }
})(jQuery, kendo, _, window);

var controller_LocationSelector = function(e){
        /* Behavior for the dynamic DesignLocation selector */
        var t = $(e.target), /* Clicked button */
            c=t.data('command'), /* Command */
            arr=_nexu_.Template.DesignLocations, /* Locations loaded from the Template */
            locIds = $.map(arr,function(o,i){
                return o.Id;
            }),
            lId=_nexu_.Selected.Location.Id,
            selectedIndex = locIds.indexOf(lId), /* getting the index of the current location */
            selectedLocation;
            _nexu_.LogoCarousel.Close();
        /* navigation logic */
        if(c==='prev'){
            if(selectedIndex === 0)
                selectedLocation=arr[arr.length-1];
            else
                selectedLocation=arr[selectedIndex-1];
        }
        else{
            if (selectedIndex === arr.length - 1) {
                selectedLocation = arr[0];
            }
            else {
                selectedLocation = arr[selectedIndex + 1];
            }
        }
        /* If the current view does not contain the location, then the correct view needs to be loaded. */
        if (_nexu_.Selected.View != selectedLocation.TemplateViewId) {
            /* since the location logic requires the view be loaded, we have to wait until the view is loaded in DOM to change the location */
            _nexu_.View.Controller.Widget().element.on('drawn', function (e) {
                $(e.target).off('drawn');
                _nexu_.Selected.set('Location', selectedLocation);
            });
            _nexu_.Selected.set('View', selectedLocation.TemplateViewId);
        }
        else {
            _nexu_.Selected.set('Location', selectedLocation); /* Correct view is loaded, change the location */
        }
        return false;
    };