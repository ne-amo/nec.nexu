; var module_summary = (function ($, kendo, _, window, undefined) {;
    var container = $('#nexu-summary').kendoWindow({
        title: 'Product Summary',
        modal: true,
        height: 420,
        resizable: false,
        visible:false,
        open: function (evt) {
            evt.sender.center();
        }
    }).data('kendoWindow');
    container.close();

    function summarize() {
        var groupHtml = '<h4>{0}</h4><ul class="nexu-summary-list">{1}</ul>';
        var regionHtml = '<li><div class="nexu-lft"><div class="nexu-summary-swatch" style="background-color:\\##= Hex #;">&nbsp;</div> </div> <div class="nexu-right"> <p class="nexu-summary-name">#: Name #</p> <p class="nexu-summary-description">#: Description #</p> </div> </li>';
        //var logoGroupHtml = '<h4>{0} - {2}</h4><div class="nexu-summary-logo-preview"> <div></div> </div> <ul class="nexu-summary-list">{1}</ul>'
        var logoHtml = '<li><div class="nexu-lft"><div class="nexu-summary-swatch" style="background-color:\\##= Hex #;">&nbsp;</div> </div> <div class="nexu-right">' +
            '<p class="nexu-summary-name">#: Name #</p> <p class="nexu-summary-description">#: Description #</p> </div> </li>';

        var logoGroupHtml = '	<h4>{0}</h4>' +
                            '	<h5>{2}</h5>' +
                            '	<div>' +
                            '		<div class="nexu-summary-logo-preview"><div></div></div>' +
                            '		<ul class="nexu-summary-list logos">{1}</ul>' +
                            '	</div>';
        var button = $("<div><a href='#' class='nx-save k-button'>Download Summary</a></div>");
        var tmpRegion = kendo.template(regionHtml);
        var tmpLogo = kendo.template(logoHtml);
        var groups = _nexu_.Template.RegionGroups.toJSON();
        var c = container.element.empty();
        c.append(button).on('click', '.nx-save', function (e) {
            var t = $(e.target);
            if (t.hasClass('nx-save')) {
                //$(container.element).addClass('show-scroll-content');
                _render(container.element);
            }
            return false;
        });
        var logos = _nexu_.Product.Logos.toJSON();

        for (var i = 0; i < groups.length; i++) {
            var group = groups[i];
            var div = document.createElement('div');
            if (group.Id != -1) {
                var regionIds = $.map(group.Regions, function (obj) {
                    return obj.Id;
                });
                /* Regions */
                var regions = kendo.render(tmpRegion, $.map(_nexu_.Product.Regions, function (obj) {
                    if (regionIds.indexOf(obj.Region.Id) > -1)
                    {
                        var x = {};
                        x.Hex = obj.Material.Hex;
                        x.Name = obj.Region.DisplayName;
                        x.Description = obj.Material.DisplayName;
                        if (obj.Region.ShowFabricContent)
                            x.Description = x.Description + ' - ' + obj.Material.FabricContent.DisplayName;

                        return x;
                    }
                    
                }));
                div.innerHTML = groupHtml.replace('{0}', group.DisplayName).replace('{1}', regions);
                c.append(div);
            }
        }

        var row = $('<div></div>').addClass('row').appendTo(c);

        for (var i = 0; i < logos.length; i++) {
            if (!logos[i].DesignColorway) {
                continue;
            }
        
            var logo = logos[i];
            var div = document.createElement('div');
        
            var regions = kendo.render(tmpLogo, $.map(logo.DesignColorway.Layers, function (obj) {
                if (obj.Recolorable)
                    return {
                        Hex: obj.Color.Hex,
                        Name: obj.Color.DisplayName,
                        Description: obj.DisplayName
                    };
            }));
            div.innerHTML = logoGroupHtml.replace('{0}', logo.Location.DisplayName).replace('{1}', regions).replace('{2}', logo.DesignColorway.Design.TapeNumber);
        
            var p = $(div).addClass('col-xs-4').appendTo(row).find('.nexu-summary-logo-preview div');

            p.kendoNexuLogoPreview({
                dataSource: logos[i].DesignColorway.Layers,
				getContainerDimensions: function(element){
					return p.width();
				},
				setMaxDimensions:false
            });

        } // close logo loop


    }
    function _close() {

    };
    function _open() {
        summarize();
        container.open();
    }

    function _render(markup) {

        var DRAWING = kendo.drawing,
            PROXY_URL = "http://demos.telerik.com/kendo-ui/service/export";

        var glob = {
            win: window,
            doc: window.document
        };

        var options = {
            fileName : 'NExU-'+new Date().getTime(),
            ext :  'pdf',
            pageSize: ['11in', '8.5in'],
            landscape : false
        }

        function render(target, options) {

            var f = $('#nexu-summary-temp'),//glob.doc.createDocumentFragment(),
                div = glob.doc.createElement("div");

            div.innerHTML = $(target).clone().html();
            f.append(div);
            return kendo.drawing.drawDOM(f)
			.then(function (group) {
			    return DRAWING.exportPDF(group,{
			        paperSize: ['11in', '8.5in'],
			        landscape: false,
			        multiPage:true

			    });
			});

        }

        render(markup, options)
		    .done(function (data) {
			    kendo.saveAs({
			        dataURI: data,
			        fileName: options.fileName + options.ext,
			        proxyURL: PROXY_URL
			    });
			    //$(container.element).removeClass('show-scroll-content');
		    });


    }

    return {
        open: function () {
            _open();
        },
        close: function () {
            _close();
        },
        save: function () {
            _render(container.element);
        }
    }
})(jQuery, kendo, _, window);