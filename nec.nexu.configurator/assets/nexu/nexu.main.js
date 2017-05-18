;
(function ($) {
var ds = new kendo.data.DataSource({
			data : [{
					Thumbnail : 'wall-tn-default',
					FileName : 'wall-default',
					DisplayName : 'Default'
				}, {
					Thumbnail : 'wall-tn-graffiti',
					FileName : 'wall-graffiti',
					DisplayName : 'Graffiti'
				}
    ]
});
var viewerThumbs = new kendo.data.DataSource({
			data : [{
					Thumbnail : 'icon-viewer-front',
					ViewId : 2,
					DisplayName : 'Front'
				}, {
					Thumbnail : 'icon-viewer-right',
					ViewId : 1,
					DisplayName : 'Right Angle'
				}, {
					Thumbnail : 'icon-viewer-back',
					ViewId : 3,
					DisplayName : 'Back'
				}, {
					Thumbnail : 'icon-viewer-left',
					ViewId : 4,
					DisplayName : 'Left Angle'
				}
    ]
})

function goMobile() {
    window.isMobileSize = true;
    //_nexu_.Mobile = new kendo.observable({});
    //_nexu_.Background.goMobile();
    //_nexu_.View.Controller.goMobile();
    //mobile_ConfiguratorList.init();
    //_nexu_.View.Carousel.goMobile();
    //kendo.bind($('#nexu-configurator-module'), _nexu_.Mobile);
}
function goDesktop() {
    window.isMobileSize = false;
    //_nexu_.Background.goDesktop();
    //_nexu_.View.Controller.goDesktop();
    //_nexu_.View.Carousel.goDesktop();
    //$('#nexu-ui-left').insertBefore($('#nexu-ui-mid'));
    //
    //kendo.unbind($('#nexu-configurator-module'), _nexu_.Mobile);
}
function testWindow() {
    /* Check for minimum width */
    var minRequirement = document.documentElement.clientWidth < 480;
		if (minRequirement) {}
    /* Check for mobile width */
    var mobile = document.documentElement.clientWidth < 768;
    if (mobile) {
        if (!window.isMobileSize)
            goMobile();
    }
    /* Check for desktop width */
    var desktop = document.documentElement.clientWidth >= 768;
    if (desktop) {
        if (window.isMobileSize)
            goDesktop();
    }
}
function _setMatching() {
    for (var i = 0; i < _nexu_.Product.Regions.length; i++) {
        var r = _nexu_.Product.Regions[i];
        if (r.Region.MatchingRegion) {
            _nexu_.Matching[r.Region.Id] = true;
        }
    }
}
function ValidatePrimaryLogo(model) {
    var exists = false;
    for (var i = 0; i < model.Product.Logos.length; i++) {
        if (model.Product.Logos[i].Location.IsPrimary && model.Product.Logos[i].DesignColorway) {
            exists = true;
            break;
        }
    }
    if (exists) {
        model.Selected.NeedPrimary = false;
		} else {
        model.Selected.NeedPrimary = true;
    }
}
function change_Hierarchy(e, data) {
    var currentH = _nexu_.Selected.get('Hierarchy');
    if (currentH && currentH.Id != data.Id) {
        _nexu_.Prompt.open({
            message: 'You are about to change the team. This will remove existing logos from the cap. Continue?',
            confirm: function () {
                _nexu_.Hierarchy.Close();

                _nexu_.View.Controller.Widget().clearLogo(
					$.map(_nexu_.Template.DesignLocations, function (obj) {
					    if (obj.Removable)
					        return obj.Code;
						}).join());

                _nexu_.Selected.set('Hierarchy', data);
                _nexu_.LogoCarousel.ResetData();
                var callOptions = null;
                if (!window.isMobileSize) {
                    //callOptions = window.mobileController.GetLogoCallOptions();
                    _nexu_.LogoCarousel.call(callOptions);
                }
                
                return false;
            }
        });
		} else if (currentH == data.Id) {
        /* Same Team */
		} else {
        /* First run */
        _nexu_.Hierarchy.Close();
        _nexu_.Selected.set('Hierarchy', data);
        _nexu_.LogoCarousel.ResetData();
        var callOptions = null;
        if (!window.isMobileSize) {
            //callOptions = window.mobileController.GetLogoCallOptions();
            _nexu_.LogoCarousel.call(callOptions);
        }
        
    }
}
function userSetDispatcher(e) {
    if (e.field === 'Id') {
        _nexu_.ColorPicker.CheckUserColorlist();
    }
}
function setUserId(data) {
    _nexu_.Product.Creator.unbind('change', userSetDispatcher);
    _nexu_.Product.Creator.bind('change', userSetDispatcher);
    if (data) {
        _nexu_.Product.set('Creator.Id', data.Id );
        _nexu_.Display.set('CreatorNickname', data.Nickname);
		} else {
        _nexu_.Product.set('Creator.Id', 1 );
        _nexu_.Display.set('CreatorNickname', '');
    }

}
window.addEventListener('resize', function () {
    testWindow();
});

var _nexu_ = function ($,kendo,_,window,undefined) {
    return {
        Config: {
            ServiceUrl: SVCURL,
				DefaultMenu : {
					Label : 'New Era by You',
					Regions : []
				}
        },
        Modes : {
            CREATE: 0,
            EDIT: 1
        },
        Handlers: {
				Selected : handlers_selected($),
				Template : handlers_template($),
            Save: handler_Save,
            Rendering: handler_Rendering,
            Viewer: function (e, data) {
                var k = _nexu_.ColorPicker;
                _nexu_.Selected.set('Region', data.modelTarget);
                data.xPosition = data.x;
                data.yPosition = data.y;
                var menu = _nexu_.Selected.get('Menu');
                if(menu && menu.Id)// && _nexu_.Display.ClickedMenus.indexOf(menu.Id) == -1)
                    k.Open(data);
                return false;
            },
            ResetMenu: function () {
                _nexu_.Selected.set('Menu', _nexu_.Config.DefaultMenu);
            },
            HierarchyChange: change_Hierarchy
        },
        ConfiguratorMenu: null,
        ColorPicker: module_regioncolorpicker($),
        Display: kendo.observable({
            ColorListTitle: 'Colors',
            AllowColorGrab: true,
            ClickedMenus: []
        }),
        Keywords: new kendo.data.DataSource({
				change : function (e) {}
        }),
        Rendering:handler_Rendering,
        LogoCarousel: module_LogoCarousel,
        LocationMenu: null,
        Background: module_background($,kendo,_,window),
        LogoRecolor: module_LogoRecolor,
        LogoWindow: module_LogoWindow,
        Referrer: module_DeepLink,
        Matching:{},
        Public: false,
        Published:false,
        Prompt: null,
		Product : null,
		/*Observable*/
		Template : null,
		/*Observable*/
        Selected: kendo.observable({
            View: null,
				Menu : {
					Label : 'New Era by You',
					Regions : []
				},
            Region: null,
            Background: null,
            ChangeAll: false,
            Logo: {}
        }),
        View: {
            Carousel: module_viewCarousel($),
            Controller: module_viewer
        },
        Init: function () {
            var t = this;

            t.Prompt = module_prompt;
            /* Intialize Independent Modules */
            _nexu_.LogoCarousel.init().SetClose(_nexu_.Handlers.ResetMenu);
            /*_nexu_.LogoRecolor.init();*/
            _nexu_.ColorPicker.init();
            _nexu_.Hierarchy.element.on('select', change_Hierarchy);

            $('#function_save', '#nexu-ui-functions').on('click', _nexu_.Handlers.Save.CLICK);
            var pId = 94;
            var directLink = 0;
            var inspiredBy = 0;
            if (window.urlParams && window.urlParams.B) {
                pId = window.urlParams.B;
            }
            if (window.urlParams && window.urlParams.DL) {
                directLink = window.urlParams.DL;
            }
            if (window.urlParams && window.urlParams.I) {
                inspiredBy = window.urlParams.I;
            }
            if (window.urlParams && window.urlParams.m == this.Modes.EDIT) {
                this.Mode = this.Modes.EDIT;
				} else {
                this.Mode = this.Modes.CREATE;
            }
            t._LoadProduct(pId)
                .done(function (data) {
                    _nexu_.Product.Creator = kendo.observable({});
                    $('.nexu-bindable').each(function (i, o) {
						var el = $(o),
						t = el.attr('data-obs');
                        kendo.bind(el, _nexu_[t]);
                    });
                    ValidatePrimaryLogo(t);
                    if(inspiredBy)
                        t.Product.InspiredByFinishedProductId = pId;

                    testWindow();
                    
                    module_viewer.LoadViews(data)
                        .done(function (view) {

						if (window.isMobileSize) {
							/* Initialize Mobile */
                                window.mobileController = mobileFunction.call();
                            }

                            module_DeepLink.Process(directLink)
                                .done(function(directData){
                                    /* Intialize  dependent modules */
							_nexu_.View.Carousel.init({
								data : $.map(_nexu_.Template.get('Views').toJSON(), function (v) {
									return {
										Thumbnail : v.ThumbnailPath,
										ViewId : v.Id,
										DisplayName : v.DisplayName
									}
								})
							});

							_nexu_.Background.init({
								autoList : true,
								data : ds
							});
							var locationFilter = (window.isMobileSize) ? {}

							 : {
								CanChoose : true
							};
                                    t.LocationMenu = nexu_DesignLocationSelector
								.Init({
                                            data: _.sortBy(_.where(_nexu_.Template.DesignLocations, locationFilter), 'SortOrder')
                                        });
                                    /* Initialize direct link referral, this should probably changed to a deferred. */
                                    if (urlParams && urlParams.direct) {
                                        _nexu_.Referrer.Load(urlParams.direct);
                                    }
                                    /* Bind Model Event Handlers */
                                    t.Template.bind(CHANGE, _nexu_.Handlers.Template.CHANGE);
                                    t.Selected.bind(CHANGE, _nexu_.Handlers.Selected.CHANGE);
                                    module_viewer.Widget().bind('removeLogo', function (e) {
                                        var logo = $.map(_nexu_.Product.Logos, function (o, i) {
                                            if (o.Location.Code == e.data.key)
                                                return o;
                                        })[0];
                                        logo.set('DesignColorway', null);
                                    }).bind('editLogo', function (e) {
                                        var logo = $.map(_nexu_.Product.Logos, function (o, i) {
                                            if (o.Location.Code == e.data.key)
                                                return o;
                                        })[0];
                                        var selColorway = _nexu_.Selected.get('Logo.DesignColorway');
                                        if (!selColorway) {
                                            _nexu_.Selected.Logo.DesignColorway = logo.DesignColorway;
                                            _nexu_.Selected.Logo.Location = logo.Location;
                                        }
                                        if (logo && logo.DesignColorway) {
                                            _nexu_.LogoRecolor.Open(logo.DesignColorway);
                                        }
                                    });
                                    _setMatching();
                                    /* Process Mobile */
                                    getUser(setUserId);

                                    /* if first visit, open tutorial */
                                    //module_tutorial.init();
                                    /* Make Visible */
                                    //cm temp
                                    $('#nexu-intitialize').fadeOut('slow',function(){
                                        $(this).remove();
                                        $('.initialHide').removeClass('initialHide');
                                        $('#nexu-configurator,#nexu-background,#nexu-ui-menu-bottom').hide().fadeIn(1000);
                                    })

                                })
                        })
                });
        },
        _LoadProduct: function (id) {
            /* Returns deferred object for chaining */
            return $.ajax({
                type: "GET",
                url: SVCURL + "FinishedProducts/configurator/"+id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg && msg.Template) {
                        /* Set the template to the model and intialize left menu */
							msg.Template.Views = _.sortBy(msg.Template.Views, function (x) {
									return x.SortOrder;
								});
                        _nexu_.Template = kendo.observable(msg.Template);
                        _nexu_.Product = kendo.observable({
                            Regions: $.map(msg.Regions,function(o,i){
										return $.extend(o, {
											UserAssigned : false
										});
                            }),
                            Logos:$.map(_nexu_.Template.DesignLocations, function (o, i) {
                                    var cw = $.map(msg.Designs,function(a){
	                                    if(a.Location.Id == o.Id)
		                                    return a.DesignColorway;
                                    })[0];
                                    return {
                                        Location: o,
                                        DesignColorway: cw
                                    }
                            })
                        });
                        _nexu_.Product.set('DisplayName', msg.DisplayName);
                        if (_nexu_.Mode == _nexu_.Modes.EDIT) {
                            _nexu_.Product.set('Id', msg.Id);
                            _nexu_.Product.set('MaterialId', null);
                        }
                        if (msg.Keywords) {
                            for (var i = 0; i < msg.Keywords.length; i++) {
                               _nexu_.Keywords.data().push(msg.Keywords[i]);
                            }
                        }
                        if (msg.Hierarchy)
                            _nexu_.Selected.set('Hierarchy', msg.Hierarchy);
							_nexu_.ConfiguratorMenu = $('div.menu', '#nexu-ui-left').kendoNexuConfiguratorMenu({
									model : _nexu_.Template
								})
							.data('kendoNexuConfiguratorMenu');
                    }
					},
					complete : function () {
						console.log('Product Load Complete');
                }
            });
        },
    };
	}
	(jQuery, kendo, _, window);

(function ($, kendo, _, window,undefined) {
    $(function () {
        window_Publish.Init();
        _nexu_.Hierarchy = module_hierarchy.init();
        _nexu_.Init();

    $('ul', '#nexu-ui-functions').on('mouseover', function (e) {
        var t = $(e.delegateTarget).removeClass('toCollapse');
        if (t.hasClass('collapsed')) {
					t.animate({
						'margin-left' : '0'
					}).removeClass('collapsed');
        }
    }).on('mouseout', function (e) {
        $(e.delegateTarget).addClass('toCollapse')
        setTimeout(function () {
					var el = $('.toCollapse', '#nexu-ui-functions'),
					w = el.width();
					el.animate({
						'margin-left' : (w - 52) + 'px'
					}).removeClass('toCollapse').addClass('collapsed');
        }, 5000);
    });

    $('#nexu-360-view-btn').on('click',module_360_view.Click);

    $('#nexuColorPicker-ColorGrab').on(window.CLICK, function (e) {
        module_ColorGrabber.open();
        return false;
    });
    $('#nexuColorPicker-MatchToFabric').on(window.CLICK, _nexu_.ColorPicker.MatchFabrics);

			$('#nexu-summary-btn').on(window.CLICK, function (e) {
				module_summary.open();
				return false;
			});

    $('.notImplemented').on('click', function (e) {
        alert('Not Implemented');
    });

    $('.addToolTip').kendoTooltip({
        autoHide: true,
        position: 'top',
        animation: false
    });

    var winEl = $('#nexu-download-window'),
        buttons = $('li > a', winEl);
    buttons.on(window.CLICK, function (e) {
        buttons.removeClass('selected');
        $(this).addClass('selected');
        return false;
    });
    $('#nexu-btn-download',winEl).on(window.CLICK,function(e){
				var viewId = _nexu_.Selected.View,
				url = _nexu_.Handlers.Rendering.GetUrl(viewId, true),
				format = buttons.filter('.selected').data('format'),
				backgroundId = _nexu_.Background.Current.Id;
        url += '&bg=' + backgroundId + '&format=' + format;
        window.open(url);
        return false;
    });
    var window_download = winEl.kendoWindow({
        resizable: false,
        draggable: false,
        modal: true,
        visible:false,
        open: function (e) {
            e.sender.center();
            $('.k-window-titlebar', e.sender.element.parent()).addClass('nexu-download-title').css('margin-top', -31);
        }
    }).data('kendoWindow');
    window_download.close();

    $('#function_download').on(CLICK,function(e){
        window_download.open();
        return false;
    })
    $('#function_publish').on(CLICK, function (e) {
        window_Publish.Open();
        return false;
    });
    $('#function_startover').on(CLICK, function (e) {
        location.reload();
        return false;
    });
    $('#function_order').on(CLICK, _nexu_.Handlers.Save.Order);

    var loginDiv = document.createElement('div');
    loginDiv.id = "nx-login-window";
    document.body.appendChild(loginDiv);
    var loginWindow = $(loginDiv).kendoWindow({
        content: '/Login.aspx',
        title: 'New Era by You Login',
        iframe: true,
        width: 420,
        height: 380,
        visible: false,
        modal:true,
        open: function (e) {
            e.sender.element.closest('.k-window').addClass('force-border');
            e.sender.center();
        },
        close: function (e) {
            getUser(setUserId);
        }
    }).data('kendoWindow');

    function getPosition(originElement,newElement, anchor) {
        var newPosition = originElement.offset();
        switch (anchor.toUpperCase()) {
            case 'RIGHT':
                newPosition.left += originElement.outerWidth();
                break;
            case 'LEFT':
                newPosition.left -= newElement.outerWidth();
                break;
            case 'TOP':
                newPosition.top -= newElement.outerHeight();
                break;
            case 'BOTTOM':
                newPosition.top += originElement.outerHeight();
                break;
        }
        return newPosition;
    }

    $('#function_login').on(CLICK, function (e) {
        loginWindow.open();
        return false;
    });

});
	})(jQuery, kendo, _, window);

	
	window._nexu_ = _nexu_;
	
})(jQuery);
