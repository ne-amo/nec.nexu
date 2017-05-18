; (function ($, kendo, _, window, undefined) {

    /* Namespace */
    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.data = nec.nexu.data || {};
    ui = nec.nexu.ui = ui = nec.nexu.ui || {};

    /* Scoped Variables */
    var config = nec.nexu.config, common = nec.nexu.common, data = nec.nexu.data, OBJ = {Id: 0}
    defaultSchema = {
        parse: function (data) {
            return (data) ? data : [];
        },
        data: function (data) {
            return data;
        },
        total: function (data) {
            return data.length;
        }
    },
    defaultEvents = {
        requestStart: function (e) {
            nec.nexu.mobile.loader.show();
        },
        requestEnd: function (e) {
            nec.nexu.mobile.loader.hide();
        }
    },
    defaultTransport_Read = {
        type: 'GET',
        contentType: "application/json",
        dataType: "json"
    }


    
    /* Members */
    data.Background = new kendo.data.DataSource( {
        transport: {
            read: $.extend({},defaultTransport_Read, {
                url: config.SVCURL + 'custombackgrounds/query'
            }),
            parameterMap: function (model, operation) {
                var obj = {};
                var hierarchy = nec.nexu.models.SelectedHierarchy,
                    template = nec.nexu.models.Template || OBJ;
                if (hierarchy)
                    obj.hierarchyId = hierarchy.Id;
                if (template)
                    obj.templateId = template.Id;
                return obj;
            }
        },
        schema: $.extend({},defaultSchema,{model: {id:'Id'}})
    })
    data.Threads = new kendo.data.DataSource($.extend({},{
        transport: {
            read: $.extend({},defaultTransport_Read, {
                url: config.SVCURL + 'masterdata/designcolors/'
            })
        },
        schema: defaultSchema
    }))
    data.ColorwayConfiguration = $.extend({},defaultTransport_Read, {
        url: config.SVCURL + "designs/defaultcolorway/"
    })
    data.DesignsConfiguration = $.extend({}, defaultEvents, {
        transport: {
            read: $.extend({},defaultTransport_Read, {
                url: config.SVCURL + 'designs/',
            }),
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
    });
    data.HierarchyConfiguration = $.extend({}, defaultEvents, {
        transport: {
            read: $.extend({},defaultTransport_Read, {url: config.SVCURL + 'hierarchies/'}),
            parameterMap: function (model) {

                var basic = { parentId: 0, locationId: 0, templateId: 0 };
                $.extend(basic,model);

                return basic;
            }
        },
        schema: {
            parse: function (data) {
                return data;
            },
            data: function (data) {
                return data;
            },
            total: function (data) {
                return data.length;
            }
        }
    });
    data.TreeViewRegions = function (i) {

        var treeviewDS = new Array();

        /*id: 0, text: "All", expanded: true, spriteCssClass: "rootfolder", items: [
                {
                    id: 6, text: "Panels", expanded: false, spriteCssClass: "regiongroup", items: [
                        { id: "LFW", dataid: 11, text: "Left Front", spriteCssClass: "panel" },
                        { id: "LMW", dataid: 12, text: "Left Middle", spriteCssClass: "panel" },
                        { id: "LBW", dataid: 13, text: "Left Back", spriteCssClass: "panel" },
                        { id: "RFW", dataid: 16, text: "Right Front", spriteCssClass: "panel" },
                        { id: "RMW", dataid: 15, text: "Right Middle", spriteCssClass: "panel" },
                        { id: "RBW", dataid: 14, text: "Right Back", spriteCssClass: "panel" }
                    ]
                },
                {
                    id: 7, text: "Visor", expanded: false, spriteCssClass: "regiongroup", items: [
                        { id: "Visor", dataid: 17, text: "Visor", spriteCssClass: "panel" },
                        { id: "Undervisor", dataid: 18, text: "Undervisor", spriteCssClass: "panel" },
                    ]
                },
                {
                    id: 8, text: "Eyelets", expanded: false, spriteCssClass: "regiongroup", items: [
                        { id: "EyeletLFW", dataid: 19, text: "Left Front", spriteCssClass: "eyelet" },
                        { id: "EyeletLMW", dataid: 20, text: "Left Middle", spriteCssClass: "eyelet" },
                        { id: "EyeletLBW", dataid: 21, text: "Left Back", spriteCssClass: "eyelet" },
                        { id: "EyeletRFW", dataid: 24, text: "Right Front", spriteCssClass: "eyelet" },
                        { id: "EyeletRMW", dataid: 23, text: "Right Middle", spriteCssClass: "eyelet" },
                        { id: "EyeletRBW", dataid: 22, text: "Right Back", spriteCssClass: "eyelet" }
                    ]
                },
                {
                    id: 9, text: "Stitching", expanded: false, spriteCssClass: "regiongroup", items: [
                        { id: "FrontStitch", dataid: 25, text: "Front", spriteCssClass: "stitching" },
                        { id: "MidStitch", dataid: 26, text: "Middle", spriteCssClass: "stitching" },
                        { id: "BackStitch", dataid: 27, text: "Back", spriteCssClass: "stitching" },
                        { id: "VisorStitch", dataid: 28, text: "Visor", spriteCssClass: "stitching" },
                        { id: "UnderStitch", dataid: 29, text: "Undervisor", spriteCssClass: "stitching" }
                    ]
                },
                {
                    id: 10, text: "Button", expanded: false, spriteCssClass: "regiongroup", items: [
                        { id: "Button", dataid: 30, text: "Button", spriteCssClass: "panel" }
                    ]
                }
            ]
        }]
    */

        var templateGroups = nec.nexu.models.Template.RegionGroups;
        var regionGroups = new Array();

        for (var g = 0; g < templateGroups.length; g++) {
            
            var groupRegions = templateGroups[g].Regions;
            var regionItems = new Array();

            for (var r = 0; r < groupRegions.length; r++) {

                var regionobj = {
                    id: groupRegions[r].Code,
                    dataid: groupRegions[r].Id,
                    text: groupRegions[r].DisplayName,
                    spriteCssClass: "region"       
                };

                regionItems.push(regionobj);
                
            }

            var groupobj = {
                id: templateGroups[g].Id,
                text: templateGroups[g].DisplayName,
                expanded: false,
                spriteCssClass: "regiongroup",
                items: regionItems           
            };

            regionGroups.push(groupobj);
        }

        var allobj = {
            id: 0,
            text: "All",
            expanded: true,
            spriteCssClass: "rootfolder",
            items: regionGroups
        };

        treeviewDS.push(allobj);
        return treeviewDS;

        //return allobj;
    }

    data.getThreadColorCollection = function () {
        var threadCol = [];
        var ds = nec.nexu.data.Threads;
        ds.filter(null);
        threadCol.push({ Id: 0, DisplayName: 'All Colors', Colors: ds.data() });

        // Matching Colors Only
        var filters = mobile.Handlers.getMatchingThreadsFilter();//ds.data().toJSON());
        
        ds.filter({ logic: 'or', filters: filters });
        threadCol.push({ Id: 1, DisplayName: 'Matching', Colors: ds.data() });

        threadCol.push({ Id: 3, DisplayName: 'Recent', Colors: ui.RecentThreads.data() });

        return threadCol;

    }
    data.ColorLists = new kendo.data.DataSource( {
        transport: {
            read: $.extend({},defaultTransport_Read, {url: config.SVCURL + 'ColorLists/?list=false'})
        },
        schema: {
            model:{id:'Id'},
            parse: function (data) {
                return (data) ? data : [];
            },
            data: function (data) {
                return $.map(data, function (d) {
                    return $.extend(d, { AllowRemove: false, AllowSave: false, IsUserCreated: false });
                });
            }
        }
    })
    data.LoadProduct = function (id) {
        /* Returns deferred object for chaining */
        return $.ajax($.extend({},defaultTransport_Read, {url: config.SVCURL + "FinishedProducts/configurator/"+id
                , complete: function () {
                    console.log('Product Load Complete');
                }
            })
        );
    }
    data.wakeUp = function()
    {
        $.ajax($.extend({}, defaultTransport_Read, {
            url: config.SVCURL + "masterdata/wakeup"
                , complete: function () {
                    console.log('Services are awake');
                }
            })
        );
    }

    /* End */
    window._nec_ = nec;
})(window.jQuery, window.kendo, window._, window);