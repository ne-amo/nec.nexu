; var handler_designrules = function ($) {
    var types =
    {
        NONE: 'NONE',
        REQUIREDLOGO: 'REQUIREDLOGO',
        COLORRESTRICTION: 'COLORRESTRICTION',
        COLORWAY: 'COLORWAY',
        DIFFERENTIATION: 'DIFFERENTIATION'
    },
    ajax_Colorway = {
        type: "POST",
        url: SVCURL + "designs/checkcolorway/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg) {
                return msg;
            }
            else {
                return null;
            }
        },
        error: function (a, b, c) {
        },
        complete: function () {
        }
    };
    function
        _Rule_Differentiation(rule) {
        /* Get Colorway */
        var colorids = $.map(_nexu_.Selected.get('Logo.DesignColorway.Layers'),function(l){
            if(l.Recolorable)
                return l.Color.Id;
        }),
        num = _.uniq(colorids).length; /* Number of unique colors */
        return num >= rule.RequiredColors; /* Whether or not the number of colors in logo meets the rule. */
    }
    function _Rule_Colorway() {
        var colorway = _nexu_.Selected.get('Logo.DesignColorway');
        var call = $.extend({}, ajax_Colorway, { url: ajax_Colorway.url + colorway.Design.Id, data: kendo.stringify(colorway) });
        return $.ajax(call);
    }
    function _getTypes() {
        return types;
    }
    return {
        Types:function(){
            return _getTypes();
        },
        callbacks:{
            DIFFERENTIATION: function () {
                alert("This logo requires " + rule.RequiredColors + " different colors.");
            }
        },
        ApplyRule: function(rule)
        {   /*
            if (!rule)
                return;
            if(rule.Type == types.DIFFERENTIATION)
            {
                if (!this.Rule_Differentiation(rule))
                    return {
                        output: false, callback: function () {
                            alert("This logo requires " + rule.RequiredColors + " different colors.");
                        }
                    };
            }
            if (rule.Type == types.COLORWAY) {
                return this.Rule_Colorway();
                //if (ret) {
                //    return {
                //        output: false, callback: function () {
                //            alert("Due to licensing restrictions, this color combination is not allowed.");
                //        }
                //    }
                //}
                //else {
                //    return {
                //        output: true
                //    }
                //}
            }
            return null;
            */
        }
    };
}(jQuery);