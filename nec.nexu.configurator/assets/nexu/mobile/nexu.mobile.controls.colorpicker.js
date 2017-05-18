///
///nec.nexu.mobile.controls.colorpicker
///

//build the nav for nexu - 1st level will mainly be static
//color picker is separate from nav elements

; (function ($, kendo, _, window) {

	/* global namespaces */
	var nec = window._nec_ || {};
	nec.nexu = nec.nexu || {};
	nec.nexu.ui = nec.nexu.ui || {};
	nec.nexu.models = nec.nexu.models || {};
	mobile = nec.nexu.moile = nec.nexu.mobile || {};
	nec.nexu.mobile.controls = nec.nexu.mobile.controls || {};
	colorpicker = nec.nexu.mobile.controls.colorpicker = nec.nexu.mobile.controls.colorpicker || {};
	
	var baseOptions = {
		dataSource: [],
		select: function () { }
	};

	var targetElement = $("#colorpicker-menu");

	/**
     * Build the color list in an area with each color group 
     */
	colorpicker.init = function (options) {
		//clear content 
		targetElement.html('');

		var colorPicker = $(String.format("<ul class='colorpicker-list'></ul>"));

		options = $.extend(baseOptions, options);

		var colorGroups = options.dataSource;
		onClickFunction = options.select;

		//move recent colors to top of list
		var recentIndex = _.findIndex(colorGroups, { id: -2 });
		colorGroups.splice(0, 0, colorGroups.splice(recentIndex, 1)[0]);

		for (var g = 0; g < colorGroups.length; g++) {
			//build groups
			var group = colorGroups[g];

			var colorGroup = $(String.format("<li id='g{1}' class='colorpicker-group' data-groupid='{1}'>{0}</li>", group.DisplayName, group.Id));
			if (group.Id == -2)
			{
			    colorGroup.addClass("hideme");
			}
			colorPicker.append(colorGroup);

			var colorList = $(String.format("<ul id='ccl{0}' class='colorpicker-color-list'></ul>", group.Id));

			//colors for each group
			for (var c = 0; c < group.Colors.length; c++) {
				var color = group.Colors[c];
				var hex = color.Hex.replace(/^#/, '');
				colorList.append(createColorButton(color, onClickFunction));

			}
			colorPicker.append(colorList);
		}

		//add to color-picker element
		targetElement.append(colorPicker);
	}

	appendRecentColor = function (e) {
        //add to recent list id = -2
		console.log('add to recent color');
		var args = e.data;
		var color = args.color;
        var recents = nec.nexu.data.ColorLists.get(-2);
        if (!_.findWhere(recents.Colors, { Id: color.Id })) {
            recents.Colors.push(color);
        	//push to UI
            $("#ccl-2").append(createColorButton(color, args.func));
            $("ul.colorpicker-list #g-2").removeClass("hideme");
        }

	}


	createColorButton = function (colorData, onClickFunction) {
		var colorEle = $(String.format("<li class='colorpicker-color' style='background-color:{1}; color:{2};'>{0}</li>", colorData.DisplayName, "#" + colorData.Hex, (isDarkColor(colorData.Hex) ? "#fff" : "inherit")));
		$.data(colorEle,"color", colorData);
		colorEle.on("click", colorData, onClickFunction).on("click", {color: colorData, func: onClickFunction}, appendRecentColor);
		return colorEle;
	}

	/******************************/
	//add to global namespace
	window._nec_ = nec;

})(jQuery, kendo, _, window);