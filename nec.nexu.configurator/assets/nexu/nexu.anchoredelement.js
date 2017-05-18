
(function ($) {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
	    _class = 'nexu-anchored',
	    _fixed = 'nexu-anchored-fixed';

    var AnchoredElement = Widget.extend({

        init: function (element, options) {
            var that = this;
            Widget.fn.init.call(this, element, options);
            that.element.addClass(_class);
        },
        options: {
            name: "AnchoredElement",
            anchor: 'body',
            top: 0,
            left: 0,
            elementRegistration: 'NW',
            anchorRegistration: 'NW'
        },
        position: function (obj) {
            var data = $.extend({}, this.options, obj);
            var e = this.element,
				a = $(data.anchor),
				top = data.top,
				left = data.left,
				aW = (typeof data.x === 'number') ? data.x : a.outerWidth(),
				aH = (typeof data.y === 'number') ? data.y : a.outerHeight(),
				eW = e.outerWidth(),
				eH = e.outerHeight();
            switch (data.elementRegistration.toUpperCase()) {
                case 'N':
                    left -= eW / 2;
                    break;
                case 'NE':
                    left -= eW;
                    break;
                case 'E':
                    left -= eW; top -= eH / 2;
                    break;
                case 'SE':
                    left -= eW; top -= eH;
                    break;
                case 'S':
                    left -= eW / 2; top -= eH;
                    break;
                case 'SW':
                    top -= eH;
                    break;
                case 'W':
                    top -= eH / 2;
                    break;
                case 'NW':
                    break;
                default:
                    top += 0; left += 0;
                    break;
            }
            if(!data.ignoreAnchor){
                switch (data.anchorRegistration.toUpperCase()) {
                    case 'N':
                        left += aW / 2;
                        break;
                    case 'NE':
                        left += aW;
                        break;
                    case 'E':
                        left += aW; top += aH / 2;
                        break;
                    case 'SE':
                        left += aW; top += aH;
                        break;
                    case 'S':
                        left += aW / 2; top += aH;
                        break;
                    case 'SW':
                        top += aH;
                        break;
                    case 'W':
                        top += aH / 2;
                        break;
                    case 'NW':
                        break;
                    default:
                        top += 0; left += 0;
                        break;
                }
            }
            this.options.top = top;
            this.options.left = left;
            e.css({ top: top, left: left });
            return this;
        }
    });

    ui.plugin(AnchoredElement);

})(jQuery);