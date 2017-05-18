//window.SVCURL = "http://localhost:50596/api/";
window.SVCURL = "http://nexuservicesqa.neweracap.com/api/";
window.NEXUIMGROOT = "http://neccmsdev.neweracap.com";
window.CHANGE = 'change';
window.CLICK = 'click';
window.urlParams = null;
window.SHOP = 'http://localhost:60452/order/?materialId={0}&Style={1}&neStyle={0}';
(window.onpopstate = function () {
    var match,
        pl = /\+/g,
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);

    window.urlParams = {};
    window.urlParams.getParameter = function (key) {
        for (var k in this) {
            if (k.toUpperCase() == key.toUpperCase())
                return this[k.toUpperCase()];
        }
    }
    while (match = search.exec(query))
        window.urlParams[decode(match[1]).toUpperCase()] = decode(match[2]);
})();
window.isIE = (function () {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf('MSIE ');
    var trident = ua.indexOf('Trident/');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }
    if (trident > 0) {
        // IE 11 (or newer) => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }
    return false;
})();
window.getCookie = function(name) {
    var value = "; " + document.cookie;
    var parts = value.split("; " + name + "=");
    if (parts.length == 2) return parts.pop().split(";").shift();
}
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
        ? args[number]
        : match
            ;
        });
    };
}
if (!String.prototype.parseIntWithDefault) {
    String.prototype.parseIntWithDefault = function (defaultValue) {
        var retValue = defaultValue;
        var str = this;
        if (str !== null) {
            if (str.length > 0) {
                if (!isNaN(str)) {
                    retValue = parseInt(str);
                }
            }
        }
        return retValue;
    };
}

/* taken from http://stackoverflow.com/questions/19817899/jquery-or-javascript-how-to-disable-window-scroll-without-overflowhidden
disables window scroll when hovering over a scrollable panel. 
*/ 
//    $('body').on('DOMMouseScroll mousewheel','.nx-scrollable', function (ev) {
//        var $this = $(this),
//            scrollTop = this.scrollTop,
//            scrollHeight = this.scrollHeight,
//            height = $this.height(),
//            delta = (ev.type == 'DOMMouseScroll' ?
//                ev.originalEvent.detail * -40 :
//                ev.originalEvent.wheelDelta),
//            up = delta > 0;
//    
//        var prevent = function () {
//            ev.stopPropagation();
//            ev.preventDefault();
//            ev.returnValue = false;
//            return false;
//        }
//    
//        if (!up && -delta > scrollHeight - height - scrollTop) {
//            // Scrolling down, but this will take us past the bottom.
//            $this.scrollTop(scrollHeight);
//    
//            return prevent();
//        } else if (up && delta > scrollTop) {
//            // Scrolling up, but this will take us past the top.
//            $this.scrollTop(0);
//            return prevent();
//        }
//    });
(function ($, kendo) {

	
	
    /* adding utility methods to the Kendo Window control */
    kendo.ui.Window.fn.fullScreen = function () {
        /* for some reason maximize doesn't account for the title bar height when calculating height */
        var that = this;
		$(window).off("resize" + ".kendoWindow" + that._marker); // the widget binds to the window resize and attempts to re-maximize. and screws the size
        that.maximize();
        that.wrapper.height(that.wrapper.outerHeight() + that.titleElement().outerHeight());
		
        return that;
    };
	kendo.ui.Window.fn.changeButton = function(selector, html, click){
		var w = this;
		var h = $(selector, w.titleElement())
		h.html(html);
		w.wrapper.find('.k-i-close').click(function(e){
			w.close();
			e.preventDefault();
		});
		return w;
	};
    kendo.ui.Window.fn.verticalCenter = function (element) {
        /* adds a top margin to the window content to center it vertically in the window */
        var that = this,
            section = $(element, that.element);
        var containerHeight = that.element.outerHeight(),
            sectionHeight = section.outerHeight(),
            title = that.titleElement().outerHeight(),
            margin = (containerHeight / 2) - (sectionHeight / 2);
        section.css({
            'margin-top': (margin - (title*2)) + 'px'
        });
        return that;
    }
    kendo.ui.Window.fn.titleElement = function () {
        /* returns the title jquery object */
        return this.wrapper.find('.k-window-titlebar');
    }
	
	
	
	
})(window.jQuery,window.kendo);


//; (function (window, undefined) {
//    var nec = window._nec_ || {};
//    nec.nexu = nec.nexu || {};
//    nec.nexu.common = nec.nexu.common || {};

//    (window.onpopstate = function () {
//        var match,
//            pl = /\+/g,
//            search = /([^&=]+)=?([^&]*)/g,
//            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
//            query = window.location.search.substring(1);

//        window.urlParams = {};
//        window.urlParams.getParameter = function (key) {
//            for (var k in this) {
//                if (k.toUpperCase() == key.toUpperCase())
//                    return this[k];
//            }
//        }
//        while (match = search.exec(query))
//            window.urlParams[decode(match[1])] = decode(match[2]);
//    })();
//    window.isIE = (function () {
//        var ua = window.navigator.userAgent;
//        var msie = ua.indexOf('MSIE ');
//        var trident = ua.indexOf('Trident/');
//        if (msie > 0) {
//            // IE 10 or older => return version number
//            return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
//        }
//        if (trident > 0) {
//            // IE 11 (or newer) => return version number
//            var rv = ua.indexOf('rv:');
//            return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
//        }
//        return false;
//    })();
//    window.getCookie = function (name) {
//        var value = "; " + document.cookie;
//        var parts = value.split("; " + name + "=");
//        if (parts.length == 2) return parts.pop().split(";").shift();
//    }
//    if (!String.prototype.format) {
//        String.prototype.format = function () {
//            var args = arguments;
//            return this.replace(/{(\d+)}/g, function (match, number) {
//                return typeof args[number] != 'undefined'
//            ? args[number]
//            : match
//                ;
//            });
//        };
//    }
//    if (!String.prototype.parseIntWithDefault) {
//        String.prototype.parseIntWithDefault = function (defaultValue) {
//            var retValue = defaultValue;
//            var str = this;
//            if (str !== null) {
//                if (str.length > 0) {
//                    if (!isNaN(str)) {
//                        retValue = parseInt(str);
//                    }
//                }
//            }
//            return retValue;
//        };
//    }

//    window._nec_ = nec;
//})(window);