; var module_viewCarousel = function () {
    var defaults = {
        pathField:'Thumbnail',
        labelField:'DisplayName',
        idField:'ViewId',
        fileLocation : '/assets/img/',
    }

    function setMobile() {
        var p = $('.viewer-mask', '#nexu-ui-viewer-selector');
        p.mCustomScrollbar('destroy');
    }
    function setDesktop() {
        //$('.viewer-list', '#nexu-ui-viewer-selector').parent().mCustomScrollbar({ axis: 'x' });
    }
    function disable() {
        $('.viewer-list', '#nexu-ui-viewer-selector').data('kendoListView').disable();
    }
    function enable() {
        $('.viewer-list', '#nexu-ui-viewer-selector').data('kendoListView').enable();
    }
    function changeHandler(e) {
        /*  When item is clicked, set the UI selected view */
        var di = e.sender.dataSource.at(e.sender.select().index());
        _nexu_.Selected.set('View', di.ViewId);
    }

    //kendo.ui.Selectable.fn._myTap = kendo.ui.Selectable.fn._tap;
    //kendo.ui.Selectable.fn._tap = function (e) {
    //    //if ($(e.target).hasClass("n-event-disabled")) {
    //    //    return;
    //    //}
    //    //this._myTap(e);
    //}
    kendo.ui.ListView.fn.enableItem = function (elem) {
        $(elem).removeClass("n-event-disabled");
    }
    kendo.ui.ListView.fn.disableItem = function (elem) {
        $(elem).addClass("n-event-disabled");
    }
    kendo.ui.ListView.fn.enable = function () {
        var w = this;
        w.element.children().each(function (i, o) {
            w.enableItem(o);
        });
        console.log('Carousel enabled');
    }
    kendo.ui.ListView.fn.disable = function () {
        var w = this;
        w.element.children().each(function (i, o) {
            w.disableItem(o);
        });
        console.log('Carousel disabled');
    }
    function getTemplate(data) {
        var c = defaults;
        if (d && d.template) {
            return d.template;
        }
        if (window.isMobileSize) {

        }
        else {
            return '<div><img src="' + c.fileLocation + '#: ' + c.pathField + ' #" alt="#: ' + c.labelField + ' #" title="#: ' + c.labelField + ' #" /></div>'
        }
    }

	return{
        /* General config options, these may change if the object model changes */
		config:defaults,
		init:function(d){
            this.container = (d && d.container) ? $(d.container) : $('#nexu-ui-viewer-selector');
            this.list = $('.viewer-list',this.container);
            /* Only initialize if datasource (array or kendo datasource) is supplied. */
			if(d && d.data)
			{
				var c = this.config;
                var template = (d && d.template) ? d.template : '<div><img src="'+c.fileLocation+'#: '+c.pathField+' #" alt="#: '+c.labelField+' #" title="#: '+c.labelField+' #" /></div>';
				this.list.kendoListView({
					dataSource:d.data,
					selectable:true,
					template:template,
					dataBound:function(e){
                        /*  When the listview is bound, images thumbnails will be loaded
                            Once the images are loaded, the width of the list view needs to be set for proper horizontal scrolling.
                            ImagesLoaded plugin is used for this.
                        */
					    if (!window.isMobileSize) {
					        imagesLoaded(e.sender.element).on('done', function (il) {
					            if (il.images.length > 0) {
					                $(e.sender.element).kendoTooltip({
					                    autoHide: true,
					                    position: 'top',
					                    animation: false,
					                    filter: 'img'
					                });
					                var w = window.isMobileSize ? 'auto' : $(il.images[0].img).parent().outerWidth() * il.images.length + 4;
					                //$(e.sender.element).width(w).parent().mCustomScrollbar({ axis: 'x' });
					            }
					        });
					    }
                        e.sender.element.children(':first').addClass('k-state-selected');
					},
					change:function(e){
                        /*  When item is clicked, set the UI selected view */
						var di=e.sender.dataSource.at(e.sender.select().index());
						_nexu_.Selected.set('View', di.ViewId);
						//e.sender.disable();
					}
				});
			}
		},
		goMobile: function () {
		    setMobile();
		},
		goDesktop: function () {
            setDesktop();
		},
		Disable: function () {
		    disable();
		},
	    Enable: function () {
	        enable();
	    }
	}
};