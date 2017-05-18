
        (function ($) {
            var kendo = window.kendo,
                ui = kendo.ui,
                Widget = ui.Widget,
		        CHANGE = "change",
                CLICK = 'click',
		        _class = 'nexu-color-wheel',
                DATABINDING = "dataBinding",
                DATABOUND = "dataBound";

            function drawSummaryBox(options) {
                var
                    padding = ((options.width-(options.width * (options.percentage / 100)))/2)+4,
                    w = options.width + 'px', h = options.height + 'px',
                    html = '<div class="wheel-description" style="padding:0 '+padding+'px; width:'+w+'; height:'+h+';">'+
                    '<div class="nexu-wheel-label" style="width:' + w + ';height:' + h + ';"><span class="donutColorTip" style="display:none;"></span></div></div>';
                return html;
            }
            function flipDepth(el1, el2) {
                var z1 = el1.css('z-index'), z2 = el1.css('z-index');
                el1.css('z-index', z2);
                el2.css('z-index', z1);
            }

            var NexuColorWheel = Widget.extend({

                init: function (element, options) {
                    var that = this;
                    kendo.ui.Widget.fn.init.call(that, element, options);
                    that.element.addClass(_class);
                    that.template = kendo.template(that.options.template || "<p><strong>#= data #</strong></p>");
                    that._closeButton = $('.nexu-close', that.element.closest('.drag-container')).on(CLICK,$.proxy(that.close,that));
                    that._dataSource();
                    if (that.options.draggable) {
                        /* setting up the dragging functionality */
                        var dropTarget;
                        if (that.options.draggable.container)
                            dropTarget = $(that.options.draggable.container);
                        else
                            dropTarget = $('body');
                        /* since there the widget itself is the wheel, we have to navigate up and get the wrapper 
                            this was ad-hoc'd after the fact, in that the dragging should be in the controller
                            but whatever */
                        that.dragger = that.element.closest('.drag-container').find('.draggable').kendoDraggable({
                            hint: function (original) {
                                /* created the element that follows the mouse */
                                return original.clone().addClass("drag-clone");
                            },
                            container: dropTarget,
                            dragstart: function (e) {
                                that.clearTips();
                                /* hide the original position "hint" */
                                this.element.addClass("drag-hide");
                            },
                            drag: function (e) {
                                /* trigger drag event on wheel widget to allow anchored elements to follow */
                                that.element.trigger('drag', $(".drag-clone").offset());
                            },
                            dragStop:function(e){
                                /* trigger drag event on wheel widget to allow anchored elements to follow */
                                that.element.trigger('drag', that.element.offset());
                            }
                        });
                        dropTarget.kendoDropTarget({
                            drop: function (e) {
                                /* when dropped, set the position */
                                var pos = $(".drag-clone").offset();
                                $(e.draggable.currentTarget).css(pos)
                                    .find('div').andSelf()
                                    .removeClass("drag-hide")
                                    
                            }
                        })
                    }
                    that.close();
                    if (that.options.autoOpen)
                        that.open();
                    else {
                        that._isClosed = true;
                        that.element.width(0).height(0);
                    }
                    
                },
                options: {
                    name: "NexuColorWheel",
                    autoBind: true,
                    template: "",
                    xPosition: 0,//function () { return Math.max(document.documentElement.clientWidth, window.innerWidth || 0) / 2; },
                    yPosition: 0,//function () { return Math.max(document.documentElement.clientHeight, window.innerHeight || 0) / 2; },
                    width: 240,
                    height: 240,
                    pageSize: 10,
                    ringPercentage:70,
                    draggable: { container: 'body' },
                    autoOpen: true,
                    zoom: true,
                    onClick: null,
                    tooltips: 'center',
                    donutOptions: {}
                },
                events: [
                    DATABINDING,
                    DATABOUND
                ],
                _drawn:false,
                refresh: function (page) {
                    var that = this;
					if(page)
						that.dataSource.page(page);
					var view = that.dataSource.view();
                    that.element.width(that.options.width).height(that.options.height);
                    if (view.length > 0) {
                        this.clearTips();
                        try {
                            
                            that.element.empty();
                            if (that.options.tooltips && that.options.tooltips == 'center')
                                that.element.append(
                                    drawSummaryBox({ width: that.options.width, height: that.options.height, percentage: that.options.ringPercentage })
                                );

                            that.element.drawDoughnutChart(view,$.extend({}, that.options.donutOptions,
                                {
                                    onPathClick: $.proxy(that._onClick, that),
                                    percentageInnerCutout: that.options.ringPercentage
                                }));

                            //flipDepth($('.wheel-description', that.element), $('svg', that.element));
                         }
                         catch(except){
                             console.log(except.message);
                         }
                         /* set position of close button */
                         if(!that._drawn){
                             var ofst = that._closeLocation();
                             that._closeButton.css(ofst);
                         }
                    }
                    that.trigger(DATABOUND);
                    that._drawn = true;
                },
                _dataSource: function () {
                    var that = this;
                    that.dataSource = kendo.data.DataSource.create(that.options.dataSource);
                    that.dataSource.pageSize(this.options.pageSize);
                    that.dataSource.bind(CHANGE, function () {
                        that.refresh();
                    });
                    if (that.options.autoBind) {
                        that.dataSource.fetch();
                    }
                },
                _onClick: function (event, data) {
                    var index = $(event.target).data().order,
						colorobj = data[index];
                    this.element.trigger(
                        'select', colorobj);
                    return false;
                },
                setWidth: function (val) {
                    this.options.width = val;
                    this.refresh();
                    return this;
                },
                setHeight: function (val) {
                    this.options.height = val;
                    this.refresh();
                    return this;
                },
                clearTips: function () {
                    $('.doughnutTip').hide();
                    return this;
                },
                change: function (data) {
                    this.clearTips().options.dataSource = data;
                    this._dataSource();
                    return this;
                },
                zoom: function (dir) {
                    var t = this;
                    if (dir && typeof dir === 'string') {
                        if (dir.toUpperCase() === 'IN') {
                            kendo.fx(t.element).zoom("in").startValue(0).endValue(1).play();
                            if (this._isClosed) {
                                t.element.trigger('drag', { top: 0, left: 0 });
                                this._isClosed = false;
                            }
                        }
                        else if (dir.toUpperCase() === 'OUT') {
                            kendo.fx(t.element).zoom("in").startValue(1).endValue(0).play();
                            this._isClose = true;
                        }
                    }
                    return this;
                },
                open: function (data) {
                    var t = this,
                        x = t.options.xPosition,//(),
                        y = t.options.yPosition;//();
                    if (data) {
                        var d = $.extend({}, t.options, data);
                        x = d.xPosition - (d.width / 2),
					    y = d.yPosition - (d.height / 2);
                    }
                    this._isClosed = false;
                    //t.element.css({ 'top': y, 'left': x }).show().trigger('open');
                    if (t.options.draggable)
                        t.dragger.css({ 'top': y, 'left': x })
                    else
                        t.element.css({ 'top': y, 'left': x })
                    t.element.show().trigger('open')
                    return t;
                },
                close: function () {
                    this._isClosed = true;
                    this.clearTips();
                    /*this.element.hide().trigger('close');*/
                    this.element.trigger('close');
                    return this;
                },
                _closeLocation: function () {
                    /* ugh math */
                    /*
                        top = y = radius*sin(angle in radians) + circle-center;
                        left = x = radius*cos(angle in radians) + circle-center;
                    */
                    var that = this,
                        pos = that.element.offset(),
                        cY = pos.top + (that.options.height / 2),
                        cX = pos.left + (that.options.width / 2), 
                        a = (315 * Math.PI / 180),
                        r = (that.options.width / 2),
                        left = cX + r * Math.cos(a),
                        top = cY + r * Math.sin(a);
                    return {left: left+6,top: top-6};
                }
            });
            ui.plugin(NexuColorWheel);

        })(jQuery);