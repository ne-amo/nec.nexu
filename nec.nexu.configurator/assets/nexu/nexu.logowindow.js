
; var module_LogoWindow = function ($) {


    var id = 'nexu-logo-window',
        list = 'nexu-window-list',
        defaults = {
            minWidth: 640,
            maxWidth: 640,
            maxHeight: 480,
            height:480,
            width: 640,
            visible: false,
            resizable: false,
            animation: false,
            scroll:true,
            title: 'Logos',
        }

    function _createElement() {
        var a = document.createDocumentFragment(),
            b = document.createElement('div'),
            c = document.createElement('div'),
            d = document.createElement('ul');
        b.setAttribute('id', id);
        c.setAttribute('class', 'nexu-window-container container');
        d.setAttribute('class', 'nexu-window-list');
        c.appendChild(d);
        b.appendChild(c);
        a.appendChild(b);
        $('body').append(a);
        return _element();
    }
    function _element() {
        return document.getElementById(id);
    }
    function _window() {
        return $(_element()).data('kendoWindow');
    }
    function _getTitle(options) {
        if (options.title)
            return options.title;

        if (_nexu_ && _nexu_.Selected)
            return _nexu_.Selected.get('Hierarchy').DisplayName;

        return "Designs";
    }
    function _open(options) {
        /* Create element if necessary */
        var e = _element();
        if (!e) {
            e = _createElement();
        }
        var w = _window()//,
            //pos = _nexu_.Background.Top.offset();
        /* Access existing window if necessary */
        if (!w) {
            var settings = $.extend({}, defaults, options);
            $(e).kendoWindow(
                $.extend({}, settings,
                    {
                        title: _getTitle(options),
                        open: function (e) {
                            //e.sender.center();
                        },
                        activate: function (e) {
                            var win = e.sender;
                            if (settings.scroll) {
                                $('.nexu-window-container', win.element).mCustomScrollbar({ axis: 'y' })
                            }
                            /* Center window */
                            /* Intialize list view with passed in data. */
                            $('.' + list, '#' + id).kendoListView($.extend({}, {
                                dataSource: options.data,
                                dataBound: function (event) {
                                    //win.center();
                                },
                                template: '<li><span><img src="#: OfficialImage #" title="#: DisplayName #" /></span></li>',
                                selectable: true,
                                change: function (e) {
                                    var item = e.sender.dataItem(e.sender.select());
                                    _nexu_.LogoCarousel.SetDesign(item);
                                }
                            }, options.listViewOptions));
                        }
                    }, options.winOverrides
                )
            );
        }
        else {
            /* If the window already exsits, then change the datasource of the ListView */
            var l = $('.' + list, '#' + id).empty().data('kendoListView');

            l.unbind('change');

            l.setDataSource(kendo.data.DataSource.create(options.data));

            var func =  $.extend({},{
                change: function (e) {
                    var item = e.sender.dataItem(e.sender.select());
                    _nexu_.LogoCarousel.SetDesign(item);
                }  
            }, options.listViewOptions).change;
            l.bind('change', func);

            w.title(options.title);
        }
        _window().open();
    }



    /* Popup window control for the user to browse thumbnails of available logo. */
    return {
        init:function(){
            
        },
        /*  Open the window, and intializes the Kendo Listview control. */
        Open: function (options) {
            if (!options)
                return this;
            _open(options);
            return this;
        },
        Close:function(){
            _window().close();
        }
    }
} (jQuery);
