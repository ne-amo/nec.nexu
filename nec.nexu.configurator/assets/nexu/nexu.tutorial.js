; var nexu_Tutorial = (function ($, _, kendo, window, undefined) {

    var module = $('#nexu-tutorial', '#nexu-configurator-module');
    var controls = $('.nx-tutorial-controls', module);
    var welcomeMessage = $('.nx-tutorial-welcome', controls);
    var totalTips = 0;
    function show() {
        module.show();
    }
    function hide() {
        module.hide();
    }
    function showTip(number) {
        var m = module;
        $('.nx-tutorial-item', m).hide().filter('div[data-tip="' + number + '"]').show();
        m.data('current-tip', number);
        $(".nx-tutorial-current", controls).text(number);
    }
    function positionTip(element) {
        var el = $(element), t = el.data('top'), l = el.data('left'), r = el.data('right'), b = el.data('bottom'), css = {};
        if (t)
            css.top = t;
        if (l)
            css.left = l;
        if (r)
            css.right = r;
        if (b)
            css.bottom = b;
        el.css(css);
        el.hide();
    }
    function click_Previous(e) {
        var current = module.data('current-tip');
        if (current != 1)
            showTip(--current);
        return false
    }
    function click_Next(e) {
        var current = module.data('current-tip');
        if (current < totalTips)
            showTip(++current);
        return false;
    }
    function setWelcome() {
        if(_.isFunction(window.getCookie)){
            var beenHere = window.getCookie('nx-visit');
            if (beenHere) {
                welcomeMessage.hide();
                hide();
                return;
            }
        }
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + 365*10);
        document.cookie = 'nx-visit=' + kendo.guid() + '; expires='+exdate.toUTCString()+';';
    }
    function formatArrow(container) {
        var c = $(container);
        var arrow = $('.nx-tutorial-callout', container);
        var v = 0, att = 'top';
        if (arrow.hasClass('k-callout-w') || arrow.hasClass('k-callout-e')) {
            v = c.outerHeight()/2, att = 'top';
        }
        if (arrow.hasClass('k-callout-n') || arrow.hasClass('k-callout-s')) {
            v = c.outerWidth()/2, att = 'left';
        }
        arrow.css(att, v - 20);
    }

    function _init() {
        var m = module;
        c = m.parent(), containerDimensions = { h: c.height(), w: c.width() };
        m.height(containerDimensions.h);
        $('.nx-tutorial-item', m).each(function (index, element) {
            positionTip(element);
            formatArrow(element);
            totalTips++;
        });
        controls.css({
            top: ((containerDimensions.h * 0.5) - (controls.outerHeight() * 0.5)) - 20,
            left: (containerDimensions.w * 0.5) - (controls.outerWidth() * 0.5)
        })
        .on(CLICK, '.nx-tutorial-prev', click_Previous)
        .on(CLICK, '.nx-tutorial-next', click_Next)
        .on(CLICK, 'a.nx-tutorial-close', function (e) {
            hide();
            return false;
        });
        $(".nx-tutorial-total", controls).text(totalTips);
        showTip(1);
    }
    _init();
    setWelcome();

    return {
        Element:function(){
            return module;
        },
        Show: function () {
            showTip(1);
            show();
            return this;
        },
        Hide: function () {
            hide();
            return this;
        }
    }
})(jQuery, _, kendo, window);
