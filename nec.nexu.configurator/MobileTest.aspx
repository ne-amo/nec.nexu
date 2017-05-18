<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MobileTest.aspx.cs" Inherits="nec.nexu.configurator.MobileTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1">

</head>
<body>
    <div class="container">
        <div>

<link href="/assets/css/reset.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/kendo/styles/kendo.common-bootstrap.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/kendo/styles/kendo.flat.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
<!--<link href="/assets/kendo/styles/kendo.bootstrap.min.css" rel="stylesheet" type="text/css" /> -->
<link href="/assets/css/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.mobile.css" rel="stylesheet" />
<style type="text/css">
    #nexu-loader img {
        opacity:0.8;
        margin-right:8px;
        width:30px;
    }
    #nexu-loader {
        font-weight: bold;
        color: #fff;
        font-size: 1.25em;
        position: absolute;
        text-align: center;
        width: 100%;
        top: 0;
        left: 0;
        line-height: 30px;
        vertical-align: middle;
        padding-top: 12px;
    }
</style>
            

<div id="nexu-configurator-module" class="nexu-ui initialHide mobileWidth mobileHeight">
    <div id="nexu-right-drawer" style="width: 270px; background-color:#000; right:0; left:auto;" class="nexu-mobile-menu-config mobile-only">
    <h3 class="nav-current"><a href="#">New Era By You</a></h3>
        <ul class="nav-menu"></ul>
        <ul class="nav-menu-sub"></ul>
        <ul class="nav-menu-colorlist"></ul>
        <ul class="nav-options"></ul>
    </div>


    <!-- container -->

    <div id="nexu-background" class="mobileHeight mobileWidth">
        <div id="nexu-background-top" class="row"></div>
        <div id="nexu-background-bottom" class="row"></div>
    </div>
    <div id="nexu-configurator" class="row  mobileWidth mobileHeight"><!-- style="display:none;"> -->
        <div id="nexu-ui-top" class="row desktop-only">
            <div id="nexu-live-description" class="col-sm-10 nexu-bindable" data-obs="Selected" data-bind="text: Menu.Label"></div>
        </div>

        <div id="nexu-ui-left" class="col-sm-2">
            <div id="nexu-tutorial-tut1" class="nexu-tutorial-tip"></div>
            <div class="menu desktop-only"></div>
        </div>

        <div id="nexu-ui-mid" class="col-sm-8" style="position: relative;">
            <div id="configurator-container">
                <div id="nexu-configurator-viewer"></div>
            </div>
        </div>
        <div id="nexu-ui-right" class="col-sm-2 desktop-only">
            <div id="nexu-product-description">
                <div>
                    <p class="main nexu-bindable" data-obs="Template" data-bind="text: DisplayName">59FIFTY</p>
                    <p class="sub nexu-bindable"  data-obs="Template" data-bind="text: PriceDescription">$57.99 w/ S&H</p>
                </div>
            </div>
            <div id="nexu-ui-functions">
                <ul>

                    <li id="function_order">
                        <div><span class="icon icon-20b-order"></span><span class="nexu-label local_text">Place Order</span></div></li>
                    <li id="function_download"><div>
                        <span class="icon icon-20b-dropper"></span><span class="nexu-label local_text">Download</span></div></li>
                    <li id="function_publish"><div>
                        <span class="icon icon-20b-list1"></span><span class="nexu-label local_text">Publish</span></div></li>
                    <li id="function_save"><div>
                        <span class="icon icon-20b-save"></span><span class="nexu-label local_text">Save Progress</span></div></li>
                    <li id="function_startover"><div>
                        <span class="icon icon-20b-reset"></span><span class="nexu-label local_text">Start Over</span></div></li>
                </ul>
                <span id="nexu-tutorial-tut2" class="nexu-tutorial-tip"></span>
            </div>
        </div>

        <div class="mobile-only nexu-mob-btn menu-config"><a href="#"><span class="icon icon-40b-menu"></span></a></div>
        <div class="mobile-only nexu-mob-btn menu-options"><a href="#"><span class="icon icon-40b-order"></span></a></div>


    </div>

    <div id="nexu-ui-menu-bottom" class="row">
        <div style="text-align:center;width: 100%;height: 0;"><span id="nexu-tutorial-tut3" class="nexu-tutorial-tip">*nbsp;</span></div>
        <div id="nexu-ui-wall-selector" class="col-sm-2 desktop-only">
            <div></div>
        </div>
        <div id="nexu-ui-viewer-selector" class="col-sm-8">
            <ul ><!--class="desktop-only">-->
                <li class="desktop-only">
                    <div id="nexu-360-view-btn" class="icon icon-40b-refresh addToolTip local_title" title="360 View">&nbsp;</div>
                </li>
                <li class="viewer-mask">
                    <div>
                        <ul class="viewer-list"></ul>
                    </div>
                </li>
                <li class="desktop-only">
                    <div  id="nexu-summary-btn"class=" icon icon-40b-preview addToolTip local_title" title="Product Summary">&nbsp;</div>
                </li>
            </ul>
        </div>
        <div id="nexu-ui-tips-selector" class="col-sm-2 desktop-only">
            <div></div>
        </div>
    </div>

</div>


<div id="nexuColorPicker" class="drag-container initialHide" style="position: absolute; top: 0; left: 0; width: 100%; height: 1px;">
    <div id="nexuColorWheel" class="draggable">
        <div class="nexu-wheel-wrap">
            <div class="wheel-container"></div>
        </div>
        <div class="nexu-close">
            <span class="icon icon-20b-close"></span>
        </div>
    </div>
    <div id="nexuWheelMenu">
        <div class="horizontal vCenter">
            <img src="/assets/img/color_icon.png" alt="New Era by You Colors" /></div>
        <div class="horizontal vCenter">
            <div class="title"><span data-bind="text: ColorListTitle" data-obs="Display" class="nexu-bindable">Colors</span> <a href="#" title="Expand">-</a></div>
            <div id="nexuColorPicker-ColorGrab" class="nexu-bindable" data-bind="visible: AllowColorGrab" data-obs="Display">
                <a href="#" title="Upload an Image">Color Grabber</a></div>
            <ul class="menu"></ul>
            <div id="nexuColorPicker-ChangeAll" class="nexu-bindable" data-bind="visible: ChangeAll" data-obs="Display">
                <input type="checkbox" class="nexu-bindable local_text" data-obs="Selected" data-bind="checked: ChangeAll" />
                Make all panels the same
            </div>
            <div id="nexuColorPicker-MatchToFabric" class="option nexu-bindable local_text" data-bind="visible: MatchToFabric" data-obs="Display">Match To Fabric</div>
            <div id="nexuWheelPager"><span class="pager"></span></div>
        </div>
    </div>
</div>
<div id="nexu-logo-location-menu"  class="initialHide">
    <div class="nexu-location-title">Select Location</div>
    <ul class="nexu-location-menu"></ul>
</div>

<div id="nexu-logo-wheel" data-scale="width,height" class="initialHide">

    <div class="nexu-mask">
        <div class="nexu-list"></div>

    </div>

    
    <div class="nexu-info-box">
        <div class="nexu-counter"></div>
        <div class="nexu-view-all"><a href="#" title="View All">VIEW ALL</a></div>
        <div class="nexu-select"><a href="#" title="Select Logo">RECOLOR LOGO</a></div>
    </div>
    <div class="nexu-border" data-scale="borderRadius"></div>
    <div class="nexu-left" data-scale="lineHeight">
        <span class="icon icon-40w-left" data-scale="lineHeight"></span>
    </div>
    <div class="nexu-right" data-scale="lineHeight">
        <span class="icon icon-40w-right" data-scale="lineHeight"></span>
    </div>
    <div class="nexu-close">
        <span class="icon icon-20b-close"></span>
    </div>
</div>


<script id="template-LogoLayer" type="text/x-kendo-template">
            <tr data-uid="#= uid #" title="#: Color.DisplayName #">
                <td style="background-color:\##: Color.Hex.replace("\#","") #; color:\##: Color.TextColor #;">#: DisplayName #</td>
                <td width="60" class="nexu-recolor-button">Modify</td>
            </tr>
</script>

<div id="nexu-hierarchy" class="initialHide">

    <script id="nexu-hierarchy-template-child" type="text/x-kendo-template">
            #if(data.Icon)
             {#
                 <li>
                    <div class="imageSwatch"></div>
                    <div>#: data.DisplayName #</div>
                </li>
            #}else{#
                 <li>
                    <div>#: data.DisplayName #</div>
                </li>
             # }#
    </script>
    <script id="nexu-hierarchy-template-parent" type="text/x-kendo-template">
            #if(data.Icon)
                {#
                <li>
                    <span class="btn-reset">-</span>
                    <div class="imageSwatch"></div>
                    <div>#: data.DisplayName #</div>
                </li>
            #}else{#
                <li>
                    <div><span class=" class="btn-reset"">-</span> #: data.DisplayName #</div>
                </li>
                # }#
    </script>

    <ul id="nexu-hierarchy-parent"></ul>
    <div class="mask">
        <ul id="nexu-hierarchy-children"></ul>
    </div>
    <div class="nexu-hierarchy-continue">Continue</div>
    <div class="nexu-close">
        <span class="icon icon-20b-close"></span>
    </div>
</div>

<div id="nexu-logo-window" class="initialHide">
    <div class="nexu-window-container">
        <ul class="nexu-window-list">
        </ul>
    </div>
</div>

<div id="nexu-prompt" class="initialHide nexu-window">
    <div class="nexu-prompt-message"></div>
    <div class="nexu-prompt-buttons">
        <a href="#" class="k-button nexu-prompt-confirm">Confirm</a>
        <a href="#" class="k-button nexu-prompt-decline">Cancel</a>
    </div>
</div>

<div id="nexu-colorgrab-window" class="initialHide">
    <div>
        <div id="nexu-colorgrab">
            <div class="desktop-only">
                <div class="image-container">
                    <img alt="preview" />
                </div>
                <div class="content-sections">
                    <div class="nexu-colorgrab-top table-row">
                        <div class="nexu-colorgrab-section nexu-colorgrab-info  nexu-colorgrab-init  nexu-colorgrab-phase">
                            <div class="hd1">Want to match you cap with your clothes?<br />
                                Extract their color and hookup your look.</div>
                            <div class="sb1">Upload your image to extract colors</div>
                        </div>
                        <div class="nexu-colorgrab-results nexu-colorgrab-phase nexu-colorgrab-section nexu-colorgrab-info">
                            <div class="hd1">We grabbed these colors for you<br />and converted them to the closest<br />equivalent new era fabric.</div>
                        </div>
                    </div>
                    <div class="nexu-colorgrab-mid table-row">
                        <div class="nexu-colorgrab-section">
                            <div class="nexu-colorgrab-upload nexu-colorgrab-phase nexu-colorgrab-init">
                                <div>
                                    <div class="nexuUploadDropzone1 desktop-only">
                                        <p><img class="dz-clickable" src="/assets/img/image-drop-icon.png" /></p>
                                        <p class="colorgrab-label"><span class="dz-clickable">Drop Image Here</span></p>
                                    </div>
                                </div>
                            </div>
                            <div class="nexu-colorgrab-results  nexu-colorgrab-phase nexu-colorgrab-results">
                                <ul></ul>
                                <p>Name this set: <input type="text" class="k-input nexu-colorgrab-name" /></p>
                            </div>
                        </div>
                    </div>
                    <div class="nexu-colorgrab-bottom table-row">
                        <div class="nexu-colorgrab-section nexu-colorgrab-info  nexu-colorgrab-phase nexu-colorgrab-init">
                            <div class="sb2">Images must be jpg format and no larger than 500k</div>
                        </div>
                        <div class="nexu-colorgrab-results  nexu-colorgrab-phase">
                            <div class="nexuUploadDropzone2  dz-clickable">
                                 <p><img src="/assets/img/image-drop-icon.png" height="48" style="margin-right:8px;" />
                                     <span class="colorgrab-label">Drop Image Here</span></p>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <div class="mobile-only">
                <div class="content-sections">
                    <div class="stage table-row" data-stage="1">
                        <div>Image <br />
                        <a href="#" class="k-button nexu-cb-mob-upload">Upload Photo</a></div>
                        <div>*Message about any devices that do not support uploading a photo into this process.</div>
                    </div>
                    <div class="stage table-row" data-stage="2">
                        <div><a href="#" class="k-button nexu-cb-mob-take">
                            <input class="mobile-only" type="file" name="files" accept="image/*" capture="camera" /></a>
                        </div>
                        <div class="nexu-cb-mob-browse">
                            <a href="#" class="k-button dz-clickable">Choose Existing</a>
                        </div>
                        <div><a href="#" class="k-button nexu-cb-mob-cancel">Cancel</a></div>
                    </div>
                    <div class="stage table-row" data-stage="3">
                        <div class="image-container">
                            <img alt="preview" />
                        </div>
                        <div><a href="#" class="k-button nexu-cb-mob-back" data-back-to="2">Upload New Photo</a> <a href="#" class="k-button nexu-cb-mob-grab">Grab Colors</a></div>
                    </div>
                    <div class="stage table-row" data-stage="4">
                        <div>We grabbed these colors for you and converted them<br />to the closest equivalent New Era swatch.</div>
                        <div class="nexu-colorgrab-results"><ul></ul></div>
                        <div><a href="#" class="k-button nexu-cb-mob-back" data-back-to="2">Upload New Photo</a> <a href="#" class="k-button nexu-cb-mob-load">Load Colors</a></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/x-kendo-template" id="nexu-tmpl-colorgrabber-result">
            <li><div class="nexu-colorgrabber-swatch" style="background-color:\##: Hex #;" title="#: DisplayName #"></div>
    </script>
    <script type="text/x-kendo-template" id="nexu-tmpl-colorgrabber-preview">
            <div class="dz-preview dz-file-preview">
            <img data-dz-thumbnail />
            <div style=""display:none;">
              <div class="dz-details">
                <div class="dz-filename"><span data-dz-name></span></div>
                <div class="dz-size" data-dz-size></div>
              </div>
              <div class="dz-progress"><span class="dz-upload" data-dz-uploadprogress></span></div>
              <div class="dz-success-mark"><span>✔</span></div>
              <div class="dz-error-mark"><span>✘</span></div>
              <div class="dz-error-message"><span data-dz-errormessage></span></div>
            </div>
            </div>
    </script>
</div>
<div id="wheelTip" class="doughnutTip initialHide"></div>
<div id="nexu-360-view"><div class="view-spin"></div></div>

<div id="nexu-summary" class="nexu-summary row initialHide" style="width:720px;">
    <div id="nexu-summary-left" class="col-sm-2">
        <ul class="nexu-summary-tabs">
            <li>Panels</li>
            <li>Logos</li>
            <li>Visor</li>
            <li>Eyelets</li>
            <li>Stitching</li>
            <li>Button</li>
        </ul>
    </div>
    <div id="nexu-summary-right" class="col-sm-10">
    </div>
</div>

<style type="text/css">
    .nexu-tutorial-window {
        text-align:center;
    }
    .nexu-tutorial-window ul.nexu-tutorial-list {
        display:none;
    }
    .nexu-tutorial-tip {
        /*display:none;*/
        opacity:0;
    }
</style>


<div id="nexu-tutorial" class="nexu-tutorial-window nexu-modal-window" style="display:none;">
    <h3>HERE ARE A FEW STEPS<br />TO GET YOU STARTED</h3>
    <h4><a href="#" class="nexu-tutorial-prev" title="Previous"><span class="icon icon-40w-left"></span></a>
        <span class="nexu-tutorial-current"></span> of <span class="nexu-tutorial-total"></span>
        <a href="#" class="nexu-tutorial-next" title="Next"><span class="icon icon-40w-right"></span></a>
    </h4>
    <ul class="nexu-tutorial-list">
        <li data-translate-key="TUT1" data-title="Customizing" data-position="left" data-anchor="tut1">These are areas of your cap that you can customize. Choose from the list, then click on the area of the cap to modify it.</li>
        <li data-translate-key="TUT2" data-title="Saving" data-position="bottom" data-anchor="tut2">All done? Publish to the gallery. Not done yet? Save and finish later. New idea? Start over.</li>
        <li data-translate-key="TUT3" data-title="Previewing" data-position="top" data-anchor="tut3">Check out your cap design from different angles. You can even generate a 3D view.</li>
    </ul>
</div>


<div id="nexu-download-window" class="initialHide  nexu-modal-window">
    <h2>Download your design:</h2>
    <h3>Any pertinent information can go here.</h3>
    <div>
        <div class="fLeft">
        <!-- left side -->
            <p>Customize with your name:</p>
            <p>
                <input type="text" id="nexu-download-input-name" class="k-input nexu-publish-input-name nexu-bindable" data-bind="value: DisplayName" data-obs="Product" />
            </p>
        </div>
        <div class="fLeft tCenter">
        <!-- right side -->
            <ul id="nexu-download-modes">
                <li><a href="#" class="k-button" data-format="1">Desktop <br /> 1920 x 1200</a></li>
                <li><a href="#" class="k-button">iPad <br /> 1024 x 768</a></li>
                <li><a href="#" class="k-button">iPhone <br /> 640 x 960</a></li>
            </ul>
            <div>
                <a href="#" id="nexu-btn-download" class="k-button">Download</a>
                <a href="#" id="nexu-btn-email" class="k-button">Email</a>
            </div>
        </div>
        <div class="clearBoth">&nbsp;</div>
    </div>
</div>

<div id="nexu-publish-window" class="initialHide nexu-modal-window">
    <h2>Publish to the Gallery</h2>
    <h3>Your current progress will be saved upon publishing.</h3>
    <div>
        <div class="fLeft">
        <!-- left side -->
            <div>
                <p>Name your cap:</p>
                <p>
                    <input type="text" class="k-input nexu-publish-input-name nexu-bindable" data-bind="value: DisplayName" data-obs="Product" /></p>
            </div>
        </div>
        <div class="fLeft">
        <!-- right side -->
            <div>
                <p>Messaging about adding keywords. List some generic examples.</p>
                <p><input type="text" class="k-input nexu-input-publish-tag" maxlength="24" /><a href="#" class="k-button nexu-btn-add">Add</a></p>
                <ul class="nexu-list-tags" style="max-width:409px;"></ul>
            </div>
            <div class="tCenter clearBoth">
                <a href="#" class="k-button nexu-btn-publish">Publish</a>
                <a href="#" class="k-button nexu-btn-cancel">Cancel</a>
            </div>
        </div>
        <div class="clearBoth">&nbsp;</div>
    </div>
</div>
<div id="nexu-save-prompt" style="display:none;">
<div>Please name your cap: <input id="inp_ProductName" type="text" class="k-input nexu-bindable" data-bind="value: DisplayName" data-obs="Product" /></div>
</div>
<script type="text/x-kendo-template" id="mobile_template_basic">
#if(data.Pattern)
 {#
    <li class="nexu-mob-colortile"><a style="background-image:url(#= data.Pattern.Thumbnail.FilePath #);"></a></li>
#}else if(data.Hex) {#
    <li class="nexu-mob-colortile"><a style="background-color:\##= data.Hex #;"></a></li>
#}else{#
    <li>#= data.DisplayName # <span class='icon icon-20w-right'></span></li>
 # }#
</script>
<script type="text/x-kendo-template" id="template_summary_row">
#if(data._type == 'logo')
    {#
        <tr> 
            <td>
                <h4>#: data.LocationDisplayName  # - #: data.DesignDisplayName #</h4>
                <div class="nexu-summary-logo-preview"><div></div></div>
                <ul class="nexu-summary-list"></ul>
            </td>
            <td></td>
        </tr>
#}else{#
        <tr>
            <td>
                <div class="nexu-lft">
                    <div class="nexu-summary-swatch" style="background-color:\\##= data.item1.Hex #;">&nbsp;</div>
                </div>
                <div class="nexu-right">
                    <p class="nexu-summary-name">#: data.item1.Name #</p>
                    <p class="nexu-summary-description">#: data.item1.Description #</p>
                </div>
            </td>
            <td>
                <div class="nexu-lft">
                    <div class="nexu-summary-swatch" style="background-color:\\##= data.item2.Hex #;">&nbsp;</div>
                </div>
                <div class="nexu-right">
                    <p class="nexu-summary-name">#: data.item2.Name #</p>
                    <p class="nexu-summary-description">#: data.item2.Description #</p>
                </div>
            </td>
        </tr>
#}#
</script>
<script type="x-kendo-template" id="nexu-mobile-menu-template">
#if(data._type == window.MenuTypes.MATERIAL || data._type == window.MenuTypes.THREAD)
{#

    <li class="nexu-mob-colortile"><div style="background-color:\##= data.data.Hex #;"><a class="nav-item" href="\\#">&nbsp;</a></div></li>

#}else if(data._type == window.MenuTypes.LAYER)
{#
    <li><a href="\\#" class="icon icon-20b-close mob-icon"></a><a class="nav-item" href="\\#">
        <div class="nexu-mob-colortile" style="background-color:\##= data.data.Color.Hex #;">&nbsp;</div> #: data.DisplayName #</a></li>

#}else if(data._type == window.MenuTypes.BACKGROUND)
{#
    <li class="nexu-mob-background-menu"><a href="\\#" class="nav-item"><img src="http://neccmsdev.neweracap.com/assets#= data.data.ThumbnailPath #" /></a></li>

#}else if(data._type == window.MenuTypes.REGION)
{#
    <li><a href="\\#" class="icon icon-20b-close mob-icon"></a><a class="nav-item" href="\\#">
        <div class="nexu-mob-colortile" style="background-color:\##= data.data.Material.Hex #;">&nbsp;</div> #: data.DisplayName #</a></li>

#}else{#

    <li><a href="\\#" class="icon icon-20b-close mob-icon"></a><a class="nav-item" href="\\#">#: data.DisplayName #</a></li>

#}#
</script>


<script type="text/javascript">
    window.SVCURL = "http://localhost:50595/api/";
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
        while (match = search.exec(query))
            window.urlParams[decode(match[1])] = decode(match[2]);
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
    window.ImageResoultion = 96;
</script>

<!-- Required Scripts -->
<script src="/assets/js/jquery-2.1.0.min.js" type="text/javascript"></script>
<script src="/assets/js/underscore-min.js" type="text/javascript"></script>
<%--<script src="/assets/kendo/js/kendo.web.min.js" type="text/javascript"></script>--%>
<script src="/assets/kendo/js/kendo.all.min.js"></script>
<script src="/assets/js/snap.svg-min.0.3.0.js" type="text/javascript"></script>
<script src="/assets/js/jquery-ui-1.10.4.effects.min.js" type="text/javascript"></script>
<script src="/assets/js/jquery.swipe.min.js" type="text/javascript"></script>
<script src="/assets/js/spritespin.min.js"></script>
<script src="/assets/js/bootstrap.min.js" type="text/javascript"></script>
<script src="/assets/js/jquery.mCustomScrollbar.concat.min.js" type="text/javascript"></script>
<script src="/assets/js/imagesloaded.pkgd.min.js" type="text/javascript"></script>
<script src="/assets/js/jquery.drawDoughnutChart_1.js" type="text/javascript"></script>
<script src="/assets/js/dropzone.js"></script>
<script src="/assets/js/temp.colors.js" type="text/javascript"></script>
<script src="/assets/js/temp.masterdata.js" type="text/javascript"></script>
<script src="/assets/js/jquery.printelement.min.js"></script>
<script src="/assets/nexu/nexu.translator.js"></script>
<script type="text/javascript">
    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length == 2) return parts.pop().split(";").shift();
    }
    $(document).ajaxSend(function (event, xhr, settings) {
        settings.xhrFields = {
            withCredentials: true
        };
    });
    var l = getCookie('prefLanguage');
    if (l && l != 'en') {
        nexu_translator.Translate(l);
        window.userLang = l;
    }
</script>
<!-- Widgets -->
<script src="/assets/nexu/nexu.loader.js"></script>
<script src="/assets/nexu/nexu.anchoredelement.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.colorwheel.js" type="text/javascript"></script>
<script src="/assets/nexu/jquery.kendo.nexuView.0.7.1.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.customPager.js" type="text/javascript"></script>

<!-- Controllers -->
<script src="/assets/nexu/nexu.background.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.viewcarousel.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.configuratormenu.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.viewer.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.regioncolorpicker.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.logocarousel.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.logorecolor.v2.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.logowindow.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.hierarchy.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.prompt.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.colorgrabber.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.module.360.js"></script>
<script src="/assets/nexu/nexu.summary.js"></script>
<script src="/assets/nexu/nexu.publish.window.js"></script>
<script src="/assets/nexu/nexu.tutorial.js"></script>

<!-- Handlers -->
<script src="/assets/nexu/nexu.templatechange.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.selectchange.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.deeplinker.js" type="text/javascript"></script>
<script src="/assets/nexu/nexu.handler.save.js"></script>
<script src="/assets/nexu/nexu.handler.rendering.js"></script>
<script src="/assets/nexu/nexu.mobile.js"></script>
<script src="/assets/nexu/nexu.main.js"></script>
<script src="/assets/nexu/devUtilities.js"></script>

</body>
</html>
