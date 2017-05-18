/* EXPOSE THE SCREEN DIMENSIONS FOR OTHER FUNCTIONS */
var exposeScreen = (function () {
    return {
        Width: function () { return Number(window.innerWidth); },
        Height: function () { return Number(window.innerHeight); },
        Ratio: function () { return Number((window.innerHeight / window.innerWidth).toFixed(2)); /* < 1 = landscape */ }
    };
}());
var breakPoints = (function () {
    var desktop = 1272,
        tablet = 1024,
        mobile = 600,
        width = 1,
        height = 1,
        ratio = 1,
        tag = jQuery('html');
    return {
        Set: function () {
            width = exposeScreen.Width(),
            height = exposeScreen.Height(),
            ratio = exposeScreen.Ratio(),
            tag.removeClass('bp-portrait bp-landscape bp-desktop bp-desktop-min bp-desktop-max bp-tablet bp-mobile bp-not-mobile bp-not-desktop');
            if (ratio > 1) {
                tag.addClass('bp-portrait');
            } else {
                tag.addClass('bp-landscape');
            }
            if (width > desktop) {
                tag.addClass('bp-desktop bp-desktop-max');
            } else if (width > tablet) {
                tag.addClass('bp-desktop bp-desktop-min');
            } else if (width > mobile) {
                tag.addClass('bp-tablet');
            } else {
                tag.addClass('bp-mobile');
            }
            if (width > mobile) {
                tag.addClass('bp-not-mobile');
            }
            if (width <= desktop) {
                tag.addClass('bp-not-desktop');
            }
        },
        maxWidth: function () {
            return desktop;
        },
        tablet: function () {
            return tablet;
        }
    };
}());