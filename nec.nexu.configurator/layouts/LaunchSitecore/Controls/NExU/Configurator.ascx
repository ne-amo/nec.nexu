<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Configurator.ascx.cs" Inherits="nec.nexu.configurator.NExU.Configurator, nec.nexu.configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" %>

<link href="/assets/css/reset.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/kendo/styles/kendo.common-bootstrap.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/kendo/styles/kendo.flat.min.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
<link href="/assets/css/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.css" rel="stylesheet" type="text/css" />
<link href="/assets/css/nexu.mobile.css" rel="stylesheet" />
<script type="text/x-kendo/template" id="mobile_template_bg">
 <li class="nexu-mob-background-menu"><a href="\\#" class="nx-item nx-bg" data-item-id="#=data.Id#"><img src="http://neccmsdev.neweracap.com/assets#= data.ThumbnailPath #" /></a></li>
</script>
    <script type="text/x-kendo/template" id="mobile_template_mob_thumbnail">
        <li class="nx-logo-tn">
            <div>
            <a class="nx-prev icon icon-20w-close mob-icon" href="\\#"></a> 
            <a href="\\#" data-item-id="#= data.Id #" class="nx-item #: data.ClassId #"><img src="#: OfficialImage #" title="#: DisplayName #" /></a>
            </div>
            <ul class="nx-sub"></ul>
        </li>
    </script>
    <script type="text/x-kendo/template" id="mobile_template_group">
    <li>
        <div>
        <a class="nx-prev icon icon-20w-close mob-icon" href="\\#"></a> 
        <a href="\\#" data-item-id="#= data.Id #" class="nx-item #: data.ClassId #">#: data.DisplayName #</a>
        </div>
        <ul class="nx-sub"></ul>
    </li>
    </script>
    <script type="text/x-kendo/template" id="mobile_template_layer">
    <li>
        <div>
        <a class="nx-prev icon icon-20w-close mob-icon" href="\\#"></a> 
        <a href="\\#" data-item-id="#: data.ImageDataId #" class="nx-item #: data.ClassId #">#: data.DisplayName #</a>
        </div>
        <ul class="nx-sub"></ul>
    </li>
    </script>
<%--<a class="nx-prev icon icon-20w-close mob-icon" href="\\#"></a>--%>
    <script type="text/x-kendo/template" id="mobile_template_colorlist">
    <li>
	    <div>
		    <a class="nx-prev glyphicon glyphicon-minus icon mob-icon" href="\\#"></a>
		    <a href="\\#" data-item-id="#= data.Id #" data-nav-type="colorlist" class="nx-item nx-colorlist">#: data.DisplayName #</a>
	    </div>
	    <div class="nx-label"></div>
	    <ul class="nx-sub nav-menu-colorlist"></ul>
    </li>
    </script>
<noscript>
	<div style="font-family:'proxima nova'; text-align:center; font-size:1.5em; color:red; margin-top:10px;">Yeah..... we're going to need you to go ahead and enable Javascript.<br />This site kind of needs it. Thanks!</div>
</noscript>
<div id="nexu-intitialize" class="row" style="display:none;">
    <span class="nx-init-msg"><%= message %></span>
</div>
<div id="nexu-configurator-module" class="nexu-ui initialHide mobileWidth mobileHeight nx-loader-container">


   <div id="nexu-right-drawer" style="width: 270px; background-color:#000; right:0; left:auto;" class="nexu-mobile-menu-config mobile-only">
        <h3 class="nav-current"><a href="#">New Era By You</a></h3>
        <ul id="nx_mobileList" class="nav-menu"></ul>
        <ul id="nx_optionsList" class="nav-options">
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-save">Save</a></div>
            </li>
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-order">Place Order</a></div>
            </li>
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-publish">Publish</a></div>
            </li>
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-summary">Summary</a></div>
            </li>
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-share">Share</a></div>
            </li>
            <li style="display: list-item;">
                <div><a href="#" data-item-id="0" class="nx-item nx-help">Help</a></div>
            </li>

        </ul>
    </div>



    <!-- container -->

    <div id="nexu-background" class="mobileHeight mobileWidth">
        <div id="nexu-background-top" class="row mobileHeight"></div>
        <div id="nexu-background-bottom" class="row"></div>
    </div>
    <div id="nexu-configurator" class="row  mobileWidth mobileHeight">
        <div id="nexu-ui-top" class="row desktop-only">
            <div id="nexu-live-description" class="col-sm-9 nexu-bindable" data-obs="Selected" data-bind="text: Menu.Label"></div>
            <div class="col-sm-3 nx-user-desc" style="text-align:right;">
                <a href="#" id="function_login">
                <span class="nexu-bindable" data-obs="Display" data-bind="text: CreatorNickname"></span>
                <img src="/assets/img/usericon.png" /></a>
            </div>
        </div>

        <div id="nexu-ui-left" class="col-sm-2">
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
                        <span class="fa fa-download" style="font-size:22px; color:#000;"> <!--class="icon icon-20b-dropper"--> </span><span class="nexu-label local_text">Download</span></div></li>
                    <li id="function_publish"><div>
                        <span class="fa fa-th"  style="font-size:22px; color:#000;"> <!--class="icon icon-20b-list1"--></span><span class="nexu-label local_text">Publish</span></div></li>
                    <li id="function_save"><div>
                        <span class="icon icon-20b-save"></span><span class="nexu-label local_text">Save Progress</span></div></li>
                    <li id="function_startover"><div>
                        <span class="icon icon-20b-reset"></span><span class="nexu-label local_text">Start Over</span></div></li>
                </ul>
            </div>
        </div>

        <div class="mobile-only nexu-mob-btn menu-config"><a href="#"><span class="icon icon-40b-menu"></span></a></div>
        <div class="mobile-only nexu-mob-btn menu-options"><a href="#"><span class="icon icon-40b-order"></span></a></div>

    </div>

    <div id="nexu-ui-menu-bottom" class="row">
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


    <div id="nexu-tutorial" class="nx-tutorial initialHide desktop-only">
        
        <div class="nx-tutorial-item" data-tip="1" data-left="12%" data-top="15%">
            <div class="nx-tutorial-text">These are areas of your cap that you can customize. Choose from the list, then click on the area of the cap to modify it.</div>
            <div class="k-callout k-callout-w nx-tutorial-callout"></div>
        </div>
        <div class="nx-tutorial-item" data-tip="2" data-right="12%" data-top="30%">
            <div class="nx-tutorial-text">All done? Publish to the gallery. Not done yet? Save and finish later. New idea? Start over.</div>
            <div class="k-callout k-callout-e nx-tutorial-callout"></div>
            </div>
        <div class="nx-tutorial-item" data-tip="3" data-left="44%" data-bottom="10%">
            <div class="nx-tutorial-text">Check out your cap design from different angles. You can even generate a 3D view.</div>
            <div class="k-callout k-callout-s nx-tutorial-callout"></div>
            </div>
        <div class="nx-tutorial-item" data-tip="4" data-left="100px" data-bottom="30px">
            <div class="nx-tutorial-text">You can customize the background. There are even some secret backgrounds...</div>
            <div class="k-callout k-callout-w nx-tutorial-callout"></div>
            </div>
        <div class="nx-tutorial-item" data-tip="5" data-right="80px" data-top="5px">
            <div class="nx-tutorial-text">With an account, you can create your own custom cap wall for the world to see.</div>
            <div class="k-callout k-callout-e nx-tutorial-callout"></div>
            </div>
        <div class="nx-tutorial-controls">
            <a class="nx-tutorial-close"><span class="icon icon-20b-close">&nbsp;</span></a>
            <h2 class="nx-tutorial-welcome">Welcome to New Era by You!</h2>
            <h3>HERE ARE A FEW TIPS<br />TO GET YOU STARTED</h3>
            <p>
                <a href="#" class="nx-tutorial-prev"><span class="icon icon-40b-left"></span></a> 
                <span class="nx-tutorial-current"></span> of <span class="nx-tutorial-total"></span> 
                <a href="#" class="nx-tutorial-next"><span class="icon icon-40b-right"></span></a> 
            </p>
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
        <div class="nexu-select"><a href="#" title="Select Logo">SELECT</a></div>
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
                <td class="hidden-lg nexu-recolor-button" width="20"><span class="icon icon-20b-dropper"></span></td>
                <td class="hidden-md nexu-recolor-button" width="60">Modify</td>
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
                            <div class="nexu-colorgrab-error nexu-colorgrab-section"></div>
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
                        <div><span class="glyphicon glyphicon-camera"></span></div>
                        <div><a href="#" class="k-button nexu-cb-mob-upload" style="font-size:1.5em;">Upload Photo</a></div>
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
                <li><a href="#" class="k-button" data-format="2">Tablet<br /> 1024 x 768</a></li>
                <li><a href="#" class="k-button" data-format="3">Phone <br /> 640 x 960</a></li>
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
<div id="nexu-summary-temp" style="width:8.5in; height:11in;"></div>

<div id="nexu-save-prompt" style="display:none;">
<div>Please name your cap: <input id="inp_ProductName" type="text" class="k-input nexu-bindable" data-bind="value: DisplayName" data-obs="Product" /></div>
</div>
<script type="text/x-kendo-template" id="mobile_template_basic">
#if(data.Pattern)
 {#
    <li data-item-id="#= data.Id #" class="nexu-mob-colortile #: data.ClassId #"><div style="background-image:url(#= data.Pattern.Thumbnail.FilePath #);"></div></li>
#}else if(data.Hex) {#
    <li data-item-id="#= data.Id #" class="nexu-mob-colortile #: data.ClassId #"><div style="background-color:\##= data.Hex #;"></div></li>
#}else{#
    <li data-item-id="#= data.Id #">#= data.DisplayName # <span class='icon icon-20w-right'></span></li>
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
<script src="/assets/nexu/nexu.common.js"></script>
<script src="/assets/nexu/nexu.translator.js"></script>
<script type="text/javascript">
	(function($){
    $('#nexu-intitialize').show();
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
    function getUser(callback) 
    {
        $.get('/nexu.token').done(function (token) {
            if (token && token != '') {
                $.ajax({
                    url: window.SVCURL + '/account/UserInfo',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader("Authorization", "Bearer " + token);
                    }
                }).done(function (data) {
                    callback.call(this, data);
                });
            }
            else {
                callback.call(this, null);
            }
        });
    };
		window.getUser = getUser;
	})(jQuery);
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






