(function ($) {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
		_snap = window.Snap,
        PROXY = $.proxy,
        _ = window._,
		ns = 'http://www.w3.org/2000/svg',
		xl = "http://www.w3.org/1999/xlink",
        CHANGE = 'change',
		LAYER = '#Layers',
		HIT = '#HitRegions',
		LOGO = '#Logos',
		PATTERN = '#Patterns',
		aLNK = 'xlink:href',
        vLNK = '/assets/nexu/a.nexuimage?l={0}&b={1}&a={2}&t={3}&c={4}',
        REMOVEBUTTON = '<div data-key="{0}" style="top:{1}px; left:{2}px;" class="nexu-viewer-logo-btn nexu-logo-remove addToolTip" title="Remove Logo"><span class="icon icon-20b-close"></span>&nbsp;</div>',
        EDITBUTTON = '<div data-key="{0}" style="top:{1}px; left:{2}px;" class="nexu-viewer-logo-btn nexu-logo-edit addToolTip" title="Edit Logo"><span class="icon icon-20b-dropper"></span>&nbsp;</div>',
        CLASSNAME = 'nexu-view',
        CLASSSELECT = '.' + CLASSNAME,
        EVT_REMOVELOGO = 'removeLogo',
        EVT_EDITLOGO = 'editLogo',
        _resolution = window.ImageResoultion || 96,
        CLASS_STATIC = 'nexu-panel-static',
        CLASS_HOVER = 'nexu-panel-hover',
        CLASS_SELECTED = 'nexu-panel-active',
        EMPTY = '',
        ID = '#nexu-configurator',
		REG_SAFARI = /Safari/i,
		REG_IE = /MSIE/i,
        
        selectedRegions = []
    ;

    function testUA() {
        return REG_SAFARI.test(navigator.userAgent) || REG_IE.test(navigator.userAgent);
    }
    function models() {
        return window._nexu_ || window._nec_.nexu.models;
    }
    function selected() {
        return window._nexu_ || window._nec_.nexu.ui;
    }
    function nexu_ui() {
        return window._nexu_ || window._nec_.nexu.ui;
    }
    function nexu_controls_Menu() {
        return window._nexu_.mobile.controls.menu;
    }

    function REMOVELOGOBUTTONS(key) {
        this.element
            .off('click', '.nexu-viewer-logo-btn')
            .off('mouseover', '.nexu-viewer-logo-btn')
            .find('.nexu-viewer-logo-btn').remove();
        this._s().root.unmouseover().unmouseout();
    }

    function REMOVELOGO(key) {
        var that = this;
        that.clearLogo(key);
        // that.element.data('kendoTooltip').destroy();
        REMOVELOGOBUTTONS.call(that, key);
        that.trigger(EVT_REMOVELOGO, { data: { key: key } });
    }
    function EDITLOGO(key) {
        var that = this;
        that.trigger(EVT_EDITLOGO, { data: { key: key } });
    }
    function ADDHOVEREVENTS() {
        var that = this,
            snap = that._s(),
            show = function () {
                $('.nexu-viewer-logo-btn', that.element).show();
            },
            hide = function () {
                $('.nexu-viewer-logo-btn', that.element).hide();
            };
        if (!nexu_ui().isMobile()) {
            that.element.on('mouseover', '.nexu-viewer-logo-btn', show);
            snap.root.mouseover(show).mouseout(hide);
        }
    }

    function ADDLOGOBUTTONS(key, logo, w) {
        var that = this,
            snap = that._s(),
            of1 = that.getLogoOffset(key),
            of2 = that.element[0].getBoundingClientRect(),//.offset(),
            useNode = snap.select('#' + logo.gray.useId),
            matrix = useNode.attr('transform').localMatrix,
            displayWidth = w * matrix.a,

            top = (of1.top - of2.top);



        var editButtonHtml = EDITBUTTON.format(key, top, (of1.left - of2.left - 50) - (displayWidth / 2)),
            removeButtonHtml = REMOVEBUTTON.format(key, top, (of1.left - of2.left + 10) + (displayWidth / 2));
        var canRemove = _.findWhere(models().Template.DesignLocations, { Code: key }).Removable;
        var editButton = $(editButtonHtml).hide().data('key', key);
        var removeButton = $(removeButtonHtml).hide().data('key', key);
        if (canRemove) {
            REMOVELOGOBUTTONS.call(that, key);
            ADDHOVEREVENTS.call(that);

            that.element
                .append(editButton)
                .append(removeButton)
            /* Moved to widget init so the listener is not assigned more than once */
            //.on('click', '.nexu-viewer-logo-btn.nexu-logo-edit', function (event) {
            //    EDITLOGO.call(that, key);
            //    return false;
            //})
            //.on('click', '.nexu-viewer-logo-btn.nexu-logo-remove', function (event) {
            //    REMOVELOGO.call(that, key);
            //    return false;
            //})
        }
        else {
            ADDHOVEREVENTS.call(that);
            that.element
                .append(editButton)
            /* Moved to widget init so the listener is not assigned more than once */
            //.on('click', '.nexu-viewer-logo-btn', function (event) {
            //    if ($(this).hasClass('nexu-logo-edit')) {
            //        EDITLOGO.call(that, key);
            //        return false;
            //    }
            //    return false;
            //})
        }
    }
    function PROCESS_PATTERN(material, snap, region) {
        var pattern = material.Pattern;
        var patternView = _.findWhere(pattern.ViewImages, { TemplateViewId: nexu_ui().Selected.View });
        var existingImage = snap.select(PATTERN + " > image#" + region.Code)
        if (existingImage) {
            /* pattern image already exists, update src path */
            existingImage.attr('visibility', 'visible');
            existingImage.node.setAttribute('href', patternView.ImagePath);
            return;
        }
        //Build Mask
        var originalMask = snap.select(PATTERN + " > #" + region.Code);
        if (!originalMask) {
            /* can't find a mask, just process as color */
            PROCESS_FABRIC.call(this, material, snap, region);
            return;
        }
        /* build a mask node. using snap.mask() is buggy, best to create it manually */
        var newMask = snap.el('mask');
        newMask.node.setAttribute('id', newMask.id);
        newMask.append(originalMask.clone().attr('visibility', 'visible')).appendTo(snap.select('defs'));
        newMask.node.setAttribute('fill', '#FFFFFF');
        newMask.addClass('nexu-code-' + region.Code);
        /* add the image node and set mask/src */
        var img = snap.image(patternView.ImagePath, 0, 0).insertAfter(originalMask);
        img.node.setAttribute('id', region.Code);
        img.node.setAttribute('mask', "url('#" + newMask.id + "')");
    }
    function PROCESS_FABRIC(material, snap, region) {
        var t = this,
            viewImage = _.findWhere(t.options.view.Images, { Code: region.Code }),
            baseImageId = _.findWhere(t.options.view.Images, { Code: 'root' }).ImageDataId,
            blend = material.BlendMode;
        
        if(viewImage == undefined || baseImageId == undefined)
        {
            return false;
        }
            
        /* check if there is an active pattern */
        var existingImage = snap.select(PATTERN + " > image#" + region.Code)
        if (existingImage) {
            // do stuff
            existingImage.attr('visibility', 'hidden');
        }
        var el = snap.select(LAYER + ' > image#' + region.Code);
        if (el) {
            /* Sets the href of the image node to the dynamic image handler */
            var url = vLNK.format(viewImage.ImageDataId, baseImageId, material.Opacity, blend, material.Hex.replace('#', ''));
            el.attr(aLNK, url + "&it=t&_u=" + (new Date()).getTime());
        }
    }
    function getSvgHeight(node) {
        return node.height.baseVal.value;
    }
    function getSvgWidth(node) {
        return node.width.baseVal.value;
    }
    function SCALE(node, newWidth, newHeight) {
        var svgH = getSvgHeight(node), svgW = getSvgWidth(node);
        // Figure out the ratio
        var ratioX = newWidth / svgW;
        var ratioY = newHeight / svgH;
        // use whichever multiplier is smaller

        var ratio;
        if (newWidth == 0) {
            ratio = ratioY
        }
        else if (newHeight == 0) {
            ratio = ratioX
        }
        else {
            ratio = ratioX < ratioY ? ratioX : ratioY;
        }

        // now we can get the new height and width
        var h = Math.floor(svgH * ratio);
        var w = Math.floor(svgW * ratio);
        if (newHeight != 0)
            node.setAttribute('height', h + 'px');
        if (newWidth != 0)
            node.setAttribute('width', w + 'px');
    }


    function LOGOBUTTONSVISIBLE(makeVisible) {
        if (makeVisible) {

        }
        else {

        }
    }

    /*
    string ; // Layer ID
    string ["b"]; // Base Image Id
    string ["c"]; // Color in HEX Format
    string ["a"]; // Alpha
    string ["t"]; // Enum of Blend Mode
    */
    /*------------
     The NexuView represents a SVG based dynamic view.
     It needs to be linked back to the Primary Model's "Product" observable.
     Upon product change, the view will update.
     "regionSelected" event can be subscribed to for when a user clicks a specific region.
    ------------*/
    var NexuView = Widget.extend({
        init: function (element, options) {
            var that = this;
            kendo.ui.Widget.fn.init.call(that, element, options);
            that.__logoMap = {};
            var html = atob(that.options.view.SvgContent);
            that.element.addClass(CLASSNAME);
            that.element[0].innerHTML = html;
            that._viewStart(that.element)._refresh(); /* Refresh builds the view, recoloring all elements and placing all logos.*/
            that.deselect();
            if (that.options.autoCenter)
                $(window).on('resize', $.proxy(that.center, that));

            ADDHOVEREVENTS.call(that);
            /* setup listener for the edit/remove logo */
            that.element.on('click', '.nexu-viewer-logo-btn.nexu-logo-edit', function (event) {
                var el = ($(this).hasClass('nexu-logo-edit')) ? $(this) : $(this).closest('.nexu-logo-edit');
                var key = el.data('key');
                EDITLOGO.call(that, key);
                return false;
            })
            .on('click', '.nexu-viewer-logo-btn.nexu-logo-remove', function (event) {
                var el = ($(this).hasClass('nexu-logo-remove')) ? $(this) : $(this).closest('.nexu-logo-remove');
                var key = el.data('key');
                REMOVELOGO.call(that, key);
                return false;
            })


            /* Bind the view to the main model so regions and logos are automatically updated.*/
            that.options.product.bind(CHANGE, function (e) {
                if (e.field === 'Regions') {
                    /* Recolor each layer that has been changed. */
                    $.each(e.items, function (i, o) {
                        that.recolorLayer(o.Region, o.Material);
                    });
                }
                if (e.field === 'Logos') {
                    $.each(e.items, function (i, o) {
                        if (o.Location && o.DesignColorway) {
                            /* Place the logo files
                             @TODO add check for if its the same logo as already placed.*/
                            that.placeLogo(o.Location.Code, o.DesignColorway);
                            /* Next recolor to the passed in colorway.*/
                            that._setGrey(o.Location.Code);
                            var rec = _.where(o.DesignColorway.Layers, { Recolorable: true });
                            for (var z = 0; z < rec.length; ++z) {
                                /* The handler and data processing for logo layer recoloring is not ready 05.14*/
                                that.recolorLogo(o.Location.Code, rec[z].Code, rec[z].Color);
                            }
                        }
                        else if (o.Code && o.Color) {
                            /* Handle Layer Recoloring*/
                            var logoObj, locCode;
                            try {
                                logoObj = o.parent().parent().parent();
                            }
                            catch (jser) {

                            }
                            if (logoObj)
                                locCode = logoObj.Location.Code;
                            else
                                locCode = nexu_ui().Selected.get('Logo.Location.Code');
                            that.recolorLogo(locCode, o.Code, o.Color);
                        }
                    });
                }
            });
        },
        options: {
            name: "NexuView",
            container: '#nexu-configurator',
            minWheel: 300,
            maxWheel: 300,
            maxHeight: 400,
            maxWidth: 0,
            product: null, /* Product model to bind to*/
            autoBind: false, /* Automatically enables active clicking of all regions*/
            autoCenter: true, /* Binds the view to automatically center itself to container on window resize*/
            allowMultiSelect: false,
            selectLinkedRegions: true
        },
        events: [
            EVT_REMOVELOGO, EVT_EDITLOGO
        ],
        _s: function () {
            return _snap(this.element.children(":first")[0]); /* Return the snap object for interacting with the SVG DOM */
        },
        enable: function (codes, forceGroup) {
            /*  binds the click events to the hit areas of the view.
                this would be automatically
                the configurator menu should trigger individual areas to become clickable/recolorable
                the widget should be bound to the "regionSelected" event to trigger initialization of the color palette. */
            var s = this._s(), that = this, nodes = [];
            that.deselect();
            if (forceGroup) {
                /* Colors can only be changed as a group. This is mainly for eyelets*/
                for (var i = 0; i < codes.length; i++) {
                    var svg = s.select(HIT + ' > #' + codes[i]);
                    if (svg) {
                        var pos = $(svg.node).offset(),
                        bb = svg.node.getBBox();
                        $(svg.node).closest(CLASSSELECT).data('kendoNexuView')
                            ._triggerSelect(svg, pos.left + (bb.width / 2), pos.top + (bb.height / 2));
                        break;
                    }
                }
            }
            else {
                /* Enable and Bind the Hit Regions */
                s.selectAll(HIT + ' > *').forEach(function (o) {
                    if (codes && codes.indexOf(o.node.id) > -1) {
                        nodes.push(o);
                        o.click(function (e) {
                            /* When a region is clicked, it fires the regionSelected event.
                               The event data can be used by the color wheel widget to be open in a specific spot
                               the region object or key is also sent. */
                            that._clickedRegion(this);
                            $(this.node).closest(CLASSSELECT).data('kendoNexuView')._triggerSelect(this, e.pageX, e.pageY);
                            return false;
                        }).addClass(CLASS_STATIC).attr('visibility', 'visible');

                        if (!nexu_ui().isMobile()) {
                            /* Hover effects */
                            o.mouseover(function (e) {
                                this.addClass(CLASS_HOVER)
                            })
                            .mouseout(function (e) {
                                this.removeClass(CLASS_HOVER)
                            })
                        }
                    }
                });



                if (nodes.length == 1 || codes.length == 1) {
                    /* If there's only 1 region that can be recolored, open the wheel centered on the region */
                    var svg = s.select(HIT + ' > #' + codes[0]);
                    if (svg) {
                        pos = $(svg.node).offset(),
                        bb = svg.node.getBBox();
                        svg.addClass(CLASS_SELECTED).attr('visibility', 'visible')
                        $(svg.node)
                            .closest(CLASSSELECT).data('kendoNexuView')
                            ._triggerSelect(svg, pos.left + (bb.width / 2), pos.top + (bb.height / 2)); /*e.pageX, e.pageY);*/
                    }
                }
            }
        },
        _clickedRegion: function (region) {
            var that = this;

            var index = selectedRegions.indexOf(region.node.id);
            var theRegion = _.find(that.options.product.Regions, function (obj) { return obj.Region.Code == region.node.id; });

            if (index == -1)//add it
            {
                that.selectRegion(region.node.id, { clearSelections: !that.options.allowMultiSelect });

                if (that.options.selectLinkedRegions) {
                    if (theRegion) {
                        var matchingRegions = _.filter(that.options.product.Regions, function (obj) { return obj.Region.MatchingRegion == theRegion.Region.Id; });

                        matchingRegions.forEach(function (m) {
                            that.selectRegion(m.Region.Code, { clearSelections: !that.options.allowMultiSelect });
                        });
                    }
                }
            }
            else {
                that.deselectSingle(region.node.id);

                if (that.options.selectLinkedRegions) {
                    if (theRegion) {
                        var matchingRegions = _.filter(that.options.product.Regions, function (obj) { return obj.Region.MatchingRegion == theRegion.Region.Id; });

                        matchingRegions.forEach(function (m) {
                            that.deselectSingle(m.Region.Code);
                        });
                    }
                }
            }
        },
        selectRegion: function (regionCode, options) {

            var that = this, s = this._s();

            options = options || {};
            options.clearSelections = options.clearSelections == undefined ? true : options.clearSelections;

            if (options.clearSelections || !that.options.allowMultiSelect) {//remove previous selections
                that.deselect();
                selectedRegions = [];//remove previous selections
            }

            if (selectedRegions.indexOf(regionCode) == -1)//add it if not found
            {
                selectedRegions.push(regionCode);
            }
            //get the snap element based on the region
            var sel = s.select(HIT + ' > #' + regionCode);
            if (sel) {
                sel.removeClass(CLASS_HOVER);
                sel.addClass(CLASS_SELECTED);
            }
        },
        getSelectedRegions: function () {
            var that = this;
            var regions = new Array();

            selectedRegions.forEach(function (r) {
                regions.push(_.find(that.options.product.Regions, function (reg) { return reg.Region.Code == r }));
            });

            return regions;
        },
        disable: function () {
            var s = this._s();
            s.selectAll(HIT + ' > *').forEach(function (o) {
                o.unmouseover().unmouseout().unclick();
            });
        },
        _triggerSelect: function (node, pX, pY) {
            /* Figure out how big the colorwheel should be based on the selected element */
            var
                wid = this,
                n = wid.options.minWheel,
                x = wid.options.maxWheel,
                bb = node.getBBox(),
                w = (bb.width > x) ? x : ((bb.width < n) ? n : bb.width),
                h = (bb.height > x) ? x : ((bb.height < n) ? n : bb.height),
                val = (w > h) ? w : h;
            wid.element.trigger('regionSelected', {
                x: pX,
                y: pY,
                width: Math.floor(val),
                height: Math.floor(val),
                key: node.node.id.split('_')[0],
                sender: wid,
                modelTarget: _.findWhere(_.map(wid.options.product.Regions, function (o) { return o.Region; }), { Code: node.node.id.split('_')[0] }).parent()
            });
        },
        clearSelections: function () {
             var that = this;
            //clear array of all selections
            selectedRegions = [];
            that.deselect();
        },
        deselect: function () {
            var that = this, s = that._s();
            s.selectAll(HIT + ' > *.' + CLASS_SELECTED).forEach(function (n) {
                n.removeClass(CLASS_SELECTED);
            });
            return that;
        },
        deselectSingle: function (key) {
            var that = this, s = that._s();
            var path = s.select(HIT + ' > #' + key);
            if (path) {
                path.removeClass(CLASS_SELECTED);
            }

            var regionIndex = selectedRegions.indexOf(key);
            if (regionIndex != -1)//remove if found
            {
                selectedRegions.splice(regionIndex, 1);
            }

            return that;
        },
        clearLogo: function (key) {
            REMOVELOGOBUTTONS.call(this, key);
            var arr = key.split(',');
            for (var i = 0; i < arr.length; i++) {
                this._clearLogo(arr[i]);
            }
            return this;
        },
        _clearLogo: function (key) {
            /* Clears the exist logo based on key.*/
            var t = this, s = t._s(),
                _symbol = s.selectAll('#symbolFor' + key + ' > *'),
                _group = s.select('#groupFor' + key);
            if (!_group)
                return;
            _symbol.remove();
            var els = _group.selectAll('use:not(#useFor' + key + ')')
            if (els)
                els.remove();
        },
        resize: function (d) {
            var that = this;
            if (!d)
                return that;
            var s = that._s();


            var dd = $.extend({}, d);
            if (dd.height) {
                if (typeof dd.height === 'string')
                    dd.height = dd.height.replace('px', EMPTY);
                s.node.setAttribute('height', dd.height + 'px');
            }
            if (dd.width) {
                if (typeof dd.width === 'string')
                    dd.width = dd.width.replace('px', EMPTY);
                s.node.setAttribute('width', dd.width + 'px');
            }
            return that;
        },
        _refresh: function (deferred) {
            /* Loops over the regions and logos in the product and updates the view accordingly.*/
            var that = this;
            that.center();
            var s = that._s();
            /* determine how to resize the image */
            /* the svg will dynamically resize if we change 1 dimension, but will not center properly */
            var svgH = getSvgHeight(s.node);
            var svgW = getSvgWidth(s.node);
            if (
                (that.options.maxHeight && svgH > that.options.maxHeight)
                ||
                (that.options.maxWidth && svgW > that.options.maxWidth)
                ) {
                SCALE(s.node, that.options.maxWidth, that.options.maxHeight);
            }
            //if (that.options.maxWidth && svgW > that.options.maxWidth) {
            //    s.node.setAttribute('width',that.options.maxWidth+'px');
            //}
            if (that.options.product) {
                /* Recolor regions*/
                for (var i = 0; i < that.options.product.Regions.length; ++i) {
                    var r = that.options.product.Regions[i];    
                    that.recolorLayer(r.Region, r.Material);
                }
                /* Place and recolor logos*/
                for (var i = 0; i < that.options.product.Logos.length; ++i) {
                    var logo = that.options.product.Logos[i];
                    if (logo.DesignColorway) {
                        that.placeLogo(logo.Location.Code, logo.DesignColorway);
                        var rec = _.where(logo.DesignColorway.Layers, { 'Recolorable': true });
                        if (that.__logoMap[logo.Location.Code]) {
                            that._setGrey(logo.Location.Code);
                            for (var z = 0; z < rec.length; ++z) {
                                that.recolorLogo(logo.Location.Code, rec[z].Code, rec[z].Color);
                            }
                        }
                    }
                    else {
                        that._clearLogo(logo.Location.Code);
                    }
                }
            }
            /* Bind the newly drawn hit regions. */
            if (that.options.autoBind)
                that.enable($.map(that.options.product.Regions, function (o) { return o.Region.Code }));

            that.element.trigger('drawn');


            console.log('re-set selections');
            //reselect regions
            selectedRegions.forEach(function (r) {
                var p = s.select(HIT + ' > #' + r);
                if (p) {
                    p.addClass(CLASS_SELECTED);
                }
            });

            if (deferred) {
                deferred.resolve();
                console.log('View resolved');
                _nec_.nexu.mobile.setImageScale();
            }
        },
        _viewStart: function (element) {
            /* Hides the cap images when first loaded/switching view.
            Then fades them in once the dynamic url is loaded.
            Ideally this would be done with a event listener delegated to the root element, 
            but jQuery will not listen for events delegated to SVG elements (known issue, no fix intended)
            */
            //var n = navigator;
            //if (!window.isIE) {
            //  if(!testUA()){
            //      element.find('image').hide().on('load', function (e) {
            //          $(e.target).fadeIn(500).unbind('load');
            //      });
            //  }


            console.log('next view start complete');
            return this;
        },
        changeView: function (viewObj, ui) {
            /* Redraws the view */
            var that = this, s = that._s();
            var deferred = $.Deferred();
            that.element.find('svg').fadeOut(500, function () {
                that.__logoMap = {}; /* Clear internal logomap */
                that.options.view = viewObj; /* Set the new view JSON object */
                that.element.html(atob(that.options.view.SvgContent)); /* Place SVG content*/
                that._viewStart(that.element)._refresh(deferred); /* Refresh view widget */
                /* If one of the regions recolorings should be enabled, enable it
                @TODO :: add ID check back in */
                if (ui && ui.Id > -1) {
                    that.enable($.map(ui.Regions, function (o) { return o.Code }));
                }
            });
            return deferred;
        },
        scale: function (options) {
            var s = this._s();
            SCALE(s.node, options.maxWidth, options.maxHeight);
            return this;
        },
        recolorLayer: function (region, material) {
            /* Finds and recolors the "layers" of the svg based on the material. */
            var t = this, s = t._s(), k = region.Code, lId = region.Id,
                viewImage = _.findWhere(t.options.view.Images, { Code: region.Code }),
                baseImageId = _.findWhere(t.options.view.Images, { Code: 'root' }).ImageDataId,
                blend = material.BlendMode; /*(material.BlendMode === 'screen') ? 1 : 0;*/
            if (region.CanHavePattern && (material.Pattern && material.Pattern.Id != 0)) {
                PROCESS_PATTERN.call(t, material, s, region);
            }
            else {
                PROCESS_FABRIC.call(t, material, s, region);
            }
            return t;
        },
        placeLogo: function (key, obj) {
            /* Places the logo and does a bunch of SVG transform and coordinate calculation.
             Since the logos can be any size, certain properties of the SVG use and corresponding image nodes must be updated whenever a graphic is changed. */
            this._clearLogo(key);
            var t = this, s = t._s(), layers = _.sortBy(obj.Layers, function (x) { return x.SortOrder }), d = obj.Design,
            /* Find the required SVG elements */
                _symbol = s.select('#symbolFor' + key),
                _group = s.select('#groupFor' + key),
                _use = s.select('#useFor' + key);
            if (!_use)
                return;
            var _trans = _use.node.getAttribute('transform');
            _symbol.attr('viewBox', '-' + (d.WidthPixel / 2) + ' -' + (d.HeightPixel / 2) + ' ' + d.WidthPixel + ' ' + d.HeightPixel);
            var cache = {};
            for (var i = 0; i < layers.length; ++i) {
                /* For each layer, create and new image and use
                 apply the transform, and set the dynamic dimensions
                 Add to internal data map*/
                var layer = layers[i],
                    imgPath = (typeof d.OfficialImage === "string") ? d.OfficialImage : d.OfficialImage.FilePath,
                    img = s.image(imgPath, -(d.WidthPixel / 2), -(d.HeightPixel / 2)),
                    use = img.use();
                img.attr('id', img.id).appendTo(_symbol);
                use.node.setAttribute('transform', _trans);
                use.node.setAttribute('id', use.id);
                use.appendTo(_group);
                //if(!layer.Recolorable) /* Hide the Team and Gray Images by default */
                //use.node.setAttribute('display','none');
                cache[layer.Code] = { imgId: img.id, useId: use.id, layerObj: layer };
            }
            t.__logoMap[key] = cache;
            if (!window.isMobileSize)
                ADDLOGOBUTTONS.call(this, key, cache, d.WidthPixel, d.HeightPixel);
            return t;
        },
        recolorLogo: function (loc, key, color) {
            var t = this, s = t._s(), layer = t.__logoMap[loc][key], gray = t.__logoMap[loc]['gray'];
            var url = vLNK.format(
                layer.layerObj.ImageDataId,
                gray.layerObj.ImageDataId,
                color.Opacity,
                color.BlendMode,
                color.Hex.replace('#', '')
                );
            url += "&m=0&r=" + _resolution + "&it=d";
            s.select('#' + layer.imgId).attr('xlink:href', url);
            return t;
        },
        _setGrey: function (loc) {
            var t = this, s = t._s(), gray = t.__logoMap[loc]['gray'];
            if (!gray)
                return;
            var url = vLNK.format(
                gray.layerObj.ImageDataId,
                gray.layerObj.ImageDataId,
                1,
                1,
                "000000"
                );
            url += "&m=0&r=" + _resolution + "&it=d";

            s.select('#' + gray.imgId).attr({
                'xlink:href': url
            });
            return t;
        },
        getLogoOffset: function (code) {
            /* This is used by other widgets for their positioning. */
            var t = this, svg = t._s(),
            u = svg.select('#useFor' + code);
            if (!u) {
                return {
                    top: (document.documentElement.clientWidth / 2),
                    left: (document.documentElement.clientHeight / 2)
                }
            }
            var el = $(u.node),
            mxstr = u.node.getAttribute('transform'),
            mx = mxstr.substr(mxstr.indexOf("(") + 1, mxstr.lastIndexOf(")") - 1),
            w = getSvgWidth(u.node),
            h = getSvgHeight(u.node),
            o = u.node.getScreenCTM();
            return {
                matrix: mx,
                top: Math.floor(o.f), //+ (h / 2),
                left: Math.floor(o.e) //+ (w / 2)
            };
        },
        center: function () {
            /* Center the viewer's parent container. */
            var t = this,
                el = t.element,
                con = $(t.options.container),
                w = el.width(),
                h = el.height(),
                pH = con.height(),
                pW = con.width();
            /*el.parent().css({ top: ((pH / 2) - (h / 2)), left: ((pW / 2) - (w / 2)) });*/
        },
        getSvg: function () {
            return this.element.children(":first")[0];
        },
        setEditButtons: function (visible) {
            var that = this;
            if (visible)
                $('.nexu-viewer-logo-btn', that.element).css('opacity', '1');
            else
                $('.nexu-viewer-logo-btn', that.element).css('opacity', '0');
            return that;
        },
        loaded: function () {
            alert('nexu kendo viewer');
        }
    });

    ui.plugin(NexuView);

})(jQuery);