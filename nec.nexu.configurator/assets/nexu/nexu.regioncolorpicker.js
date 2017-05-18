;m_rcp = module_regioncolorpicker = function($){
    var CLICK = 'click',
        LOADER = nexu_loader,
        saveClass = 'nx-colorlist-save',
        removeClass = 'nx-colorlist-remove',
        USERURL = SVCURL + 'usercolorlists',
        listTemplate = '<li>#= DisplayName # <span class="nx-colorlist-buttons">'+
        '#if (data.AllowSave) {# <a href="\\#" title="Save" alt="Save" class="nx-colorlist-save"><span class="fa fa-floppy-o"></span></a> #}# ' +
        '#if (data.AllowRemove) {# <a href="\\#" title="Remove" alt="Remove" class="nx-colorlist-remove"><span class="fa fa-trash-o"></span></a> #}# ' +
        '</span></li>',
        _ = window._,
        ds_ColorLists = new kendo.data.DataSource({
            transport: {
                read: {
                    url: SVCURL + 'ColorLists/?list=false',
                    type: 'GET',
                    contentType: "application/json",
                    dataType: "json"
                }
            },
            schema: {
                parse: function (data) {
                    return data;
                },
                data: function (data) {
                    return $.map(data,function(d){
                        return $.extend(d, { AllowRemove: false, AllowSave: false, IsUserCreated: false });
                    });
                }
            }
        });

    function _addColorList(list) {
        ds_ColorLists.add(list);
    }
    function _matchToFabric() {
        /* this function is the CLICK function for the "Match to Fabric" button,
        this does not handle the auto-matching */

        if (_nexu_.Selected.get('ForcedGroup')) {
            /* if we have to recolor all the group */
            var regionIds = _nexu_.Selected.get('ForcedGroup');
            for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
                if (regionIds.indexOf(_nexu_.Product.Regions[i].Region.Id) > -1) {
                    var selected = _nexu_.Product.Regions[i].Region;
                    for (var a = 0; a < _nexu_.Product.Regions.length; a++) {
                        if (_nexu_.Product.Regions[a].Region.Id == selected.MatchingRegion) {
                            _nexu_.Product.Regions[i].set('Material', _nexu_.Product.Regions[a].Material);
                            break;
                        }
                    }

                }
                /* Since "match fabric" was clicked, we'll assume that it should be matched going forward */
                _nexu_.Matching[_nexu_.Product.Regions[i].Region.Id] = true;
            }
        }
        else {
            /* just recolor the selected region */
            var selected = _nexu_.Selected.get('Region');
            for(var i=0; i < _nexu_.Product.Regions.length; i++){
                if (_nexu_.Product.Regions[i].Region.Id == selected.Region.MatchingRegion) {
                    selected.set('Material', _nexu_.Product.Regions[i].Material);
                    break;
                }
            }
            /* Since "match fabric" was clicked, we'll assume that it should be matched going forward */
            _nexu_.Matching[selected.Region.Id] = true;
        }
    }
    function _SELECT_wheel(e, d) {
        if(_nexu_.Selected.ChangeAll)
        {
            for(var i = 0; i < _nexu_.Product.Regions.length; i++){
                _nexu_.Product.Regions[i].set('Material', d);
                _nexu_.Product.Regions[i].UserAssigned = true;
            }
        }
        else if (_nexu_.Selected.get('ForcedGroup')) {
            var regionIds = _nexu_.Selected.get('ForcedGroup');
            for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
                if (regionIds.indexOf(_nexu_.Product.Regions[i].Region.Id) > -1) {
                    _nexu_.Product.Regions[i].set('Material', d);
                    _nexu_.Product.Regions[i].UserAssigned = true;
                    _nexu_.Matching[_nexu_.Product.Regions[i].Region.Id] = false;
                }
            }
        }
        else {
            _nexu_.Selected.set('Region.Material', d);
            _nexu_.Selected.set('Region.UserAssigned', true);
            _auto_match(_nexu_.Selected.get('Region.Region.Id'), d);
            if (_nexu_.Selected.Region.Region.MatchingRegion) {
                _nexu_.Matching[_nexu_.Selected.Region.Region.Id] = false;
            }
        }
    }
    function _auto_match(sourceId, material) {
        for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
            if (_nexu_.Product.Regions[i].Region.MatchingRegion == sourceId && _nexu_.Matching[_nexu_.Product.Regions[i].Region.Id]) {
                _nexu_.Product.Regions[i].set('Material', material);
                _nexu_.Product.Regions[i].UserAssigned = true;
            }
        }
    }
    function click_Save(e) {
        var li = $(e.target).closest('li'), uid = li.data('uid'), item = ds_ColorLists.getByUid(uid);
        var data = {
            DisplayName: item.DisplayName,
            CreatorId: _nexu_.Product.Creator.Id,
            MaterialIds: $.map(item.Colors, function (a) { return a.Id })
        };
        /* save to server */
        LOADER.onResolve($.post(USERURL, data, function (d) {
            item.set('Id', d);
            item.set('AllowSave', false);
        }), 'close');

        return false;
    }
    function click_Remove(e) {
        var li = $(e.target).closest('li'), uid = li.data('uid'), item = ds_ColorLists.getByUid(uid);
        /* if its a user created list that has been saved, then delete it */
        if (item.IsUserCreated && item.Id && item.Id != 0) {
            $.ajax({
                url: USERURL + '/' + item.Id,
                type: 'DELETE'
            });
        }
        /* remove from datasource, will refresh the menu list */
        ds_ColorLists.remove(item);
        return false;
    }
    function wasOptionClicked(event) {
        if(!event.target)
            return false;
        var t = $(event.target);
        return t.hasClass(saveClass) || t.hasClass(removeClass) || t.parent().hasClass(saveClass) || t.parent().hasClass(removeClass);
    }
    function getAllMaterials() {
        var arr =  ds_ColorLists.data().toJSON(), r=[];
        for (var i = 0; i < arr.length; i++) {
            for (var a = 0; a < arr[i].Colors.length; a++) {
                r.push(arr[i].Colors[a]);
            }
        }
        return r;
    }
    function processUserColorLists(data) {
        var materials = getAllMaterials();
        for (var i = 0; i < data.length; i++) {
            var list = {};
            list.Id = data[i].Id;
            list.DisplayName = data[i].DisplayName;
            list.AllowSave = false;
            list.AllowRemove = true;
            list.IsUserCreate = true;
            list.Colors = [];
            for(var a= 0; a< data[i].MaterialIds.length; a++){
                var m = _.findWhere(materials, { Id: data[i].MaterialIds[a] });
                if (m)
                    list.Colors.push(m);
            }
            if (list.Colors.length > 0)
                ds_ColorLists.add(list);
        }
    }
    function checkForUserLists() {
        if (!_nexu_.Product.Creator || _nexu_.Product.Creator.Id === 1) {
            return;
        }
        $.ajax({
            url: USERURL + '/' + _nexu_.Product.Creator.Id,
            type: 'GET',
            success: processUserColorLists
        });
    }

    return {
        CheckUserColorlist:function(){
            checkForUserLists();
        },
        Defaults : {
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
        },
        UI:{
            elWheel:null,
            elMenu: null,
            elPager:null,
            elContainer:null
        },
        Change: function (e) {
            /* prevent the change event from firing if the save or remove button was clicked */
            var prevent = wasOptionClicked(event);

            if (!prevent) {
                var m_rcp = _nexu_.ColorPicker;
                var i = $(e.sender.select()).index(),
                    b = e.sender.dataSource.at(i);
                var current = e.sender.element.data('selectedIndex');
                /* check if the currently select item was clicked, which it shouldnt.... because this is a CHANGE event. but w/e */
                if (current != i) {
                    e.sender.element.data('selectedIndex', i);
                    var wheel = m_rcp.UI.elWheel.data('kendoNexuColorWheel');
                    /* change the wheel to display colors, and collapse menu */
                    wheel.change(b.Colors);
                    var p = m_rcp.UI.elPager.show().find('.pager');
                    if (p.data('kendoNexuPager')) p.data('kendoNexuPager').destroy();
                    p.kendoNexuPager($.extend(m_rcp.Defaults, { dataSource: wheel.dataSource }));
                    _nexu_.Display.set('ColorListTitle', b.DisplayName);
                    _nexu_.Display.set('AllowColorGrab', false);
                    m_rcp.Click();
                }
            }
        },
        Click: function(e) {
        /* maximize or minimize the colorlist menu */
            var m_rcp = _nexu_.ColorPicker;
            if ($('.menu',m_rcp.UI.elMenu).is(':visible')) {
                $('.title a',m_rcp.UI.elMenu).text('+');
                $('.menu', m_rcp.UI.elMenu).hide();
                _nexu_.Display.set('AllowColorGrab', false);
                m_rcp.UI.elMenu.data('kendoAnchoredElement').position(m_rcp.UI.elWheel.offset());
            }
            else if (!$('.menu',m_rcp.UI.elMenu).is(':visible')) {
                $('.title a',m_rcp.UI.elMenu).text('-');
                $('.menu', m_rcp.UI.elMenu).show();
                _nexu_.Display.set('AllowColorGrab', true);
                m_rcp.UI.elMenu.data('kendoAnchoredElement').position(m_rcp.UI.elWheel.offset());
                _nexu_.Display.set('ColorListTitle','Colors');
            }
            return false;
        },
        Open:function(obj){
            var wheel = _nexu_.ColorPicker.UI.elWheel.data('kendoNexuColorWheel');
            //wheel.change(obj.lists);
            if (wheel._isClosed)
            {
                wheel.open(obj);
                _nexu_.ColorPicker.UI.elContainer.show();
                _nexu_.View.Controller.Widget().setEditButtons(false);
            }
        },
        Close: function (obj) {
            var m = _nexu_.Selected.Menu;
            if (m && m.Id != -1 && (m.Regions.length == 1 || m.ForceGroup)) {
                _nexu_.Handlers.ResetMenu();
            }
            _nexu_.View.Controller.Widget().deselect().setEditButtons(true);
            _nexu_.Selected.set('ChangeAll', false);
            _nexu_.ColorPicker.UI.elContainer.hide();
        },
        AddColorList:function(list){
            _addColorList(list);
        },
        MatchFabrics:function(e){
            _matchToFabric();
            return false;
        },
        ChangeColor:function(colorObj){
            _SELECT_wheel(null, colorObj);
            return this;
        },
        getData:function(){
            return ds_ColorLists;
        },
        init:function(){
            var m = this;
            def = m.Defaults;
            m.UI.elContainer = $('#nexuColorPicker');
            eW = m.UI.elWheel = $('.wheel-container','#nexuColorWheel');
            eM = m.UI.elMenu = $('#nexuWheelMenu');
            eP = m.UI.elPager = $('#nexuWheelPager');
            dragged = $('#nexuColorWheel');

            wheel = eW.kendoNexuColorWheel({
                autoOpen:false
            }).on('drag', function (e, data) {
                eM.data('kendoAnchoredElement').position({ top: data.top, left: data.left+4 });
            }).on('open', function (e) {
                var targ = dragged, top = parseInt(targ.css('top')), left=parseInt(targ.css('left')),
                menuopen = eM.is(':visible');
                /* gotta fudge the top positioning, i dont remember why, but this works */
                eM.data('kendoAnchoredElement').position({top:top-( (menuopen)? 0 : 140),left:left});
                eM.show();
            }).on('close', function (e) {
                m.Close();
            }).on('drawn',function(e){
            }).on('select', _SELECT_wheel)
            .data('kendoNexuColorWheel');
                
            eP.find('.pager')
                .kendoNexuPager($.extend(def,{dataSource: wheel.dataSource}));
            /* configure the list menu */
            var lv = eM.kendoAnchoredElement({ anchor: eW, elementRegistration: 'W', anchorRegistration: 'E' })
                .find('.menu').kendoListView({
                    dataSource: ds_ColorLists,
                    template: listTemplate,
                    selectable: true,
                    dataBound: function (e) {
                        /* whenever a new colorlist is added, the menu is redrawn, this maintains the "selected" item */
                        var index = e.sender.element.data('selectedIndex');
                        //eM.data('kendoAnchoredElement').position(m.UI.elContainer.offset()); // 
                        e.sender.element.children('li:eq(' + index + ')').addClass('k-state-selected');
                    },
                    change: this.Change
                })
                .on(CLICK, '.' + saveClass, click_Save)
                .on(CLICK, '.' + removeClass, click_Remove)
                .data('kendoListView').one('dataBound', function (e) {
                        /* once data is read the first time, select the first item in the list to default the wheel */
                        e.sender.select(e.sender.element.children().first());
                });
            eP.hide();
            /* bind expand/minimize on menu title */
            $('.title a', eM).on(CLICK, this.Click);
            m.UI.elContainer.hide();
        }
    };
};