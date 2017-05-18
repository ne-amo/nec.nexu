; window.nexuDevTools = (function ($, kendo, _, window, undefined) {
    var html =
 '<div id="devTools"> <div> <select id="devTools-language"> <option value="en">English</option> <option value="es">Spanish</option> <option value="ja">Japanese</option> </select> <button id="btn-setLanguage">Set</button><button id="btn-resetLang">Reset</button> </div> </div>';

    var CLICK = 'click',
        COOKIE_LANG = 'prefLanguage',
        rID = '#devTools';

    function setCookie(cname, cvalue, exdays, domain,port) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires + ';domain=' + "" + ';port=' + port;
    }
    function deleteCookie(name) {
        document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }
    function _init() {
        var element = document.createElement('div');
        element.innerHTML = html;
        document.body.insertBefore(element, document.body.firstChild)
        $('button#btn-setLanguage', rID).on(CLICK, function (e) {
            var val = $('#devTools-language', rID).val();
            setCookie(COOKIE_LANG, val, 1, 'localhost', '50595');
            return false;
        });
        $('button#btn-resetLang', rID).on(CLICK, function (e) {
            deleteCookie(COOKIE_LANG);
            return false;
        });
        var win = $(element).kendoWindow({
            title: 'Tools',
            visible:false,
        }).data('kendoWindow');
        win.close();
        var map = []; // Or you could call it "key"
        onkeydown = onkeyup = function (e) {
            e = e || event; // to deal with IE
            map[e.keyCode] = e.type == 'keydown';
            if (map[17] && map[16] && map[191]) { // CTRL+SHIFT+? //191 ? 66 B
                win.open();
            }
        }
    }
    _init();
    return {

    }
})(jQuery, window.kendo, _, window);

/**
 *  string format extension like in C#
 */
String.format = function (text) {
    //check if there are two arguments in the arguments list
    if (arguments.length <= 1) {
        //if there are not 2 or more arguments there's nothing to replace -> return the original text
        return text;
    }
    
    var tokenCount = arguments.length - 2;
    for (var token = 0; token <= tokenCount; token++) {
        //iterate through the tokens and replace their placeholders from the original text in order
        text = text.replace(new RegExp("\\{" + token + "\\}", "gi"), arguments[token + 1]);
    }
    return text;
};

function isDarkColor(hexValue)
{
    var darkColor = false;
    var c = hexValue.replace(/^#/, '');
    var rgb = parseInt(c, 16);   // convert rrggbb to decimal
    var r = (rgb >> 16) & 0xff;  // extract red
    var g = (rgb >> 8) & 0xff;  // extract green
    var b = (rgb >> 0) & 0xff;  // extract blue

    var luma = 0.2126 * r + 0.7152 * g + 0.0722 * b; // per ITU-R BT.709


    if (luma < 100)// yup it's dark
    {
        darkColor = true;
    }
    return darkColor;
}


function getQueryStrings() {
    //Holds key:value pairs
    var queryStringColl = new Array();

    //Get querystring from url
    var requestUrl = window.location.search.toString();

    if (requestUrl != '') {
        //window.location.search returns the part of the URL 
        //that follows the ? symbol, including the ? symbol
        requestUrl = requestUrl.substring(1);
        
        //Get key:value pairs from querystring
        var kvPairs = requestUrl.split('&');

        for (var i = 0; i < kvPairs.length; i++) {
            var kvPair = kvPairs[i].split('=');
            queryStringColl[kvPair[0]] = kvPair[1];
        }
    }

    return queryStringColl;
}

//function svg2png() {
//    var svg = document.querySelector("svg");
//    var svgData = new XMLSerializer().serializeToString(svg);

//    var canvas = document.createElement("canvas");
//    var ctx = canvas.getContext("2d");

//    var img = document.createElement("img");
//    img.setAttribute("src", "data:image/svg+xml;base64," + btoa(svgData));

//    img.onload = function () {
//        ctx.drawImage(img, 0, 0);

//        // Now is done
//        console.log(canvas.toDataURL("image/png"));
//    };
//}