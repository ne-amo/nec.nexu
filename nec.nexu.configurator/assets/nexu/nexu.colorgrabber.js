; var module_ColorGrabber = (function ($, window, undefined) {
    var imgFormats = ['PNG', 'JPG', 'JPEG', 'BMP', 'GIF'];
    var maxSize = 1;
    var moduleElement = $('#nexu-colorgrab-window');
    var listContainer = $('.nexu-colorgrab-results > ul', moduleElement);
    var listTemplate = kendo.template($('#nexu-tmpl-colorgrabber-result', moduleElement).html());
    var def = {
        resizable: false,
        modal: true,
        draggable: false,
        height: 420,
        visible: false,
        open: function (e) {
            e.sender.element.closest('.k-window').addClass('nexu-colorgrabber-window-container');
            e.sender.element.prev().addClass('nexu-prompt-title');
            _hideAll();
            _showInit();
            e.sender.center();
        }
    };

    var windowWidget = moduleElement.kendoWindow(def).data('kendoWindow');

    function _hideAll() {
        $('.nexu-colorgrab-phase', moduleElement).hide();
        $('.image-container img', moduleElement).hide();
    }
    function _showInit() {
        $('.nexu-colorgrab-init', moduleElement).show();
    }
    function _showProgress() {
        $('.nexu-colorgrab-progress', moduleElement).show();
        $('.image-container img', moduleElement).show();
    }
    function _showResults() {
        $('.nexu-colorgrab-results', moduleElement).show();
        $('.image-container img', moduleElement).show();
    }
    function _showError(message) {
        _showInit();
        $('.nexu-colorgrab-error', moduleElement).show().text(message);
    }
    function thumbnailCreated(e, dataUrl) {
        $('.image-container img', moduleElement).hide().attr('src', dataUrl);
        _showStage(3);
    }
    function _showStage(num) {
        var el = moduleElement;
        $('.stage', el).hide();

        $('[data-stage="'+num+'"]', el).show();
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
            container = $('#nexu-colorgrab', moduleElement),
            cW = container.innerWidth(),
            cH = container.innerHeight();
        var size = ScaleImage(sW, sH, cW, cH-30,true );
        return {
            srcWidth:sW,
            srcHeight:sH,
            srcX:0,
            srcY:0,
            trgWidth:size.width,
            trgHeight:size.height,
            trgX:0,
            trgY:0
        }
    }
    function onSelect(e) {
        var isError = false, errorMsg;
        $.each(e.files, function (index, value) {
            if (imgFormats.indexOf(value.extension.replace(".", "").toUpperCase()) == -1) {
                isError = true;
                errorMsg = "Invalid file format.";
            }
            if (value.size > maxSize) {
                isError = true;
                errorMsg = "File size too large.";
            }
            if (isError) {
                e.preventDefault();
                alert(errorMsg);
            }
        });
    };
    function uploadDrop(event) {
        /* Hide the drop, show the spinner */
        _hideAll();
        _showProgress();
    }
    function canUserSave() {
        return _nexu_.Product.Creator.Id != 1;
    }
    function uploadSuccess(file, response) {
        /* Process the upload response */
        var list = $.parseJSON(response);
        if (list.length == 0) {
            /* no colors found */
        }
        var button = $('<li class="nexu-colorgrabber-button"><a href="#">Load Colors</a></li>').on('click', function (evt) {
            var name = $('.nexu-colorgrab-name', moduleElement).val();
            if (name == '')
                name = 'Grabbed Colors';
            var colorlist = {
                Id: 0,
                DisplayName: name,
                MaterialCount: list.length,
                AllowRemove: true,
                AllowSave: canUserSave(),
                IsUserCreated:true,
                Colors: list
            };
            _nexu_.ColorPicker.AddColorList(colorlist);
            windowWidget.close();
            return false;
        });
        listContainer.html(kendo.render(listTemplate, list)).append(button).data('colorlist',list);
    }
    function uploadComplete(file) {
        _hideAll();
        $('.dz-preview', moduleElement).remove();
        if (file.status == 'error') {
            var message = parseError(file);
            _showError(message);
        }
        else {
            _showResults();
        }
    };
    function parseError(file) {
        if (!file.accepted) {
            /* invalid file type or exceeded size */
            if (file.size > (maxSize * 1000000)) {
                /* file too large */
                return "There was an error. The file size is too large.";
            }
            else {
                /* invalid type */
                return "There was an error. The file type is not supported.";
            }
        }
        else {
            /* generic error */
            return "There was an unknown error has occurred.";
        }
    }

    var dropOptions = {
        url:"/match.colorgrab?upload=true",
        paramName: "files",
        createImageThumbnails: true,
        clickable:'.dz-clickable',
        resize:resizeImage,
        uploadMultiple: false,
        maxFilesize: maxSize,
        acceptedFiles:'.jpeg,.png,.bmp,.jpg',
        init: function () {
            this.on("drop", uploadDrop);
            this.on("success", uploadSuccess);
            this.on("complete", uploadComplete);
            this.on('thumbnail', thumbnailCreated);
        }
    };
    new Dropzone('.nexuUploadDropzone1,.nexuUploadDropzone2, #nexu-colorgrab .nexu-cb-mob-browse', dropOptions);

    _hideAll();
    _showInit();
    windowWidget.close();

    return {
        open: function () {
            windowWidget.open();
            return this;
        },
        close: function () {
            windowWidget.close();
            return this;
        },
        window:function(){
            return windowWidget;
        },
        setSize: function (options) {
            if(!options){
                return this;
            }
            if (options.width) {
                windowWidget.setOptions({ width: options.width });
            }
            if (options.height) { /* -32 for titlebar height */
                windowWidget.setOptions({ height: options.height-32 });
            }
            return this;
        },
        showStage:function(num){
            _showStage(num);
            return this;
        },
        reset: function () {
            windowWidget.setOptions(def);
            _showStage(0);
            return this;
        },
        setMobile: function () {

            $('.nexu-cb-mob-upload', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                module_ColorGrabber.showStage(2);
                return false;
            })
            $('.nexu-cb-mob-take', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                return false;
            });
            $('.nexu-cb-mob-browse', '#nexu-colorgrab')
                .off(CLICK).on(CLICK, function (e) {
                    return false;
                })
            $('.nexu-cb-mob-cancel', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                windowWidget.close();
                return false;
            });
            $('.nexu-cb-mob-back', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                module_ColorGrabber.showStage($(this).data('backTo'));
                return false;
            });
            $('.nexu-cb-mob-grab', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                _showStage(4);
                return false;
            });
            $('.nexu-cb-mob-load', '#nexu-colorgrab').off(CLICK).on(CLICK, function (e) {
                var name = 'Grabbed Colors';
                var list = listContainer.data('colorlist');
                var colorlist = {
                    Id: 0,
                    DisplayName: name,
                    MaterialCount: list.length,
                    Colors: list
                };
                _nexu_.ColorPicker.AddColorList(colorlist);
                windowWidget.close();
                listContainer.data('colorlist',null);
                return false;

            });

        }
    }
})(jQuery, window);



