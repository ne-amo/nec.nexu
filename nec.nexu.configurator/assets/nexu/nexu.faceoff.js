
; (function NexuFaceOff($, kendo, window, undefined) {

    var module = $('.nx-faceoff-container'), imgRoot = window.NEXUIMGROOT, api = window.SVCURL;
    var circles = {
        left: $('.nx-circle-container.nx-left .nx-circle', module),
        right: $('.nx-circle-container.nx-right .nx-circle', module)
    };
    var labels = {
        left: $('.nx-faceoff-percentage.nx-left', module),
        right: $('.nx-faceoff-percentage.nx-right', module)
    };

    var images = [];

    function imageLoad(e) {
        var i = images.indexOf(e.srcElement.currentSrc);
        if (i > -1) {
            images.splice(i, 1);
        }
        if (images.length)
            return;
        setHeight();
    }
    function setHeight() {
        var row = $('.nx-faceoff-caps', module);
        $('.nx-mid:not(.nx-mobile)', row).css('line-height', row.outerHeight() + 'px');
        formatCircleContainer();
        drawCircles();
    }
    function drawCircles() {
        $('.nx-circle-container', module).each(function (index, element) {
            var el = $(element);
            var w = el.outerWidth() * 2, h = el.outerHeight();
            var circle = $('.nx-circle', el);
            var percentage = parseInt(circle.attr('data-percentage'));
            var val = Math.floor(w * (percentage / 100));
            var top = (h / 2) - (val / 2);
            var color = circle.data('bg');
            var css = {
                width: val,
                height: val,
                top: top,
                'border-radius': val,
                'background-color': color
            }
            if (el.hasClass('nx-right')) {
                css.right = '-' + ((val / 2) - 100) + 'px';
            }
            else {
                css.left = '-' + ((val / 2) - 100) + 'px';
            }
            circle.css(css);
        });
        $('.nx-faceoff-fill').each(function (index, element) {
            var color = $(element).data('bg');
            $(element).css('background-color', color);
        });
    }
    function formatCircleContainer() {
        $('.nx-circle-container', module).height(module.outerHeight());
    }
    function formatImages() {
        $('.nx-hidden-image', module).each(function (index, element) {
            var el = $(element), imgUrl = el.val();
            var panelwidth = el.closest('.nx-faceoff-panel').width();
            var imgWidth = Math.min(380, Math.floor(panelwidth * .9));
            var image = new Image();
            image.src = imgUrl + '?w=' + imgWidth;
            image.classList.add('nx-faceoff-img');
            image.alt = "Face-Off Cap";
            image.onload = imageLoad;
            images.push(imgUrl + '?w=' + imgWidth);
            el.replaceWith(image);
        });
    }

    function getRemainder(value) {
        return parseInt(value.toFixed(1).toString().split('.')[1])
    }
    function getPercentages(model) {
        var total = model.TotalVotes;
        if (model.TotalVotes == 0) {
            return {
                left: 0,
                right: 0
            };
        }
        var leftTotal = model.FinishedProductAVoteCount,
            rightTotal = model.FinishedProductBVoteCount
            , leftPer = (leftTotal / total) * 100
            , rightPer = (rightTotal / total) * 100
            , remLeft = getRemainder(leftPer)
            , remRight = getRemainder(rightPer);
        if (remLeft < remRight) {
            leftPer = Math.floor(leftPer);
            rightPer = Math.ceil(rightPer);
        }
        else {
            if (leftPer > rightPer) {
                leftPer = Math.ceil(leftPer);
                rightPer = Math.floor(rightPer);
            }
            else {
                leftPer = Math.floor(leftPer);
                rightPer = Math.ceil(rightPer);
            }

        }
        return {
            left: leftPer,
            right: rightPer
        };
    }
    function changeCheck(el) {
        $('.nx-check').addClass('transparent');
        el.removeClass('transparent');
    }
    function formatModule() {
        /* Breakpoint specific logic */
        var d = $('html');
        var PRT = 'bp-portrait', LND = 'bp-landscape', DSK = 'bp-desktop', DSKmin = 'bp-desktop-min', DSKmax = 'bp-desktop-max', TAB = 'bp-tablet', MOB = 'bp-mobile', notMOB = 'bp-not-mobile', notDSK = 'bp-not-desktop';

        if (d.hasClass(MOB)) {
            /* Mobile */
            var rightUser = $('.nx-faceoff-panel.nx-right .nx-faceoff-users', module);
            rightUser.insertAfter(rightUser.closest('.nx-faceoff-panel').children(':first-child'));
        }
        if (d.hasClass(TAB)) {
            /* Tablet */
            var rightUser = $('.nx-faceoff-panel.nx-right .nx-faceoff-users', module);
            rightUser.appendTo(rightUser.closest('.nx-faceoff-panel'));
        }
        if (d.hasClass(DSK)) {
            /* Desktop */
            var rightUser = $('.nx-faceoff-panel.nx-right .nx-faceoff-users', module);
            rightUser.appendTo(rightUser.closest('.nx-faceoff-panel'));
        }
    }
    function updateModel(data) {
        data.TotalVotes = data.FinishedProductAVoteCount + data.FinishedProductBVoteCount;
        var percentages = getPercentages(data);
        view.set('TotalVotes', data.FinishedProductAVoteCount + data.FinishedProductBVoteCount);
        view.set('PercentageA', percentages.left);
        view.set('PercentageB', percentages.right);
        view.set('HasCurrentUserVoted', data.HasCurrentUserVotedFor || data.HasCurrentUserVotedForB);
        view.set('HasCurrentUserVotedForA', (data.HasCurrentUserVotedForA) ? true : false);
        view.set('HasCurrentUserVotedForB', (data.HasCurrentUserVotedForB) ? true : false);

        $('.nx-faceoff-fill.nx-right').width(percentages.right + '%');
        $('.nx-faceoff-fill.nx-left').width(percentages.left + '%');

        drawCircles();
    }

    function parseData(data) {
        var imgAPath = $.map(data.FinishedProductA.AllImages, function (obj) {
            if (parseInt(obj.FilePath.split('_')[1].split('.')[0]) == data.FinishedProductAViewId)
                return obj.FilePath;
        })[0];
        data.FinishedProductA.DetailPath = imgRoot + imgAPath;
        var imgBPath = $.map(data.FinishedProductB.AllImages, function (obj) {
            if (parseInt(obj.FilePath.split('_')[1].split('.')[0]) == data.FinishedProductBViewId)
                return obj.FilePath;
        })[0];
        data.FinishedProductB.DetailPath = imgRoot + imgBPath;
        data.TotalVotes = data.FinishedProductAVoteCount + data.FinishedProductBVoteCount;
        var percentages = getPercentages(data);
        data.PercentageA = percentages.left;
        data.PercentageB = percentages.right;
        data.HasCurrentUserVoted = data.HasCurrentUserVotedFor || data.HasCurrentUserVotedForB;
        data.HasCurrentUserVotedForA = (data.HasCurrentUserVotedForA) ? 1 : 0;
        data.HasCurrentUserVotedForB = (data.HasCurrentUserVotedForB) ? 1 : 0;
        data.NavLinks = data.NextFaceoff != 0 && data.PreviousFaceoff != 0;
        data.NextFaceOffLink = "FaceOffTest.aspx?faceoff=" + data.NextFaceoff;
        data.PreviousFaceOffLink = "FaceOffTest.aspx?faceoff=" + data.PreviousFaceoff;

        $('.nx-faceoff-fill.nx-right').width(percentages.right + '%');
        $('.nx-faceoff-fill.nx-left').width(percentages.left + '%');

        return data;
    }
    // For testing locally
    //breakPoints.Set();
    //window.onresize = function () {
    //    breakPoints.Set();
    //};
    var view;

    (function init() {
        var id = 0;
        var param = window.urlParams.getParameter('faceOff');
        if (param) {
            id = param;
        }

        $.get(api + id, function (data) {

            var d = parseData(data);
            view = kendo.observable(d);

            totalInput = $('#nx-faceoff-total-value', module);
            circles.left = $('.nx-circle-container.nx-left .nx-circle', module);
            circles.right = $('.nx-circle-container.nx-right .nx-circle', module);
            labels.left = $('.nx-faceoff-percentage.nx-left', module);
            labels.right = $('.nx-faceoff-percentage.nx-right', module);
            labels.total = $('.nx-faceoff-total', module).text(data.FinishedProductAVoteCount + data.FinishedProductBVoteCount);

            kendo.bind(module, view);
            module.fadeIn().removeClass('hidden');
            formatModule();
            formatImages();

            $('a.nx-vote.nx-left').on('click', function (e) {
                $.post(api + view.Id + '/cap/' + view.FinishedProductA.Id + '/vote?returnStats=true', updateModel);
                return false;
            });
            $('a.nx-vote.nx-right').on('click', function (e) {
                $.post(api + view.Id + '/cap/' + view.FinishedProductB.Id + '/vote?returnStats=true', updateModel);
                return false;
            });
        });
    })();

})(jQuery, kendo, window);