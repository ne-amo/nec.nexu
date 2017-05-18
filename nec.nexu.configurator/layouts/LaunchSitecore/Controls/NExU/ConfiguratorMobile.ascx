<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfiguratorMobile.ascx.cs" Inherits="nec.nexu.configurator.NExU.ConfiguratorMobile, nec.nexu.configurator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" %> 



<!--<link href="/assets/css/reset.css" rel="stylesheet" />-->
<link href="/assets/css/bootstrap.min.css" rel="stylesheet" />
<link href="/assets/kendo/styles/kendo.common.min.css" rel="stylesheet" />

<link href="/assets/kendo/styles/kendo.common.min.css" rel="stylesheet" />
<link href="/assets/font/Font-Awesome/font-awesome.css" rel="stylesheet" />
<link href="/assets/css/nexu.kendo.css" rel="stylesheet" />
<link href="/assets/css/nexu.css" rel="stylesheet" />



<style type="text/css">
	@media(max-width:480){
	#mobile-menu{
		font-size:10px;
		}
		
	}
	.fa:before{
		font-family:FontAwesome;
	}
	.glyphicon:before{
		font-family:'Glyphicons Halflings';
	}
	.container{
			padding-right:0;
		padding-left:0;
	}
	.nx-loader-container{


	}
	.row{

	}
    .mobile-menu-item {
        margin-bottom: 2px;
        padding: 10px 4px;
        cursor: pointer;
    }

    .header .row {
        color: rgb(0,0,0);
        background-color: rgb(255,242,0);
        font-size: 1em;
        font-weight: bold;
    }

    .sub-header .row {
        background-color: rgb(125,125,125);
        color: rgb(255,255,255);
        font-weight: bold;
    }

    .menu .row {
        background: rgb(50,50,50);
        color: rgb(255,255,255);
    }

    .align-right {
        text-align: right;
    }

    .menu .row div.block-item {
        display: none;
    }

    .menu.tiled .row {
        display: inline-block;
        margin-left: 0;
        margin-right: 0;
        width: 23%;
        height: 50px;
        /*border:1px solid black;*/
        background-color: transparent;
        padding: 0px;
    }

    .menu.tiled {
        padding: 4px 0 5px 5px;
        margin-left: -15px;
        margin-right: -15px;
    }

    .menu.tiled .row div[class*="col-xs-10"] {
        height: 100%;
        margin: 0 2px;
    }

    .menu.tiled .row div[class*="col-xs-2"] {
        display: none;
    }

    .k-widget, .k-widget .row * {
        box-sizing: border-box;
    }

    .k-widget, .k-block, .k-inline-block, .k-draghandle {
        border-style: none;
    }

    .nexu-mobile-window {
        background-color: rgba(0,0,0,.90);
        color: #FFF;
    }

    .mobile-configurator {
        color: #000;
        font-weight: bold;
    }

    .mobile-btn {
        position: absolute;
        padding: 8px 16px 16px;
        font-size: 2em;
    }

    .nx-top {
        top: 0;
    }

    .nx-right {
        right: 0;
    }

    .nx-bottom {
        bottom: 0;
    }

    .mobile-drawer {
        background-color: rgb(0,0,0);
        overflow-y: scroll;
    }

    .v-scroll {
        overflow-y: scroll;
    }

    .mobile-container {
        position: relative;
    }

    .mobile-container .menu, .mobile-container .header, .mobile-container .sub-header, .mobile-container .mobile-menu-helper {
        margin-right: -15px;
        margin-left: -15px;
        background-color: #000;
    }

    .mobile-container .mobile-menu-helper {
        background-color: rgb(225,225,225);
    }

    .nx-menu-wrapper {
        padding: 0 15px;
        margin-right: -15px;
        margin-left: -15px;
    }

    .nexu-viewer-logo-btn {
        visibility: hidden;
    }

    .mobile-notification {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
		z-index:1999;
    }

    .mobile-notification-bar {
        background-color: rgb(80,80,80);
        padding: 8px;
        color: rgb(255,255,255);
    }

    .mobile-notification-bar .k-button {
        border: 1px solid rgb(255,242,0);
        margin: 0 2px;
    }

    .mobile-notification .notify-modal {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        background-color: rgba(0,0,0,.6);
    }

    .mobile-view-selector {
        position: absolute;
        bottom: 4px;
        left: 0;
        width: 100%;
    }

    .mobile-view-selector .view-selector {
        text-align: center;
        background-color: transparent;
    }

    .mobile-view-selector .view-selector div {
        background: rgba(50,50,50,1);
        height: 18px;
        width: 18px;
        -webkit-border-radius: 18px;
        -moz-border-radius: 18px;
        border-radius: 18px;
        margin: 2px 12px;
        opacity: 1;
        border: 1px solid rgb(0,0,0);
        display: inline-block;
    }

    .mobile-view-selector .view-selector div.k-state-selected {
        border: 1px solid rgb(255,242,0);
    }

    .nexu-view {
        margin-left: -15px;
    }

    .dz-image {
        display: none;
    }

    .image-preview {
        text-align: center;
    }

    .image-preview .dz-image {
        display: initial;
        /*position:absolute;*/
    }

    .mobile-background {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
    }

    .mobile-background .bg-top {
        width: 100%;
        background-color: #000;
        background-image: url(/assets/img/wall-default.jpg);
        background-position: center;
    }

    .xlg-text {
        font-size: 200%;
    }

    .menu.purchase {
        background-color: #000;
    }

    .align-center {
        text-align: center;
    }

    .menu.purchase .mobile-bound-item {
        padding: 15px;
    }

    .mobile-colorgrabwindow.k-window-content {
        color: rgb(255,255,255);
        background: rgb(0,0,0);
    }

    .nexu-colorgrab-results ul {
        text-align: center;
        margin-bottom: 14px;
    }

    #nexu-logo-window {
        background-color: rgba(0,0,0,1);
    }

    .not-visible {
    }

    .portrait-message {
        padding: 72px 24px;
        font-size: 1.125em;
    }

    .mobile-summary-window {
        padding: 6px;
    }

    .mobile-summary-window li {
        margin-bottom: 4px;
    }

    .mobile-summary-window .nexu-colorgrabber-swatch {
        margin-right: 8px;
        display: inline-block;
        vertical-align: middle;
    }
	.main-menu{
		margin-bottom:24px;
	}
	.nexu-colorgrab-name input{
		color:#000;
	}
	#mobile-menu{
		margin-top:24px;
    }
	.k-window-titlebar {
		padding-top:12px;
	}
	.mobile-colorgrabwindow,
	.mobile-logo-window,
	.mobile-background-window{
		background-color: #000;
		margin-top: -2px;
	}
	.mobile-logo-list,.mobile-background-list{
		margin-bottom:48px;
	}
	.k-window-titlebar .k-window-actions{
	/*.k-i-close{*/
		/*transform: 			scale(2);
		-webkit-transform: 	scale(2);
		-moz-transform: 	scale(2);
		-o-transform: 		scale(2);
		-ms-transform: 		scale(2);*/
		padding-top:5px;
		font-size:120%;
		padding-right: 12px;
		color: #FFF;
    }

</style>




    <div id="mobile-menu">
        <div class="container nx-loader-container">

            <div class="nx-mobile-portrait portrait-message hidden align-center">

                New Era x You works best in landscape mode.<br />Please rotate your device.

            </div>

            <div class="row nx-mobile-landscape mobile-container hidden ">
			
                <!-- Main Row -->

                <div class="mobile-background bound-height" style="height: 375px;">
                    <div class="bg-top bound-height" style="height: 375px;"></div>
                </div>

                <div class="col-xs-8 mobile-configurator bound-height" style="height: 375px;">

                    <div id="configurator-container" style="margin-top:24px;">
                        <div id="nexu-configurator-viewer"></div>
                    </div>

                    <div class="mobile-view-selector">
                        <div class="view-selector"></div>
                    </div>

                    <div class="mobile-btn nx-top nx-right"><span class="btn-drawer fa fa-paint-brush" data-targetmenu="main"></span></div>

                    <div class="mobile-btn nx-bottom nx-right">
                        <span class="btn-drawer fa fa-shopping-cart" data-targetmenu="purchase"></span>
                        <br />
                        <span class="btn-drawer glyphicon glyphicon-option-horizontal" data-targetmenu="options"></span>
						<br />
						<span class="btn-fullscreen glyphicon glyphicon-fullscreen hidden"></span>
                    </div>

                </div>

                <div id="nxLoader" style="position:absolute; top: 4px; left:4px;"><img src="/assets/img/loading.gif" /></div>
                <div id="nxLock" style="position:absolute; bottom: 4px; left:4px;"><i class="fa fa-lock"></i> Scroll Locked</div>

                <div class="col-xs-4 mobile-drawer nx-scrollable  bound-height-max " data-currenttarget="main"  style="max-height: 375px;">
                    <!-- Menu Column -->

                    <div class="header">
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12">#: DisplayName #</div>
                            <div class="col-xs-2 align-right #: SortOrder #"><span class="glyphicon glyphicon-chevron-left"></span></div>
                        </div>
                    </div>
                    <div class="sub-header">
                        <div class="row mobile-menu-item">
                            <div class="col-xs-9">#: DisplayName #</div>
                            <div class="col-xs-2 align-right"><span class="glyphicon glyphicon-minus"></span></div>
                        </div>
                    </div>
                    <div class="row mobile-menu-item mobile-menu-helper mobile-bound-item" data-bind="visible: ShowHelper" data-model="_nec_.nexu.mobile.model">
                        <div class="col-xs-12" data-bind="text: MobileHelperName"></div>
                    </div>
                    <div class="menu main-menu" data-menu="main">
                        <div class="row mobile-menu-item">
                            <div class="col-xs-9">#: DisplayName #</div>
                            <div class="col-xs-2 align-right"><span class="glyphicon glyphicon-chevron-right"></span></div>
                        </div>
                    </div>
                    <div class="menu options" data-menu="options">
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12 mobile-bound-item" data-bind="events: { click: Login }" data-model="_nec_.nexu.events"">
                                Log-in
                            </div>
                        </div>
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12 mobile-bound-item" data-bind="events: { click: Save }" data-model="_nec_.nexu.events"">
	                            Save
                            </div>
                        </div>
                        <div class="row mobile-menu-item mobile-save-name" style="display:none;">
                            <div class="col-xs-12  align-center">
                                Please name your cap<br />
                                <input id="input-product-name" type="text" class="k-input mobile-bound-item"
                                    data-model="_nec_.nexu.models.product"
                                    data-bind="value: DisplayName" />
                                <br />
                                <div>
                                <a class="k-button xlg-text mobile-bound-item" data-bind="events: { click: ConfirmSave }" data-model="_nec_.nexu.events">
	                                OK
                                </a>
                                </div>
                            </div>
                        </div>
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12 mobile-bound-item" data-bind="events: { click: Summary }" data-model="_nec_.nexu.events"">
	                            Product Summary
                            </div>
                        </div>
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12 mobile-bound-item" data-bind="events: { click: Share }" data-model="_nec_.nexu.events"">
	                            Share
                            </div>
                        </div>
                        <div class="row mobile-menu-item">
                            <div class="col-xs-12 mobile-bound-item" data-bind="events: { click: Help }" data-model="_nec_.nexu.events"">
	                            Help
                            </div>
                        </div>

                    </div>

                <div class="menu purchase" data-menu="purchase">
                    <div class="mobile-bound-item" data-model="_nec_.nexu.mobile.model">
                        <div id="nexu-product-description">
                            <div>
                                <p class="main" data-bind="text: StyleDescription"></p>
                                <p class="sub" data-bind="text: PriceDescription">$79.99</p>
                            </div>
                        </div>
                        <p class="align-center">
							<span style="color:#FFF; margin-bottom:2px;">Name your cap</span><br />
							<input id="input-product-name" type="text" style="max-width:100%;" class="k-input mobile-bound-item"
								data-model="_nec_.nexu.models.product"
								data-bind="value: DisplayName" />
							<br />
							<a style="display:block;" class="k-button xlg-text mobile-bound-item" data-bind="events: { click: ConfirmSave }" data-model="_nec_.nexu.events">
	                                BUY CAP
							</a>
                        </p>
                    </div>
                </div>

            </div>
            <!-- Close Menu Column -->


            <div class="mobile-notification">
                <div class="notify-modal"></div>
                <div class="mobile-notification-bar row">
                    <div class="col-xs-7 notify-message">Welcome</div>
                    <div class="col-xs-4 align-right">
                        <a class="k-button notify-confirm">OK</a>
                        <a class="k-button notify-decline">Cancel</a>
                        <a class="k-button notify-close"><span class="glyphicon glyphicon-remove"></span></a>
                    </div>
                </div>

            </div>

        </div>
        <!-- Close Main Row -->
        <div style="display:none;">
            <div class="mobile-colorgrabwindow">
                <div class="cg-section" data-section="1">
                    <div class="align-center">
                        <p class="xlg-text"><span class="xlg-text glyphicon glyphicon-camera"></span></p>
                        <div class="cb-upload-1" data-target="2">
                            <span class="k-button dz-clickable  xlg-text">Upload Photo</span>
                        </div>
                        <p>*Message about any devices that do not support uploading a photo into this process.</p>
                    </div>
                </div>
                <div class="cg-section" data-section="2" style="display:none;">
                    <div class="image-preview"></div>
                    <div class="align-center">
                        <a href="#" class="k-button xlg-text cb-upload-2"><span class="dz-clickable">Upload New Photo</span></a>
                        <a href="#" class="k-button xlg-text"  data-target="3">Grab Colors</a>
                    </div>
                </div>
                <div class="cg-section" data-section="3" style="display:none;">
                    <div class="nexu-colorgrab-label align-center">&nbsp;</div>
                    <div class="nexu-colorgrab-results"><ul></ul></div>
                    <div class="nexu-colorgrab-name align-center" style="margin-bottom:8px;">Name your set: <input type="text" /></div>
                    <div class="align-center">
                        <a href="#" class="k-button xlg-text cb-upload-3" data-target="2">Upload New Photo</a>
                        <a href="#" class="k-button xlg-text cb-load">Load Colors</a>
                    </div>
                </div>
            </div>
            <div class="mobile-summary-window">
                <ul class="mobile-summary-regions"></ul>
                <ul class="mobile-summary-logos"></ul>
            </div>
            <div class="mobile-background-window">
                <ul class="mobile-background-list"></ul>
            </div>
            <div class="mobile-logo-window">
                <ul class="mobile-logo-list"></ul>
            </div>
        </div>
    </div>
    <!-- Close COntainer -->
    </div><!-- close #mobile-menu -->

    <script src="/assets/js/jquery-2.1.0.min.js" type="text/javascript"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/underscore.js/1.8.3/underscore-min.js" type="text/javascript"></script>
    <script src="/assets/kendo/js/kendo.all.min.js"></script>
    <script src="/assets/js/snap.svg-min.0.3.0.js" type="text/javascript"></script>
    <script src="/assets/js/jquery.swipe.min.js" type="text/javascript"></script>
    <script src="/assets/js/spritespin.min.js"></script>
    <script src="/assets/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="/assets/js/imagesloaded.pkgd.min.js" type="text/javascript"></script>
    <script src="/assets/js/dropzone.js"></script>
    <script src="/assets/js/jquery.mCustomScrollbar.concat.min.js" type="text/javascript"></script>
    <script src="/assets/js/jquery.scrollLock.js"></script>

    <script src="/assets/js/customBreakpoints.js"></script>

    <script src="/assets/nexu/mobile/nexu.config.js"></script>
    <script src="/assets/nexu/nexu.common.js"></script>
    <script src="/assets/nexu/jquery.kendo.nexuView.0.7.1.js"></script>
    <script src="/assets/nexu/nexu.loader.js"></script>
    <script src="/assets/nexu/nexu.handler.rendering.js"></script>

    <script src="/assets/nexu/mobile/nexu.data.js"></script>
    <script src="/assets/nexu/nexu.logowindow.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.handlers.js"></script>
    <script src="/assets/nexu/mobile/nexu.ui.js"></script>
    <script src="/assets/nexu/mobile/nexu.event.handlers.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.viewer.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.controls.notification.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.controls.colorgrab.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.controls.summary.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.backgroundwindow.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.logowindow.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.handlers.save.js"></script>
    <script src="/assets/nexu/mobile/nexu.mobile.main.js"></script>