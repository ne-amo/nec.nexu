;var window_Publish = (function($,kendo,_,window,undefined){
    var ID_Element = '#nexu-publish-window',
		kWIN = 'kendoWindow',
		CLICK = 'click',
		btnPublish = '.nexu-btn-publish',
		btnCancel = '.nexu-btn-cancel',
		btnAdd = '.nexu-btn-add',
		inpName = '.nexu-publish-input-name',
		inpTag = '.nexu-input-publish-tag',
		list = '.nexu-list-tags',
		template = '<li>#= data # <a href="\\#" class="k-button nexu-btn-remove-tag"><span class="icon icon-20w-close icon-10"></span></a></li>',
		defaults_window = {
		    resizable: false,
		    draggable: false,
		    modal: true,
            visible:false,
		    open: function (e) {
		        e.sender.center();
		        $('.k-window-titlebar', e.sender.element.parent()).addClass('nexu-download-title').css('margin-top', -31);
		    }
		},
		defaults_list = {
		    dataSource:[],
		    template: template,
		    selectable: false
		};
    function getElement(){
        return $(ID_Element);
    }
    function getWidget(){
        return $(ID_Element).data(kWIN);
    }
    function getElementList(){
        return $(list, ID_Element);
    }
    function getWidgetList(){
        return getElementList().data('kendoListView');
    }
    function _close(){
        getWidget().close();
    }
    function _open(){
        getWidget().open();
    }
    function CLICK_Publish(e) {
        _nexu_.Published = true;
        _nexu_.Public = true;
        _nexu_.Product.DisplayName = $(inpName, ID_Element).val();
        handler_Save.Save();
        _close();
        return false;
    }
    function CLICK_Cancel(e){
        _close();
        return false;
    }
    function CLICK_Add(e){
        /* Probably need to check profanity filter */
        var i = $(inpTag, ID_Element),
            val = i.val(),
            ds = getWidgetList().dataSource;
        if (val != '' && ds.data().toJSON().indexOf(val) == -1) {
            ds.data().push(val);
            getWidget().center();
        }
        i.val('');
        return false;
    }
    function CLICK_Remove(e) {
        var lv = getWidgetList(),
			target =  $(e.target).closest('li').index();
        lv.dataSource.data().splice(target, 1);
        e.preventDefault();
        return false;
    }
    function DATABOUND_List(e){
        $('.nexu-btn-remove-tag').off(CLICK).on(CLICK,CLICK_Remove);
    }
    function _init(){
        getElement().kendoWindow(defaults_window)
			.on(CLICK, btnPublish, CLICK_Publish)
			.on(CLICK, btnAdd, CLICK_Add)
			.on(CLICK, btnCancel, CLICK_Cancel);
        getElementList().kendoListView($.extend({}, defaults_list,
            {
                dataSource: _nexu_.Keywords
            }))
            .on(CLICK, '.nexu-btn-remove-tag', CLICK_Remove);
        $(inpTag, ID_Element).keyup(function (e) {
            if(e.keyCode == 13)
            {
                CLICK_Add(e);
            }
        });
        _close();
    }
    return {
        Init:function(){
            _init();
        },
        Open:function(){
            _open();
            return this;
        },
        Close:function(){
            _close();
            return this;
        },
        Window:function(){
            return getWidget();
        },
        List:function(){
            return getWidgetList
        },
        Data:function(){
            return ds;
        }
    }
	
})(jQuery,kendo,_,window);