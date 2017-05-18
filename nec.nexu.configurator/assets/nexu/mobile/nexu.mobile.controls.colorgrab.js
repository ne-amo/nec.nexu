
; (function ($, kendo, _, window, undefined) {

    /* global namespacing */
    var nec = window._nec_ || {};
    nec.nexu = nec.nexu || {};
    nec.nexu.mobile = nec.nexu.mobile || {};
    nec.nexu.models = nec.nexu.models || {};
    nec.nexu.ui = nec.nexu.ui || {};
    nec.nexu.data = nec.nexu.data || {};
    nec.nexu.config = nec.nexu.config || {};
    nec.nexu.events = nec.nexu.events || {};
    nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
    nec.nexu.mobile.controls.colorgrabber = nec.nexu.mobile.controls.colorgrabber || {};

    /* local shorthand variables */
    var data = nec.nexu.data,
		config = nec.nexu.config,
		mobile = nec.nexu.mobile,
		models = nec.nexu.models,
		ui = nec.nexu.ui,
		events = nec.nexu.events,
		k_Ds = kendo.data.DataSource,
        controls = nec.nexu.mobile.controls,
        grabber = nec.nexu.mobile.controls.colorgrabber,
        listTemplate = kendo.template('<li><div class="nexu-colorgrabber-swatch" style="background-color:\\##: Hex #;" title="#: DisplayName #"></div></li>'),
        CLICK = 'click';

    var maxSize = 2;


    function changeSection(num, container) {
        $('.cg-section', container).hide()
            .filter('[data-section="' + num + '"]')
            .show();
        nec.nexu.mobile.controls.colorgrabber.window.verticalCenter('.cg-section:visible');
    }


    function uploadSuccess(file, response) {
        /* Process the upload response */
        var list = $.parseJSON(response);
        if (list.length == 0) {
            /* no colors found */
            alert('no colors found');
        }
        nec.nexu.mobile.controls.colorgrabber.list = list;
        nec.nexu.mobile.controls.colorgrabber.listContainer.empty().html(kendo.render(listTemplate, list));
    }
    function uploadComplete(file) {
        changeSection($(this.element).data('target'));
    };

    function canUserSave() {
        return models.Product.Creator.Id != 1;
    }

    function loadColors() {
        var name = $('.nexu-colorgrab-name input', nec.nexu.mobile.controls.colorgrabber.element).val();
        if (name.trim() == '')
            name = 'Grabbed Colors';
        var list = nec.nexu.mobile.controls.colorgrabber.list;
        var colorlist = {
            Id: 0,
            DisplayName: name,
            MaterialCount: list.length,
            AllowRemove: true,
            AllowSave: canUserSave(),
            IsUserCreated: true,
            Colors: list
        };
        data.ColorLists.add(colorlist);

        nec.nexu.mobile.controls.colorgrabber.window.close();
    }


    function ScaleImage(srcwidth, srcheight, targetwidth, targetheight, fLetterBox) {

        var result = { width: 0, height: 0, fScaleToTargetWidth: true };

        if ((srcwidth <= 0) || (srcheight <= 0) || (targetwidth <= 0) || (targetheight <= 0)) {
            return result;
        }

        // scale to the target width
        var scaleX1 = targetwidth;
        var scaleY1 = (srcheight * targetwidth) / srcwidth;

        // scale to the target height
        var scaleX2 = (srcwidth * targetheight) / srcheight;
        var scaleY2 = targetheight;

        // now figure out which one we should use
        var fScaleOnWidth = (scaleX2 > targetwidth);
        if (fScaleOnWidth) {
            fScaleOnWidth = fLetterBox;
        }
        else {
            fScaleOnWidth = !fLetterBox;
        }

        if (fScaleOnWidth) {
            result.width = Math.floor(scaleX1);
            result.height = Math.floor(scaleY1);
            result.fScaleToTargetWidth = true;
        }
        else {
            result.width = Math.floor(scaleX2);
            result.height = Math.floor(scaleY2);
            result.fScaleToTargetWidth = false;
        }
        result.targetleft = Math.floor((targetwidth - result.width) / 2);
        result.targettop = Math.floor((targetheight - result.height) / 2);

        return result;
    }
    function resizeImage(file) {
        var sW = file.width,
            sH = file.height,
            cW = document.documentElement.clientWidth * 0.8,
            cH = document.documentElement.clientHeight * 0.8;
        var size = ScaleImage(sW, sH, cW, cH - 30, true);
        return {
            srcWidth: sW,
            srcHeight: sH,
            srcX: 0,
            srcY: 0,
            trgWidth: size.width,
            trgHeight: size.height,
            trgX: 0,
            trgY: 0
        }
    }
    function thumbnailCreated(e, dataUrl) {
        var c = $('.image-preview', nec.nexu.mobile.controls.colorgrabber.window.element).empty();
        var img = $('.dz-image', nec.nexu.mobile.controls.colorgrabber.window.element)
            .appendTo(c).find('img')[0];
        img.onload = function (e) {
            nec.nexu.mobile.controls.colorgrabber.window.verticalCenter('.cg-section:visible');
        }
    }

    function initDropZone(container) {
        var dropOptions = {
            url: "/match.colorgrab?upload=true",
            autoProcessQueue: false,
            paramName: "files",
            createImageThumbnails: true,
            clickable: '.dz-clickable',
            resize: resizeImage,
            maxFiles: 1,
            uploadMultiple: false,
            maxFilesize: maxSize,
            acceptedFiles: '.jpeg,.png,.bmp,.jpg',
            init: function () {
                //this.on("drop", uploadDrop);
                this.on("success", uploadSuccess);
                this.on("complete", uploadComplete);
                this.on('thumbnail', thumbnailCreated);
                this.on('error', function (e, errorMessage, xhr) {
                    alert(errorMessage);
                });





                /* taken from:
    https://github.com/sonpython/dropzonejs_add_client_side_resize_compress/blob/master/dropzone_resize_and_compress_client_side.js
    allows resizing of the image before uploading, this should prevent most of the "file too large" errors when uploading from mobile. */
                var myDropzone = this;
                var canvas = document.createElement('canvas');

                this.on('addedfile', function (file) {
                    var max_w = 800;
                    var max_h = 600;

                    var reader = new FileReader();
                    reader.onload = function (e) {
                        var img = new Image();
                        img.onload = function () {
                            var w = img.width;
                            var h = img.height;
                            var ratio_w = 1;
                            var ratio_h = 1;
                            if (w > max_w) {
                                ratio_w = max_w / w;
                            }
                            if (h > max_h) {
                                ratio_h = max_h / h;
                            }

                            var ratio = Math.min(ratio_w, ratio_h);
                            w = Math.floor(w * ratio);
                            h = Math.floor(h * ratio);
                            canvas.width = w;
                            canvas.height = h;
                            var ctx = canvas.getContext('2d', { preserveDrawingBuffer: true });
                            ctx.drawImage(img, 0, 0, w, h);

                            var dataURL = canvas.toDataURL('image/jpeg', 0.5);
                            var a = dataURL.split(',')[1];
                            var blob = atob(a);
                            var array = [];
                            for (var k = 0; k < blob.length; k++) {
                                array.push(blob.charCodeAt(k));
                            }
                            var data = new Blob([new Uint8Array(array)], { type: 'image/jpeg' });
                            //resizeImgList.push(data);
                            myDropzone.removeFile(file);
                            //myDropzone.addFile(data);
                            myDropzone.processFile(data);
                        };
                        img.src = e.target.result;
                    };
                    reader.readAsDataURL(file);
                });

                this.on('removedfile', function (file) {
                    ////var k = filelist.indexOf(file);
                    //if (k > -1) {
                    //	//filelist.splice(k, 1);
                    //	//resizeImgList.splice(k, 1);
                    //}
                });


            }
        };
        new Dropzone('.cb-upload-1, .cb-upload-2, .cb-upload-3', dropOptions);
    }


    function _init(element) {

        var el = $(element);

        var win = el.kendoWindow({
            title: '',
            resizable: false,
            visible: false,
            open: function (e) {

                //.titleElement().css('border-color', '#000000');
            },
            activate: function (e) {
                e.sender
                e.sender.fullScreen()
                .verticalCenter('.cg-section:visible')

            },
            close: function (e) {
                mobile.resetMenu();
            }
        }).data('kendoWindow').changeButton('.k-window-action',
			'<span role="presentation" class="glyphicon glyphicon-remove k-i-close"></span>');

        el.on(CLICK, 'a[data-target]', function (e) {
            if ($(e.target).is('a')) {
                changeSection($(e.target).data('target'), el);
            }
            return false;
        });

        initDropZone(el);

        nec.nexu.mobile.controls.colorgrabber.window = win;
        nec.nexu.mobile.controls.colorgrabber.element = el
            .on(CLICK, 'a.cb-load', function (e) {
                var x = $(e.target);
                if (x.is('a')) {
                    loadColors();
                    return false;
                }
            });
        nec.nexu.mobile.controls.colorgrabber.listContainer = $('.nexu-colorgrab-results ul', nec.nexu.mobile.controls.colorgrabber.element)
            .on(CLICK, 'div.nexu-colorgrabber-swatch', function (e) {
                var x = $(e.target)
                if (x.is('div')) {
                    $('.nexu-colorgrab-label', nec.nexu.mobile.controls.colorgrabber.element).text(x.attr('title'));
                }
            });
    }

    grabber.init = _init;

    function _open(options) {
        changeSection(1);
        nec.nexu.mobile.controls.colorgrabber.window.open();
    }
    grabber.open = _open;

    window._nec_ = nec;

})(jQuery, kendo, _, window);





