; nexu_translator = (function ($, window, undefined) {
    var file = '/assets/nexu/nexu.locales.0.json',
        query = '[class*=local_]',
        fields = {
            TEXT: 'text',
            TITLE: 'title'
        };

    function getDictionary(lang) {
        return $.ajax({
            dataType: "json",
            url: file.replace('0', lang)
        });
    }
    function setText(element,dictionary) {
        var t = element.textContent.replace(' ', '_');
        var val = dictionary[t.toUpperCase()];
        if(val)
            element.textContent = val;
    }
    function setTitle(element,dictionary) {
        var t = element.getAttribute(fields.TITLE).replace(' ', '_');
        var val = dictionary[t.toUpperCase()];
        if(val)
            element.setAttribute(fields.TITLE,val);
    }
    function translate(lang, object) {

        var deferred = $.Deferred();

        function resolved(dictionary) {
            window.nexu_dictionary = dictionary;// = d;
            var all = $(query, object);
            all.each(function (i, o) {
                if (o.className.indexOf(fields.TEXT) > -1) {
                    setText(o, dictionary);
                }
                if (o.className.indexOf(fields.TITLE) > -1) {
                    setTitle(o, dictionary);
                }
            });
            deferred.resolve();
        }

        if (!window.nexu_dictionary)
            getDictionary(lang).done(resolved);
        else
            resolved(window.nexu_dictionary);

        return deferred;
    }

    return {
        Translate: function (lang, container) {
            return translate(lang, container);
        }
    }
})(jQuery, window);