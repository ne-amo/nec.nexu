; var handlers_selected = function ($) {
    function closeAll() {
        var viewer = _nexu_.View.Controller.Widget(),
            logos = _nexu_.LogoCarousel;

        viewer.disable();

        if (logos.widget().isOpen())
            logos.Close();

        _nexu_.LocationMenu.Close();
        _nexu_.ColorPicker.Close();
        _nexu_.Hierarchy.Close();
        _nexu_.LogoCarousel.Close();

        _nexu_.ConfiguratorMenu.unselect();
    }
    return {
        RESET: closeAll,
        CHANGE:function(e){
            var val = this.get(e.field),
                viewer = _nexu_.View.Controller.Widget(),
                logos = _nexu_.LogoCarousel;
            switch (e.field) {
                case 'Hierarchy':
                    _nexu_.Background.refresh();
                    break;
                case 'Menu':
                    viewer.disable(); /* Disable the view */
                    _nexu_.Selected.set('ChangeAll', false);
                    if(logos.widget().isOpen())
                        logos.Close(); /* Close the Logo Carousel */
                    //_nexu_.LocationMenu.hide(); /* Hide Location Menu */
                    _nexu_.LocationMenu.Close();
                    if (val == null || val == undefined || val.Id == undefined) {
                        /*  When color or logo menus are closed, the selected menu will be null, resetting the active editor.  */
                        _nexu_.ConfiguratorMenu.unselect();
                        this.set('Location',null);
                        return false;
                    }
                    else if (val.Id === -1) { /* Logos */
                        /* Close color picker  */
                        _nexu_.ColorPicker.Close();
                        _nexu_.Hierarchy.Close();
                        /* If Not, gotta select team */
                        //_nexu_.Hierarchy.Open();
                        _nexu_.LocationMenu.Open();
                    }
                    else {
                        _nexu_.Hierarchy.Close();
                        _nexu_.LogoCarousel.Close();
                        /*  Assumes the ConfiguratorMenu widget sets the ProductRegionGroup associated with the clicked item. */
                        /*  Passes the Codes to enable in the viewer */
                        viewer.enable(_.map(val.Regions.toJSON(), function (obj) { return obj.Code; }), val.ForceGroup);
                        _nexu_.Display.set('ChangeAll',val.AllowRecolorAll);
                        _nexu_.Display.set('MatchToFabric', val.AllowMatching);

                        if (val.ForceGroup) {
                            _nexu_.Selected.set('ForcedGroup', $.map(val.Regions, function (obj) { return obj.Id }));
                        }
                        else {
                            _nexu_.Selected.set('ForcedGroup', null);
                        }

                        if (_nexu_.Display.ClickedMenus.indexOf(val.Id) == -1) {
                            /* First time being clicked */
                            _nexu_.Display.ClickedMenus.push(val.Id)
                            if (val.TemplateViewId)
                                _nexu_.Selected.set('View', val.TemplateViewId);
                        }
                        else {
                            /* Just open the menu */
                        }

                    }

                    break;
                case 'Location':
                    viewer.disable();
                    ///* when the selected location is changed, we get the current properties of the placeholder in the viewer for that logo, 
                    //and initialize the carousel. The view must be loaded into the DOM. */
                    //if(val!=null){

                    ///* If the Product already has a colorway for that logo location, it should open the recolorer? */
                    //    var logo = _.filter(_nexu_.Product.Logos,function(logo){
                    //        return logo.Location.Id == val.Id
                    //        });
                    //   if( logo.length > 0)// && (typeof logo[0].DesignColorway != typeof null))
                    //   {
                    //        _nexu_.Selected.set('Logo', logo[0]);
                    //    }
                    //    var 
                    //        o = viewer.getLogoOffset(val.Code);
                    //        scale = (o.matrix) ? Math.floor(parseFloat(o.matrix.split(',')[0]) * 100) : 1,

                    //        args = {
                    //            centerX: o.left,
                    //            centerY: o.top,
                    //            imageScale: scale
                    //        }
                    //    _nexu_.LogoCarousel.Open(args);
                    //}
                    break;
                case 'Logo':
                    // _nexu_.Selected.set('Location',val.Location);
                    //_nexu_.Selected.set('DesignColorway',val.DesignColorway);
                    break;
                case 'Logo.DesignColorway':
                    if(val != null)
                    {
                        _nexu_.LogoCarousel.Close();
                        if (_nexu_.Display.IsMobile) {
                            var item = {
                                _type: window.MenuTypes.COLORWAY,
                                DisplayName: "Design",
                                _parentType: window.MenuTypes.HIERARCHY,
                                _childType: window.MenuTypes.LAYER,
                                data: val
                            };
                            mobileController.SetList(window.MenuTypes.LAYER, item);
                        }

                        if (!_nexu_.Display.IsMobile) {
                            _nexu_.LogoRecolor.Open(val);
                        }
                    }
                    break;
                case 'Logo.Location':
                    //var 
                    //    o = viewer.getLogoOffset(val.Code);
                    //    scale = (o.matrix) ? Math.floor(parseFloat(o.matrix.split(',')[0]) * 100) : 1,
                    //    args = {
                    //        dataSource:images,
                    //        centerX: o.left,
                    //        centerY: o.top,
                    //        imageScale: scale
                    //    }
                    //_nexu_.LogoCarousel.Open(args);
                    break;
                case 'View':
                    /* Call the ChangeView method on view controller */
                    var v = _.findWhere(_nexu_.View.Views,{Id:val});
                    _nexu_.View.Controller.ChangeView(v,e.sender.Menu);
                    break;
            }
        }
    }
};
function oldlogobreak() {
    _nexu_.LocationMenu.show();
    /* 
    When "Logos" is clicked:
        Set the selected Location to the first that matches the current view id
        Open the logo carousel
    */
    var v = _.findWhere(_nexu_.Template.DesignLocations, { TemplateViewId: _nexu_.Selected.View });
    if (!v) {
        /* If the current view doesn't have an assigned design location, we use the first location */
        v = _nexu_.Template.DesignLocations[0];
        /* For proper flow, the view has to be switched, and once loaded, then the location changed. */
        function setLocation(e) {
            /*$(e.target).off('drawn',setLocation);*/
            _nexu_.Selected.set('Location', v);
            _nexu_.Selected.set('Logo.Location', v);
        }
        viewer.element.one('drawn', setLocation);
        _nexu_.Selected.set('View', v.TemplateViewId);
    }
    else {
        /* If the current view has a location, open it */
        _nexu_.Selected.set('Location', v);
        _nexu_.Selected.set('Logo.Location', v);
    }
}