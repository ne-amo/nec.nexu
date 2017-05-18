/******************************

NEXU Logo recolor window widget and controller

HTML Template:

<div id="nexu-logo-recolor">
    <script id="template" type="text/x-kendo-template">
    <tr data-uid="#= uid #" title="#: Color.title #">
        <td style="background-color:#: Color.Hex #; color:#: Color.TextHex #;">#: DisplayName #</td>
        <td width="60" class="nexu-recolor-button">Modify</td>
    </tr>
    </script>
    <div class="nexu-top">
        <div id="nexu-logo-preview"></div>
        <div id="nexuColorWheel"></div>
    </div>
    <div class="nexu-bottom">
        <div class="nexu-notification">This is the notification area.</div>
        <div class="nexu-left split-panel">
            <div class="nexu-container">
                <div id="nexu-logo-recolor-list"></div>
            </div>
        </div
        ><div class="nexu-right split-panel">
            <div class="nexu-container">
                <div id="nexuWheelMenu">
                    <div class="horizontal vCenter">
                        <div class="title">Colors <a href="#" title="Expand">-</a></div>
                        <ul class="menu"></ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>


******************************/

	(function () {
	    var kendo = window.kendo,
		ui = kendo.ui,
		Widget = ui.Widget,
		_ = window._,
		CHANGE = "change",
        MAXPREVIEW = 320,
		Link = '/assets/nexu/a.nexuimage?l={0}&b={1}&a={2}&t={3}&c={4}';

		var NexuLogoPreview = Widget.extend({

				init : function (element, options) {
					var that = this;
					kendo.ui.Widget.fn.init.call(that, element, options);
					that.template = kendo.template(that.options.template || "<p><strong>#= data #</strong></p>");
					that._dataSource();
				},
				options : {
					name : "NexuLogoPreview",
					autoBind : true,
					template : "",
                    containerWidth:0,
                    containerHeight:430,//0,
					centerToParent : true
				},
				refresh : function () {
					var that = this,
					view = that.dataSource.view(),
					recolorable = _.where(view, {
							Recolorable : true
						});
					var blob = document.createDocumentFragment(),
					gLayer = _.findWhere(view, {
							Code : "gray"
						}),
					gImg = new Image();
					gImg.src = that._formatLink(gLayer, gLayer, { Hex: '666666',Opacity:100, BlendMode:0 });
					gImg.onload = function () {
					    var im = this;
					    w = (im.width > MAXPREVIEW) ? MAXPREVIEW : im.width;
					    h = (im.height > MAXPREVIEW) ? MAXPREVIEW : im.height;
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
					parent = that.element.parent(),
					pW = parent.width(),
					pH = 430;
					that.element.css({
						left : (pW / 2) - (w / 2),
						top : (pH / 2) - (h / 2)
					});
				},
				_formatLink : function (layer, gray, color) {
					return Link.format(
						layer.ImageDataId,
						gray.ImageDataId,
						color.Opacity,
						color.BlendMode,
						color.Hex.replace('#', '')) + "&m=0&r=96&it=d";
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

	var module_LogoRecolor = function () {
		var container = '#nexu-logo-recolor',
		wheelSelector = '#nexuColorWheel',
		listSelector = '#nexu-logo-recolor-list',
		menuSelector = '.menu',
		previewSelector = '#nexu-logo-preview',
		templateSelector = '#template',
        colorLists = [
                {DisplayName: 'All Colors', Id: 0},
                { DisplayName: 'Matching Colors', Id: 1 },
                { DisplayName: 'Grabbed Colors', Id: 2 }
            ],
        dsThreads = new kendo.data.DataSource({
            transport:{
                read:{
                    url: SVCURL + 'masterdata/designcolors',
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
                }
            }
        });
		return {
			Config : {
				TemplateListItem : null,
				ElementContainer : null,
				ElementWheel : null,
				ElementList : null,
				ElementMenu : null,
				ElementPreview : null,
				WidgetWheel : null,
				WidgetList : null,
				WidgetMenu : null,
				WidgetPreview : null,
				Columns : [{
						field : 'DisplayName',
						headerAttributes : {
							style : 'display: none'
						}
					}
				]
			},
			init : function () {
				/* Initialize Components */
				var elW,
				elL,
				elM,
				elP,
				__this = this;
				__this.Config.ElementContainer = $(container);
				__this.Config.ElementWheel = elW = $(wheelSelector, container);
				__this.Config.ElementList = elL = $(listSelector, container);
				__this.Config.ElementMenu = elM = $(menuSelector, container);
				__this.Config.ElementPreview = elP = $(previewSelector, container);
				__this.Config.TemplateListItem = tLI = $(templateSelector, container).remove();

				/* Configure Wheel Widget */
				__this.Config.WidgetWheel = elW.kendoNexuColorWheel({
				        dataSource: dsThreads,
						draggable : false,
						ringPercentage : 86,
                        pageSize:36,
						width : '500px',
						height : '430px',
                        xPosition:0,
                        yPosition:0
					}).on('select', function (e, d) {
						/*
						Wheel color selection handler
						This Changes the data-item's color
						 */
						var grid = elL.data('kendoGrid'),
						row = grid.select(),
						item = grid.dataSource.view()[row.index()];
						if (item) {
							grid.element.data('selectedIndex', row.index());
							item.set('Color', d);
						}
					}).data('kendoNexuColorWheel');

				/* Configure the Layer List */
				__this.Config.WidgetList = elL.kendoGrid({
                        autoBind:false,
						rowTemplate : kendo.template(tLI.html()),
						columns : __this.Config.Columns,
						scrollable : false,
						selectable : true,
						change : function (e) {
							try {
								var grid = e.sender,
								row = grid.select(),
								item = grid.dataSource.view()[row.index()];
								$('td', grid.element).removeClass('selected');
								row.find('.nexu-recolor-button').addClass('selected');
								if (item.Rule) {
									var filter = {
										logic : item.Rule,
										filters : item.Rules
									}
									__this.Config.WidgetWheel.dataSource.filter(filter);
								} else {
									__this.Config.WidgetWheel.dataSource.filter(null);
								}
							} catch (jse) {}
						},
						dataBound : function (e) {
							/* Maintain Selection when the list is re-drawn on CHANGE */
							var index = e.sender.element.data('selectedIndex'),
							row = $('tbody tr', e.sender.element)[0];
							if (typeof index === 'number')
								e.sender.select($('tbody tr', e.sender.element)[index]);
							else {
								e.sender.select($('tbody tr', e.sender.element)[0]);
								e.sender.trigger('change');
							}
						}
					}).data('kendoGrid');
                    __this.Config.WidgetList.dataSource.filter({field:'Recolorable',operator:'eq',value:true});



				/* Configure the Menu */
				__this.Config.WidgetMenu = elM.kendoListView({
				    dataSource: colorLists,/*@@ TODO Test*/
						template : '<li>#= DisplayName #</li>',
						selectable : true,
						dataBound : function (e) {
							var index = e.sender.element.data('selectedIndex');
							e.sender.element.children('li:eq(' + index + ')').addClass('k-state-selected');
						},
						change : function (e) {
							var i = $(e.sender.select()).index(),
							b = e.sender.dataSource.at(i);
							var current = e.sender.element.data('selectedIndex');
							if (current != i) {
							    e.sender.element.data('selectedIndex', i);
							    if (b.Id == 1) {
							        /* Matching colors selected */
							        var all = __this.Config.WidgetWheel.dataSource.data().toJSON();
							        var filters = [],
							        regions = _nexu_.Product.Regions.toJSON();
							        for(var i =0; i < regions.length; i++) {
							            var region = regions[i].Material;
							            if (region.MatchingDesignColorIds) {
							                if (region.MatchingDesignColorIds.length == 1) {
							                    var obj = _.findWhere(all, { Id: region.MatchingDesignColorIds[0] });
							                    if (obj)
							                        filters.push({ field: 'Id', operator: 'eq', value: obj.Id });
							                }
							                else {
							                    var obj = _.find(all, { Id: region.MatchingDesignColorIds[0] });
							                    if (obj) {
							                        for (var o in obj) {
							                            filters.push({ field: 'Id', operator: 'eq', value: o.Id });
							                        }
							                    }
							                }
							            }
							        };
							        if (filters.length > 0) {
							            __this.Config.WidgetWheel.dataSource.filter({ logic: 'or', filters: filters });
							            __this.Config.WidgetWheel.refresh();
							        }
							    }
							    else if(b.Id == 2){
							        /* Color Grabbed selected */
							    }
							    else {
							        /* Load all colors */
							        __this.Config.WidgetWheel.dataSource.filter(null);
							        __this.Config.WidgetWheel.refresh();
							    }
							}
						}
				}).data('kendoListView').one('dataBound', function (e) {
				    /* once data is read the first time, select the first item in the list to default the wheel */
				    e.sender.select(e.sender.element.children().first());
				})

				/* Initialize Logo Preview */
				__this.Config.WidgetPreview = elP.kendoNexuLogoPreview({
                        autoBind:false
					}).data('kendoNexuLogoPreview');

				__this.Config.ElementContainer.hide();


                $('.nexu-close',__this.Config.ElementContainer).on('click',__this.Close);
			},
			/* End Init */
			Open : function (data) {
				var _t = this.Config,
				ds = _t.WidgetList.dataSource;
                ds.data(data.Layers);
				_t.WidgetPreview.dataSource.data(data.Layers);
				_t.ElementContainer.show();
			},
			Close: function () {
			    $('.doughnutTip').remove();
			    var _t = module_LogoRecolor.Config;/*this.Config;*/
			    //var ruleResult = handler_designrules.ApplyRule({ Type: 'DIFFERENTIATION', RequiredColors: 2 }); //@@TODO TEST
			    var ruleResult = handler_designrules.ApplyRule({ Type: 'COLORWAY'}); //@@TODO TEST
			    ruleResult.done(function (response) {
			        if (typeof response === 'boolean' && response) {
			            _t.ElementContainer.hide();
			        }
                    else if(response){
			            if (response.UserMessage && response.UserMessage != "") {
			                alert(response.UserMessage);
			            }
			            else{
			                alert("There was an issue with applying this color combination.");
			            }
			        }
			        else {
			            _t.ElementContainer.hide();
			        }
			    });
			}
		}
	}
	();