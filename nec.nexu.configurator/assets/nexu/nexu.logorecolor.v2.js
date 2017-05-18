/* Logo Preview Widget */
(function ($) {
	var kendo = window.kendo,
	ui = kendo.ui,
	Widget = ui.Widget,
	_ = window._,
	CHANGE = "change",
	MAXPREVIEW = function (element) {
	    return (element.closest('.nexu-middle').width() * 0.75);
	},
    _resolution = window.ImageResoultion,
    className = 'nexu-logo-preview',
	Link = '/assets/nexu/a.nexuimage?l={0}&b={1}&a={2}&t={3}&c={4}';

	function resizeImages(container,max,width,height) {
	    var images = container.find('img'), css = {};
	    if (max) {
            if(width)
                css['max-width'] = width;
            if (height)
                css['max-height'] = height;
	    }
	    else {
	        if (width)
	            css['width'] = width;
	        if (height)
	            css['height'] = height;
	    }
	    images.css(css);
	}

	var NexuLogoPreview = Widget.extend({

			init : function (element, options) {
				var that = this;
				kendo.ui.Widget.fn.init.call(that, element, options);
				$(element).addClass(className);
				that.template = kendo.template(that.options.template || "<p><strong>#= data #</strong></p>");
				that._dataSource();
			},
			options : {
				name : "NexuLogoPreview",
				autoBind : true,
				template : "",
				containerWidth:0,
				containerHeight:430,//0,
				centerToParent : true,
				parentElement: null,
				setMaxDimensions: true,
                getContainerDimensions: MAXPREVIEW
			},
			refresh : function () {
				var that = this,
				view = that.dataSource.view(),
				recolorable = _.where(view, {
						Recolorable : true
				});
				that.element.css('opacity', 0);
				var blob = document.createDocumentFragment(),
				gLayer = _.findWhere(view, {
						Code : "gray"
					}),
				gImg = new Image();
				gImg.src = that._formatLink(gLayer, gLayer, { Hex: '666666',Opacity:100, BlendMode:0 });
				gImg.onload = function () {
				    var im = this, w, h, _MAXPREVIEW = that.options.getContainerDimensions.call(that, that.element);

                        //MAXPREVIEW(that.element);

				    resizeImages(that.element, that.options.setMaxDimensions , _MAXPREVIEW, _MAXPREVIEW);

					w = im.clientWidth;// (im.naturalWidth == im.width) ? im.naturalWidth : im.width;
					h = im.clientHeight;//(im.naturalHeight == im.height) ? im.naturalHeight : im.height;

					that._centerToParent(w, h);
				};
				blob.appendChild(gImg);
				for (var i = 0; i < recolorable.length; i++) {
					var img = new Image();
					img.src = that._formatLink(recolorable[i], gLayer, recolorable[i].Color);
					img.setAttribute('data-layer', recolorable[i].Code);
					blob.appendChild(img);
				}
				that.element.html(blob);
			},
			_getWidthAndHeight: function(image){
				var that = this,
				parent = that.element.parent(),
				iWidth = image.width,
				iHeight = image.height,
				pWidth = parent.width(),
				pHeight = parent.height();
			},
			_centerToParent : function (w, h) {
			    var that = this,
				parent = (that.options.parentElement == null) ? that.element.parent() : $(that.options.parentElement),
                rect = parent[0].getBoundingClientRect(),
				pW = rect.width, //parent.width(),
				pH = rect.height;//parent.width();
				that.element.css({
					left : (pW / 2) - (w / 2),
					top : (pH / 2) - (h / 2)
				});
				that.element.css('opacity', 1);
			},
			_formatLink : function (layer, gray, color) {
				return Link.format(
					layer.ImageDataId,
					gray.ImageDataId,
					color.Opacity,
					color.BlendMode,
					color.Hex.replace('#', '')) + "&m=0&r="+_resolution+"&it=d";
			},
			_recolor : function (item) {
				var that = this,
				gray = $.map(that.dataSource.data(), function (o, i) {
						if (o.Code === 'gray')
							return o;
					})[0],
				url = that._formatLink(item, gray, item.Color);
				$('[data-layer="' + item.Code + '"]', that.element).attr('src', url);
			},
			_dataSource : function () {
				var that = this;
				that.dataSource = kendo.data.DataSource.create(that.options.dataSource);
				that.dataSource.bind(CHANGE, function (e) {
					if (e.action == 'itemchange') {
						that._recolor(e.items[0]);
					} else {
						that.refresh();
					}
				});
				if (that.options.autoBind) {
					that.dataSource.fetch();
				}
			}
		});

	ui.plugin(NexuLogoPreview);

})(jQuery);


/* End Logo Preview Widget */


/* Custom Grid Widget */
(function ($) {
	var kendo = window.kendo,
	ui = kendo.ui,
	Grid = ui.Grid,
	CLICK = 'click',
	CHANGE = "change",
	SELECT = 'select';

	var NexuLogoLayerGrid = Grid.extend({

			init : function (element, options) {
				var that = this;
				kendo.ui.Grid.fn.init.call(that, element, options);
				that.element.off(CLICK);
				that.element.on(CLICK, 'tr td', function(e){
					that.trigger(SELECT, $.extend({},{sender:that, event:e}));
				});
			},
			options : {
				name : "NexuLogoLayerGrid"
			},
			events:[
				SELECT
			]
	});
	
	ui.plugin(NexuLogoLayerGrid);
})(jQuery);
/* End Custom Grid Widget */



/* Logo Editor Configuration and Controls  */

var module_LogoRecolor = (function($,kendo,_,window,undefined){
var 
	/*htmlModal = '<div id="nexu-logo-recolor-window" class="nexu-logo-recolor-window"> <div class="nexu-left nexu-logo-layers"> <div></div> </div> <div class="nexu-middle nexu-logo-wheel"> <div class="wheel-panels"> <div class="wheel-panel wheel-left"></div> <div class="wheel-panel wheel-right"></div> </div> <div> <div class="nexu-logo-preview"></div> </div> <div class="logo-colorwheel-container"> <div class="wheel"></div> </div> </div> <div class="nexu-right nexu-logo-menu"> <div></div> </div> </div>',*/
	htmlModal = '<div id="nexu-logo-recolor-window" class="nexu-logo-recolor-window"> <div class="nexu-logorecolor-left nexu-logo-layers"> <div class="wrapper"> <div class="title">Logo Sections</div> <div class="nexu-menu-layers transparent-border"></div> </div> </div> <div class="nexu-middle nexu-logo-wheel"> <div class="wheel-panels"> <div class="wheel-panel wheel-left"></div> <div class="wheel-panel wheel-right"></div> </div> <div> <div class="nexu-logo-preview"></div> </div> <div class="logo-colorwheel-container"> <div class="wheel"></div> </div> <div class="buttons"> <div class="nexu-btn-confirm"><span class="icon icon-40b-check"></span></div> <div class="nexu-btn-cancel"><span class="icon icon-40b-close"></span></div> </div> </div> <div class="nexu-logorecolor-right nexu-logo-menu"> <div class="wrapper"> <div class="title">Embroidery Colors</div> '+/*<div id="nexuColorPicker-ColorGrab" class="nexu-bindable" data-bind="visible: AllowColorGrab" data-obs="Display"><a href="#" title="Upload an Image">Color Grabber</a></div>*/'<ul class="nexu-menu-threads"></ul> <div class="nexu-menu-threads-pager"><span class="pager"></span></div> </div> </div> </div>',
	htmlOverlay = '<div id="nexu-logo-overlay">&nbsp;</div>',
	htmlLayerID = '#template-LogoLayer',
	kCOLORWHEEL = 'kendoNexuColorWheel',
	heightMax = 600,
	widthList = 250,
    LOADER = nexu_loader,
	heightPercentage = .9,
	colorLists = [ /* this should be replaced with a dynamic list */
		{DisplayName: 'All Colors', Id: 0},
		{ DisplayName: 'Matching Colors', Id: 1 }
	],
	_recentColors = kendo.observable(
		{
			Id:2,
			DisplayName:'Recent Colors',
			Colors: new kendo.data.DataSource({
				data:[],
				schema:{
					model:{
						id:'Id'
					}
				}
			})
		}
	),
	dsThreads = new kendo.data.DataSource({
		transport:{
			read:{
				url: SVCURL + 'masterdata/designcolors/',
				type:'GET',
				contentType: "application/json",
				dataType: "json"
			}
		},
		schema:{
			parse:function(data){
				return (data) ? data : [];
			},
			data:function(data){
				return data;
			},
			total:function(data){
				return data.length;
			}
		}
	}),
    defaults_ajax_ColorwayCheck = {
        type: "POST",
        url: window.SVCURL + "designs/checkcolorway/",
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    },
	defaults_ColorWheels = {
		dataSource: dsThreads,
		draggable : false,
		ringPercentage : 88,
		pageSize:36,
		width : '430px',
		height : '430px',
		xPosition:0,
		yPosition:0,
		tooltips:false
	},
	defaults_Columns = [{
		field : 'DisplayName',
		headerAttributes : {
			style : 'display: none'
			}
		}
	],
	defaults_LayerList = {
		autoBind:false,
		rowTemplate : kendo.template($(htmlLayerID).html()),
		columns : defaults_Columns,
		scrollable : false,
		selectable : true
	},
	defaults_ColorList = {
		dataSource: new kendo.data.DataSource({data:colorLists}),/*@@ TODO Test*/
		template : '<li>#= DisplayName #</li>',
		selectable : true
	},
	defaults_Pager = {
		buttonCount: 0,
		info: true,
		numeric: false,
		input: false,
		pageTotals: true,
		messages: {
			display: "Wheel {0} of {1}",
		},
		dataBound: function (e) {
		},
		change: function (e) {
		}
	};
	/* Element retrieval and modification */
	function _getElement_Container(){
		return $('#nexu-logo-recolor-window');
	}
	function _getElement_Wheel(){
		return $('.wheel', '#nexu-logo-recolor-window');
	}
	function _getElement_Panels(){
		return $('.wheel-panels', '#nexu-logo-recolor-window');
	}
	function _getElement_LayerList(){
		return $('.nexu-menu-layers', '#nexu-logo-recolor-window');
	}
	function _getElement_ListMenu(){
		return $('.nexu-menu-threads', '#nexu-logo-recolor-window');
	}
	function _getElement_LogoPreview(){
		return $('.nexu-logo-preview', '#nexu-logo-recolor-window');
	}
	function _getElement_ListPager(){
		return $('.nexu-menu-threads-pager > .pager', '#nexu-logo-recolor-window');
	}
	function _getElement_Buttons(){
		return $('.buttons', '#nexu-logo-recolor-window');
	}
	function _getOverlay(){
		return $('#nexu-logo-overlay');
	}
	function _addModal(){
		var h = $(htmlModal).hide();
		var u = $(htmlOverlay).hide();
		$('#nexu-configurator-module').append(u).append(h);
	}
	function _visible(show){
		if(show)
			$('#nexu-logo-recolor-window,#nexu-logo-overlay').show();
		else
			$('#nexu-logo-recolor-window,#nexu-logo-overlay').hide();
	}
	function _calcDisplay(){
		/* get sizes for UI.  */
		var winH = document.documentElement.clientHeight;
		var winW = document.documentElement.clientWidth;
	    var modalH = ((winH * heightPercentage) > heightMax) ? heightMax : (winH * heightPercentage),
            modalW = ((modalH + (widthList * 2)) > winW) ? winW : modalH + (widthList * 2);
	    //modalW = modalH + (widthList*2);
		return {
			h:modalH,
			w: modalW,
			x : (winW - modalW) / 2,
			y : (winH - modalH) / 2,
			ww :winW, wh: winH
		}
	}
	function _setSizes() {
	/* Calculate the viewable screen size and set the height on necessary ui elements. */
		var sz = _calcDisplay();
		var container = _getElement_Container();
		container.width(sz.w).height(sz.h).css({ top: sz.y, left: sz.x });
		$('.nexu-middle', container).width(sz.h).height(sz.h);
		_getElement_Buttons().width(sz.h);
		_getOverlay().width(window.innerWidth).height(window.innerHeight);
		var rect = _setPanels();
		if (rect == undefined) {
		    return sz;
		}
		$('.wheel-panel').width((rect.width / 2) - 6).height(rect.height - 6).parent().css({ top: '8px', left: '8px' });
		return sz;
	}
	function _setPanels() {
	    var wheel = _getElement_Wheel(),
	        svg = wheel[0].firstChild;
	    if (svg == undefined)
	        return;
	    var path;
	    $.each(svg.childNodes, function (i,el) {
	        if (el.nodeName == 'path') {
	            path = el;
	            return false;
	        }
	    });
	    var b = path.getBoundingClientRect();
	    return b;
	}
	function _verticalCenter(element, sizes) {
	    var sz = (sizes) ? sizes : _calcDisplay();
	    var height = element.height();
	    element.css({ top: ((sz.h / 2) - (height / 2)) + 'px' });
	}
    /* Widget handlers */

	function _handler_Select_Wheel(e,d){
		/*
		Wheel color selection handler
		This Changes the data-item's color
		 */
		var grid = _getElement_LayerList().data('kendoGrid'),
		row = grid.select(),
		item = grid.dataSource.view()[row.index()];
		if (item) {
			grid.element.data('selectedIndex', row.index());
			item.set('Color',d);
			if(!_recentColors.Colors.get(d.Id))
				_recentColors.Colors.add(d);
			e.preventDefault();
		}
	}
	function _helper_FailDifferentiation(){
		/* see if there are rules */
		if(!_nexu_.ColorwayRules)
			return false;
		var rule = _nexu_.ColorwayRules.ColorDifferentiationRules[0];
		if(!rule)
			return false;	
		/* get the unique number of colors in colorway */
		var colorway =  $.map(_getElement_LayerList().data('kendoGrid').dataSource.data().toJSON(),function(l){
            if(l.Recolorable)
                return l.Color.Id;
			});
		/* if it fails, return the rule */
		return (_.uniq(colorway).length < rule.RequiredColors) ? rule : false;
	}
	function _helper_FailColorway() {
	    var obj = _nexu_.Selected.get('Logo.DesignColorway'),
            colorway = {
                Layers: $.map(obj.Layers, function (layer) {
                    if(layer.Recolorable && layer.Color)
                        return {
                            ImageDataId: layer.ImageDataId,
                            Color: {
                                Id:layer.Color.Id
                            }
                        }
                })
            },
            call = $.extend({},
                defaults_ajax_ColorwayCheck,
                {
                    url: defaults_ajax_ColorwayCheck.url + obj.Design.Id,
                    data: kendo.stringify(colorway)
                }
            );
	    return $.ajax(call)
	}
	function _helper_FindLayerRule(dataId){
		if(!_nexu_.ColorwayRules)
			return null;
		var rule = _.findWhere(_nexu_.ColorwayRules.DesignColorRules, {ImageDataId : dataId});
		if(!rule)
			return null;
		return {
			logic: rule.Combination.toLowerCase(),
			filters: $.map(rule.Values,function(obj,ind){
				return { field: 'Id', operator: obj.Operator, value: obj.DesignColorId };
			})
		}
	}
	var filterTypes = {
		RULE : 0,
		UI: 1,
		OUTER: 3
	}
	function _parseLayerRulesForDataSource(source, layer) {
	    var rule = _helper_FindLayerRule(layer.ImageDataId);
	    if (rule) {
			var filterToSet;
	        var filter = {
	            logic: rule.logic,
	            filters: rule.filters,
				//type: filterTypes.RULE
	        }
			var currentPage = source.page();
	        source.filter(filter);
			source.page(currentPage);
			
	    } else {
	        /* No rules, clear the filters. */
			var currentPage = source.page();
	        source.filter(null);
			source.page(currentPage);
	    }
	}
	function _handler_Change_Layers(e){
		try {
			/* Depending on the layer that is chosen, we may have to filter or un-filter the available thread lists.
			Define variables and access the colorwheel's datasource */
			var grid = e.sender,
			row = grid.select(),
			wheelSource = _getElement_Wheel().data(kCOLORWHEEL).dataSource,
			item = grid.dataSource.view()[row.index()];
			$('td', grid.element).removeClass('selected');
			row.find('.nexu-recolor-button').addClass('selected');
		    /* Item is the layer data item. We check the Rule property */
			_parseLayerRulesForDataSource(wheelSource, item);

		} catch (jse) {}
	}
	function _handler_DataBound_Layers(e){
		/* Maintain Selection when the list is re-drawn on CHANGE */
		var index = e.sender.element.data('selectedIndex'),
			row = $('tbody tr', e.sender.element)[0];
		if (typeof index === 'number') {
		    /* there's an issue with just calling the select method of the grid, causing it to refresh the color wheel. 
            while this is kind of the expected behavior. it doesn't play well for the user */
		    $('tbody tr:eq('+index+')', e.sender.element).addClass('k-state-selected').attr('aria-selected', true);
		    //e.sender.select($('tbody tr', e.sender.element)[index]);
		}
		else {
		    e.sender.select($('tbody tr', e.sender.element)[0]);
		    e.sender.trigger('change');
		}
	}
	function _handler_DataBound_ColorList (e) {
		/* Simply adding a selected class to the list item */
		var index = e.sender.element.data('selectedIndex');
		e.sender.element.children('li:eq(' + index + ')').addClass('k-state-selected');
	}
	function _helper_GetRegionColor(id){
		var a = _nexu_.Product.Regions.toJSON();
		var ret;
		for(var i = 0; i < a.length; i++){
			if(a[i].Region.Id == id)
				ret = a[i].Material.Hex;
		}
		return ret;
	}
	function _helper_SetPanelColors(options){
		var P = _getElement_Panels(),
			L = $('.wheel-left',P),
			R = $('.wheel-right',P),
			LOC = options.parent().Location,
			LEFT = _helper_GetRegionColor(LOC.BackgroundLeftRegionId),
			RIGHT =  _helper_GetRegionColor(LOC.BackgroundRightRegionId);
		L.css('background-color','#'+LEFT);
		R.css('background-color','#'+RIGHT);
	}
	
	
	
	function _helper_ColorList_Matching(data){//wheel){
		var colors = [],
			distinct = [],
			regions = _nexu_.Product.Regions.toJSON();
		
		for(var i =0; i < regions.length; i++) {
			var region = regions[i].Material;
			if (region.MatchingDesignColorIds) {
				/* depending on how many matching colors there are, we have to send add an extra loop to filter. */ 
				if (region.MatchingDesignColorIds.length == 1) {
					var obj = _.findWhere(data, { Id: region.MatchingDesignColorIds[0] });
					if ( obj && distinct.indexOf(obj.Id) == -1 ){
						colors.push(obj);
						distinct.push(obj.Id);
					}
				}
				else {
					var obj = _.find(data, { Id: region.MatchingDesignColorIds[0] });
					if (obj) {
						for (var o in obj) {
							if(distinct.indexOf(o.Id) == -1){
								colors.push(o);
								distinct.push(o.Id);
						}
					}
				}
			}
		}
		};
		return colors;
		
		//		/* Helper function for when "Matching colors" selected */
		//		var all = wheel.dataSource.data().toJSON(),
		//			filters = [],
		//			regions = _nexu_.Product.Regions.toJSON();
		//		/* Loop through the regions and get the Ids of matching Design colors */
		//		for(var i =0; i < regions.length; i++) {
		//			var region = regions[i].Material;
		//			if (region.MatchingDesignColorIds) {
		//				/* depending on how many matching colors there are, we have to send add an extra loop to filter. */ 
		//				if (region.MatchingDesignColorIds.length == 1) {
		//					var obj = _.findWhere(all, { Id: region.MatchingDesignColorIds[0] });
		//					if (obj)
		//						filters.push({ field: 'Id', operator: 'eq', value: obj.Id });
		//				}
		//				else {
		//					var obj = _.find(all, { Id: region.MatchingDesignColorIds[0] });
		//					if (obj) {
		//						for (var o in obj) {
		//							filters.push({ field: 'Id', operator: 'eq', value: o.Id });
		//						}
		//					}
		//				}
		//			}
		//		};
		//		/* Update Colorwheel */
		//		if (filters.length > 0) {
		//			wheel.dataSource.filter({ logic: 'or', filters: filters, type:filterTypes.UI });
		//			wheel.refresh();
		//		}
	}
	function getRecentColorFilter(){
		return $.map(_recentColors.Colors.data(),function(o,i){
			return o; //{ field: 'Id', operator: 'eq', value: o.Id };
		});
	}
	function _handler_Change_ColorList(e) {
		/* Changel the Colorwheel's datasource 
			These options are somewhat hardcoded. Will need to update once source list is made dynamic.
		*/
		var i = $(e.sender.select()).index(),
			b = e.sender.dataSource.at(i),
			wheel = _getElement_Wheel().data(kCOLORWHEEL);
		var current = e.sender.element.data('selectedIndex');
		if(typeof current === 'undefined')
			return
		if (current != i) {
			e.sender.element.data('selectedIndex', i);
			if (b.Id == 1) {
				var matching = _helper_ColorList_Matching(dsThreads.data().toJSON());//wheel);
				wheel.dataSource.page(1);
				wheel.dataSource.data(matching);
			}
			else if(b.Id == 2){
				/* Recent Colors */
				var d = _recentColors.Colors.data().toJSON();
				wheel.dataSource.page(1);
				wheel.dataSource.data(d);
			}
			else {
				/* Load all colors */
				wheel.refresh(1);
				wheel.dataSource.data(dsThreads.data().toJSON());
			}
		}
	}
	function _getRules(designId){
		return $.ajax({
			url: SVCURL + 'designs/rules/'+designId,
			type:'GET',
			contentType: "application/json",
			dataType: "json",
			success:function(msg){
				_nexu_.ColorwayRules = msg;
			}
		});
	}

	function _openExec(options) {
	    _visible(true);
	    var sizes = _setSizes();
		_helper_SetPanelColors(options);
		var layerList = _getElement_LayerList(), layers = layerList.data('kendoGrid').dataSource;
		layerList.data('selectedIndex', 0);
		layers.one('change', function (e) {
		    _verticalCenter(layerList.closest('.wrapper'),sizes);
		});
		layers.data(options.Layers);
		_verticalCenter(_getElement_ListMenu().closest('.wrapper'), sizes);
		_getElement_LogoPreview().data('kendoNexuLogoPreview').dataSource.data(options.Layers);
		_nexu_._PreviousColorway = options.Layers.toJSON();
		
	}
	function _openModal(options){
		/* 	If the logo is being opened fresh, we check for any pertinent rules
			this saves us from hitting the server if a logo is edited right away
		*/
		if(!_nexu_.ColorwayRules || _nexu_.ColorwayRules.DesignId != options.Design.Id){
		    LOADER.onResolve(_getRules(options.Design.Id).then(function (data) {
		        /* open the ui */
		        _openExec(options);
		    }), 'close');
		}
		/* logo is being edited after just being added, rules should still be in memory */
		else{
			/* open the ui */
			_openExec(options);
		}
	}
	function _setPrimaryFlag() {
        if(_nexu_.Selected.Logo.Location.IsPrimary)
	        _nexu_.Selected.NeedPrimary = false;
	}
	function _helper_Cancel_Confirm(){
		/* If the user cancels the recolor, restore the previous colorway */
		var P = _nexu_._PreviousColorway,
			C = _getElement_LayerList().data('kendoGrid').dataSource.data();
		for(var i = 0; i < C.length; i++){
			/* Only the recolorable layers. The indexes should be the same. This may need to be tested */
			if(C[i].Recolorable)
			{
				C[i].set('Color',P[i].Color);
			}
		}
		_visible(false);
	}
	function _closeModal(restore){

		
		if(restore){
			_nexu_.Prompt.open({
				title:'Cancel Changes?',
				message: 'Are you sure you\'d like to undo any changes?',
				confirm: _helper_Cancel_Confirm
			});
		}
		else{
		    /* check color differential rules since they are parsed client side */
			var failsDifferential = _helper_FailDifferentiation();
			if (failsDifferential)
			{
			    _nexu_.Prompt.open({
			        title: 'Sorry',
			        message: fails.UserMessage,
			        showDecline: false,
			        confirmText: 'Ok'
			    });
			}
			else {
                /* now check colorway rules. these have to go to the server */
			    LOADER.onResolve(
                    $.when(_helper_FailColorway()).then(function (response) {
                        if (response) {
                            /* failed rule check */
                            _nexu_.Prompt.open({
                                title: 'Sorry',
                                message: response.UserMessage,
                                showDecline: false,
                                confirmText: 'Ok'
                            });
                        }
                        else {
                            /* allowed, close overlay */
                            _setPrimaryFlag();
                            _nexu_._PreviousColorway = null;
                            _visible(false);
                        }
                    }),'close'
                );
			}
		}
		
		
	}
	function _init_Widgets(options){
		/* intialize the ui */
	    var sz = _setSizes();
		
		dsThreads.read().done(function(){
			
		var wheel = _getElement_Wheel()
			.kendoNexuColorWheel($.extend({}, defaults_ColorWheels,
            {
				dataSource:dsThreads.data().toJSON(),
                width: sz.h + 'px',
                height: sz.h + 'px',
                donutOptions: {
                    afterDrawed: function (e) {
                        $(this).kendoTooltip({
                            filter:'path',
                            callout: false,
							show:function(e){
								if(!e.sender.popup.wrapper.hasClass('no-events'))
									e.sender.popup.wrapper.addClass('no-events');
							}
                        });
                    }
                }
            }))
				.on('select', _handler_Select_Wheel).data('kendoNexuColorWheel');

			_getElement_ListPager().kendoNexuPager($.extend({},defaults_Pager,{dataSource: wheel.dataSource}));
			
			});
			
		_getElement_LayerList()
			.kendoGrid($.extend({},defaults_LayerList,{change : _handler_Change_Layers,dataBound : _handler_DataBound_Layers}))
			.data('kendoGrid')
			.dataSource.filter({field:'Recolorable',operator:'eq',value:true});
		var menu = _getElement_ListMenu()
			.kendoListView($.extend({},defaults_ColorList,{change:_handler_Change_ColorList,dataBound:_handler_DataBound_ColorList}))
			.data('kendoListView');

		menu.element.data('selectedIndex', 0);
		_recentColors.one('change',function(e){
			menu.dataSource.add(_recentColors);
		});
			
		menu.one('dataBound', function (e) {
				/* once data is read the first time, select the first item in the list to default the wheel */
				e.sender.select(e.sender.element.children().first());
			});
		_getElement_LogoPreview().kendoNexuLogoPreview({autoBind:false,parentElement:_getElement_Wheel()});
		_getElement_Panels().on('click',function(e){_closeModal();return false;});
		$('.nexu-btn-cancel',_getElement_Container()).on('click',function(e){
			_closeModal(true);
		});
		$('.nexu-btn-confirm',_getElement_Container()).on('click',function(e){
			_closeModal(false);
		});
	}
	_addModal();
	_init_Widgets();
	/* return object to interact with other modules. */
	return {
		_recentColors: _recentColors,
	    SetPrimary:function(){
	        _setPrimaryFlag();
	        return false;
	    },
	    GetDataSource:function(layer){
	        if (layer) {
	            _parseLayerRulesForDataSource(dsThreads, layer);
	        }
	        return dsThreads;
	    },
	    GetLayers:function(){
	        return _getElement_LayerList().data('kendoGrid').dataSource;
	    },
		Resize:function(){
			_setSizes();
		},
		Open:function(data){
			_openModal(data);
		},
		Close:function(data){
			_closeModal(data);
		},
	}

})(jQuery,kendo,_,window);