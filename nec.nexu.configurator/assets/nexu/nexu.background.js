var module_background = function ($,kendo,_,window,undefined) {
    var ds = new kendo.data.DataSource({
        transport: {
            read: {
                url: window.SVCURL + 'custombackgrounds/query',
                type: 'GET',
                contentType: "application/json",
                dataType: "json"
            },
            parameterMap: function (options, operation) {
                var obj = {};
                var hierarchy = _nexu_.Selected.get('Hierarchy');
                obj.templateId = _nexu_.Template.Id;
                if (hierarchy)
                    obj.hierarchyId = hierarchy.Id;
                return obj;
            }
        },
        schema: {
            model:{ id: 'Id'},
            data: function (data) {
                return data;
            }
        }
    }),
    defaults = {
        thumbField : "ThumbnailPath",
        fileField : 'FilePath',
        extension : 'jpg',
        fileLocation: '/assets',
        popupWidth : 84
    };
    function elements(){
        return {
            container: $('#nexu-background'),
            top: $('#nexu-background-top'), bottom: $('#nexu-background-bottom')
        };
    }
    ds.one('change', function (event) {
        _nexu_.Background.Current = event.sender.data()[0];
    }).bind('change', function (event) {
        /*
        Whenever the background datasource, we check the currently selected background to ensure it is still in the list of available backgrounds.
        If it's not available, then we alert the user, and switch to the default.
        */
        var currentId;
        if(!_nexu_.Background.Current)
            return;
        currentId = _nexu_.Background.Current.Id;
        var isAllowed = ($.map(event.items, function (o) { return o.Id; }).indexOf(currentId) > -1);
        if (isAllowed) /* is available for use, exit function */
            return;

        _nexu_.Background.Current = event.items[0];
        _nexu_.Background.Top.css('background-image', 'url(' + defaults.fileLocation + event.items[0][defaults.fileField] + ')');

        _nexu_.Prompt.open({
            message: 'The background you had selected was an exclusive offering and is no longer available to use.'
        });

    });
    function refreshData() {
        ds.read();
    }
    function setMobile() {
        var h = document.documentElement.clientHeight;
        var w = (document.documentElement.clientWidth - 40);
        $('.boundToHeight').css({ maxHeight: h + 'px', height: h + 'px' });
        var els = elements();
        els.top.css({ maxHeight: (h * (415 / 610)) + 'px', height: (h * (415 / 610)) + 'px' });
        els.bottom.css({ maxHeight: (h * (195 / 610)) + 'px', height: (h * (195 / 610)) + 'px' });
    }
    function setDesktop() {
        $('.boundToHeight').css({ maxHeight: 610 + 'px', height: 610 + 'px' });
        var els = elements();
        els.top.css({ maxHeight: 415 + 'px', height: 415 + 'px' });
        els.bottom.css({ maxHeight: 195 + 'px', height:195+ 'px' });
    }
    function _change(item) {
        /* Change wall background color on select */
        _nexu_.Background.Current = item;
        _nexu_.Background.Top.css('background-image', 'url(' + defaults.fileLocation + item[defaults.fileField] + ')');
    }
	return {
            /* General config options, these may change if the object model changes */
	        config: defaults,
            /* Cache's jQuery selectors, using default names if necessary */
            init:function(d){
                this.config.Element = this.Element = $('#nexu-background');
                this.config.Top = this.Top = (d && d.top) ? $(d.top) : $('#nexu-background-top');
                this.config.Bottom = this.Bottom = (d && d.bottom) ? $(d.bottom) : $('#nexu-background-bottom');
                this.config.List = this.List = (d && d.list) ? $(d.list) : $('div','#nexu-ui-wall-selector');
                /* Whether or not the list should be intiated by default, mostly for testing purposes */
                if(d && d.autoList)
                    this.initList(d.data);
            },
            initList:function(data){
            /* Calling this depends on whether or not background options are unique to a silhouette */
            /* If options are global, this could be called on page init. If they are tied to silhouette, then it should be called when the template is loaded */
                c=this.config;
                /* Intializes the background image drop down list */
                /* Assumes that the FileName and Thumbnail properties are the image file keys. */
                _nexu_.Background.List.kendoDropDownList({
                    dataSource: ds,
                    dataTextField:'DisplayName',
                    dataValueField:this.config.fileField,
                    height:210,
                    template:'<span><img src="'+c.fileLocation+'#: '+c.thumbField+' #" alt="#: DisplayName #" /></span>',
                    valueTemplate: '<span><img src="' + c.fileLocation + '#: ' + c.thumbField + ' #" alt="#: DisplayName #" /></span>',
                    dataBound:function(e){
                        /* Need to manually set the ID of the popup to apply styling */
						e.sender.list
							.attr('id', 'nexu-wall-list')
							.width(c.popupWidth).find('ul.k-list')
							.css({ 'overflow-y': 'scroll', 'overflow-x':'hidden'});
                        /* Intialize custom scrolling on the popup */
                        //$(e.sender.list).mCustomScrollbar();
                    },
                    change:function(e){
                        /* Change wall background color on select */

                        _change(e.sender.dataItem());
                        //_nexu_.Background.Current = e.sender.dataItem();
                        //_nexu_.Background.Top.css('background-image','url('+c.fileLocation+this.value()+')');
                    }
                });
            },
            refresh: function () {
                refreshData();
            },
            getDataSource:function(){
                return ds;
            },
            change:function(item){
                _change(item);
                return this;
            },
            goMobile: setMobile,
            goDesktop:setDesktop,
        }
};