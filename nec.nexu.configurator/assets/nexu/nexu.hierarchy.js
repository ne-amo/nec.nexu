; var module_hierarchy = (function ($,kendo,undefined) {
    var childSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: window.SVCURL + 'hierarchies/',
                type: 'GET',
                contentType: "application/json",
                dataType: "json"
            },
            parameterMap: function (model) {
                var id = 0,
                    location = 0,
                    template = 0;
                if (model && model.Id) {
                    id = model.Id;
                }
                if (_nexu_.Selected.get('Location.Id')) {
                    location = _nexu_.Selected.get('Location.Id');
                }
                if (_nexu_.Template.Id) {
                    template = _nexu_.Template.Id;
                }
                return { parentId: id, locationId:location, templateId:template };
            }
        },
        schema: {
            parse: function (data) {
                return data;
            },
            data: function (data) {
                return data;
            },
            total:function(data){
                return data.length;
            }
        }
    }),
    parentSource = new kendo.data.DataSource({
        data: []
    }),
    childTemplate = kendo.template($('#nexu-hierarchy-template-child').html()),
    parentTemplate = kendo.template($('#nexu-hierarchy-template-parent').html()),
    el = $('#nexu-hierarchy'),
    elParent = $('#nexu-hierarchy-parent', el),
    configurator = $('#nexu-configurator-module'),
    elChildren = $('#nexu-hierarchy-children', el);

    dummyItem = { Id: -1, DisplayName: 'Choose a category' },
    childChange = function (e) {
        var di = e.sender.dataSource.at(e.sender.select().index());
        if (di) {
            //e.sender.dataSource.one('change', function (dsEvent) {
            //    if (dsEvent.sender.total() == 0) {
            //        module_prompt.open({
            //            title: "No designs found",
            //            message: '<div>Sorry, there are no designs available.</div>',
            //            showDecline: false
            //        });
            //    }
            //    else
            //    {
            //        if (parentSource.at(0).Id == -1)
            //            parentSource.remove(parentSource.at(0));/* Remove dummy from parent */
            //        parentSource.add(di); /* Add selected child to parent */
            //    }

            //});
            if (parentSource.at(0).Id == -1)
                parentSource.remove(parentSource.at(0));/* Remove dummy from parent */
            parentSource.add(di); /* Add selected child to parent */
            e.sender.dataSource.read(di); /* refresh child list */
        }
    },
    parentChange = function (e) {
        var di = e.sender.dataSource.at(e.sender.select().index());
        if (di && di.Id !== -1) {
            /* remove the selected node from parent */
            parentSource.remove(di);
            if (di.ParentHierarchy == null) {
                /* we're at a root node */
                /* clear the parent list */
                while (parentSource.at(0))
                    parentSource.remove(parentSource.at(0));
                childSource.read(); /* default child read */
                parentSource.insert(0, dummyItem); /* insert dummy to parent */
            }
            else {
                /* refresh child with parent */
                childSource.read(di.ParentHierarchy);
            }
        }
    },
    _read = function () {
        $('.nexu-hierarchy-continue', '.nexu-hierarchy-container').hide();
        var r = childSource.read()
        return r;
    }
    function _center(el, container) {
        var q = (el) ? el : '.nexu-hierarchy-container',
            c = $('#nexu-configurator-module'),
            w = c.width(),
            h = c.height(); //document.documentElement.clientWidth, h = document.documentElement.clientHeight;

           // w = document.documentElement.clientWidth,
           // h = document.documentElement.clientHeight;
        //if (container) {
        //    w = container.outerWidth();
        //    h = container.outerHeight();
        //}
        $(q).css({
            left: (w / 2) - (el.outerWidth() / 2) + 20 + 'px',
            top: (h / 2) - (el.outerHeight() / 2) + 20 + 'px'
        });
    }
    function _CLICK_continue(e) {
        $('.nexu-hierarchy-container').trigger('select', parentSource.at(parentSource.total() - 1).toJSON());
        return false;
    }

    return {
        element: null,
        wParent: null,
        wChildren:null,
        init: function (options) {
            var t = this,o=options;
            /* add the default dummy item to parent list */
            parentSource.add(dummyItem);
            this.element = el.addClass('nexu-hierarchy-container');
            /* initialize parent list */
            this.wParent = elParent.kendoListView({
                dataSource: parentSource,
                template: parentTemplate,
                selectable:true,
                change:parentChange
            }).data('kendoListView');
            /* initialize child list */
            this.wChildren = elChildren.kendoListView({
                dataSource: childSource,
                autoBind:false,
                template: childTemplate,
                selectable:true,
                dataBound: function (e) {
                    /* initialize custom scroll bar whenever the child list is re-drawn */
                    e.sender.element.parent().mCustomScrollbar({ axis: 'y' });
                    //t.__center(el); /* center element */
                    _center(el);
                    if (e.sender.dataSource.total() == 0) { /* No more children to navigate through, pass to logo selection */
                        t.__selection(el);
                    }
                },
                change:childChange
            }).data('kendoListView');


            $('.nexu-hierarchy-continue', el).on('click', _CLICK_continue);

            $('.nexu-close', el).on(CLICK, function (e) {
                t.Close();
            });

            if (o && o.autoOpen)
                t.Open();
            else
                t.Close();
            //this.__enableDrag(el, options);
            return t;
        },
        isOpen:function(){
            return el.is(':visible');
        },
        __center:function(el){
            el.css({
                left: (document.documentElement.clientWidth/2)-(el.outerWidth()/2)+20+'px',
                top:(document.documentElement.clientHeight/2)-(el.outerHeight()/2)+20+'px'
            });
        },
        __selection: function (el) {
            /* dispatch select event to listeners */

            el.trigger('select',parentSource.at(parentSource.total() - 1).toJSON());
        },
        __enableDrag:function(el,options){
            /* Enable Dragging */
            var dropTarget;
            if (options && options.draggable && options.draggable.container)
                dropTarget = $(options.draggable.container);
            else
                dropTarget = $('body');

            el.kendoDraggable({
                hint: function (original) {
                    /* created the element that follows the mouse */
                    return original.clone().addClass("drag-clone");
                },
                container: dropTarget,
                dragstart: function (e) {
                    /* hide the original position "hint" */
                    this.element.addClass("drag-hide");
                },
                drag: function (e) {
                    /* trigger drag event on wheel widget to allow anchored elements to follow */
                },
                dragStop: function (e) {
                    /* trigger drag event on wheel widget to allow anchored elements to follow */
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
            });
        },
        Read:function(promise){
            if (promise) {
                return _read();
            }
            else {
                _read();
            }
            return this;
        },
        Open: function () {
            _center(el, configurator);
            if (_nexu_.Selected.get('Hierarchy'))
                $('.nexu-hierarchy-continue', '.nexu-hierarchy-container').show();
            el.show();
        },
        Close: function () {
            //_nexu_.Handlers.ResetMenu();
            el.hide();
        },
        ChildDataSource: function () {
            return childSource;
        },
        ParentDataSource: function () {
            return parentSource;
        }
    }
})(jQuery,window.kendo);