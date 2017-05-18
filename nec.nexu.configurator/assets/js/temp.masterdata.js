if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
        });
    };
}



var colors = colors1.concat(colors2),
colorList1 = {
    DisplayName: 'All Colors',
    IsDefault: true,
    Colors: colors1
},
colorList2 = {
    DisplayName: 'Seasonal Colors',
    Colors: colors2
};
var firstColorway = {
    Design: {
        Id: 1,
        Name: 'MA48',
        HeightPixel:182,
        WidthPixel:218
    },
    Layers: [{
        DisplayName: 'Gray Image',
        Code: 'gray',
        SortOrder: 0,
        Recolorable: false,
        Color: colors[0],
        ImageDataId:109,
        LayerFile: {
            Id: 11,
            FilePath: 'Images/MA48_gray.png'
        }
    }, {
        DisplayName: 'Team Image',
        Code: 'team',
        SortOrder: 1,
        Recolorable: false,
        Color: colors[0],
        LayerFile: {
            FilePath: 'Images/MA48_team.png'
        }
    }, {
        DisplayName: 'Layer 1',
        Code: 'layer1',
        SortOrder: 1,
        Recolorable: true,
        Color: colors[4],
        ImageDataId: 102,
        LayerFile: {
            Id: 12,
            FilePath: 'Images/MA48_red.png'
        },
        Rule: 'and',
        Rules: [
            { field: 'title', operator: "!=", value: "BACHELOR BUTTON" }
        ]
    }, {
        DisplayName: 'Layer 2',
        Code: 'layer2',
        SortOrder: 2,
        Recolorable: true,
        Color: colors[6],
        ImageDataId: 103,
        LayerFile: {
            Id: 13,
            FilePath: 'Images/MA48_green.png'
        },
        Rule:false,
        Rules: []
    }, {
        DisplayName: 'Layer 3',
        Code: 'layer3',
        SortOrder: 3,
        Recolorable: true,
        Color: colors[1],
        ImageDataId: 104,
        LayerFile: {
            Id:14,
            FilePath: 'Images/MA48_blue.png'
        },
        Rule: 'or',
        Rules: [
            { field: 'Id', operator: "==", value: 1114 },
            { field: 'Id', operator: "==", value: 1139 },
            { field: 'Id', operator: "==", value: 1127 }
        ]
    }
	]
};
var secondColorway = {
    Design: {
        Id: 2,
        Name: 'PT10',
        HeightPixel:180,
        WidthPixel:270
    },
    Layers: [{
        DisplayName: 'Gray Image',
        Code: 'gray',
        SortOrder: 0,
        Recolorable: false,
        Color: colors[0],
        ImageDataId:108,
        LayerFile: {
            FilePath: 'Images/PT10_gray.png'
        }
    }, {
        DisplayName: 'Team Image',
        Code: 'team',
        SortOrder: 1,
        Recolorable: false,
        Color: colors[0],
        LayerFile: {
            FilePath: 'Images/PT10_team.png'
        }
    }, {
        DisplayName: 'Layer 1',
        Code: 'layer1',
        SortOrder: 1,
        Recolorable: true,
        Color: colors[0],
        ImageDataId: 106,
        LayerFile: {
            FilePath: 'Images/PT10_red.png'
        },
        Rule:false,
        Rules: []
    }, {
        DisplayName: 'Layer 2',
        Code: 'layer2',
        SortOrder: 2,
        Recolorable: true,
        Color: colors[1],
        ImageDataId:107,
        LayerFile: {
            FilePath: 'Images/PT10_green.png'
        },
        Rule: false,
        Rules: []
    }
	]
};
var thirdColorway = {
    Design: {
        Id: 3,
        Name: 'PT06'
    },
    Layers: [{
        DisplayName: 'Gray Image',
        Code: 'gray',
        SortOrder: 0,
        Recolorable: false,
        Color: colors[0],
        LayerFile: {
            FilePath: 'Images/PT06_gray.png'
        },
        Rules: []
    }, {
        DisplayName: 'Team Image',
        Code: 'team',
        SortOrder: 1,
        Recolorable: false,
        Color: colors[0],
        LayerFile: {
            FilePath: 'Images/PT06_team.png'
        },
        Rule: false,
        Rules: []
    }, {
        DisplayName: 'Layer 1',
        Code: 'layer1',
        SortOrder: 1,
        Recolorable: true,
        Color: colors[0],
        LayerFile: {
            FilePath: 'Images/PT06_layer1.png'
        },
        Rule: false,
        Rules: []
    }, {
        DisplayName: 'Layer 2',
        Code: 'layer2',
        SortOrder: 2,
        Recolorable: true,
        Color: colors[1],
        LayerFile: {
            FilePath: 'Images/PT06_layer2.png'
        },
        Rule: false,
        Rules: []
    }
	]
};
var colorways = [firstColorway, secondColorway];//, thirdColorway];
var tempRegions = [{
    Region: {
        Id:2,
        Code: 'LFW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:4,
        Code: 'LMW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:6,
        Code: 'LBW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:7,
        Code: 'RBW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:5,
        Code: 'RMW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:3,
        Code: 'RFW'
    },
    Material: colors[4]
}, {
    Region: {
        Id:1,
        Code: 'Visor'
    },
    Material: colors[3]
}, {
    Region: {
        Id:8,
        Code: 'Button'
    },
    Material: colors[3]
}, {
    Region: {
        Id:10,
        Code: 'Eyelets'
    },
    Material: colors[3]
}, {
    Region: {
        Id:9,
        Code: 'Stitching'
    },
    Material: colors[3]
}
],
tempLogos = [{
    Location: {
        Code: 'FrontLogo'
    }
},
{
    Location: {
        Code: 'BackLogo'
    }
    //,DesignColorway: {}
}
];




var ProductTemplate = {
    Id:0,
    Active:true,
    DisplayName:"Test",
    SortOrderLogo:1,
    DesignLocations:[],
    RegionGroups:[],
    DefaultMaterial:null,
    Images:[]
}


var images = [
            { OfficialImage: { FilePath: 'Images/MA48_team.png' }, DisplayName: 'Bay Bears', DefaultColorway: firstColorway },
            { OfficialImage: { FilePath: 'Images/PT10_team.png' }, DisplayName: 'Charlotte Hornets', DefaultColorway: secondColorway }/*,
            { OfficialImage: { FilePath: 'Images/PT06_team.png' }, DisplayName: 'Chicago Bulls', DefaultColorway: thirdColorway }*/
            ];
var tmp = "<li class='nexu-lc-item'><span class='nexu-lc-helper'></span><img src='#= OfficialImage.FilePath #' title='#= DisplayName #' alt='image' /></li>"




var tmp_ProductRegions = [{
		Id : 1,
		DisplayName : '5950 Regions',
		Label : 'something something',
		SortOrder : 1,
		ForceGroup : false,
		AllowMatching : false,
		Regions : [{
				Id : 2,
				Code : 'LFW'
			}, {
				Id : 4,
				Code : 'LMW'
			}, {
				Id : 6,
				Code : 'LBW'
			}, {
				Id : 7,
				Code : 'RBW'
			}, {
				Id : 5,
				Code : 'RMW'
			}, {
				Id : 3,
				Code : 'RFW'
			}
		]
	}, {
		Id : 2,
		DisplayName : '5950 Visor',
		Label : 'Visor',
		SortOrder : 2,
		ForceGroup : false,
		AllowMatching : false,
		Regions : [{
				Id : 1,
				Code : 'Visor'
			}
		]
	}, {
		Id : 3,
		DisplayName : '5950 Button',
		Label : 'Button',
		SortOrder : 3,
		ForceGroup : false,
		AllowMatching : false,
		Regions : [{
				Id : 8,
				Code : 'Button'
			}
		]
	}, {
		Id : 4,
		DisplayName : '5950 Eyelets',
		Label : 'Eyelets',
		SortOrder : 4,
		ForceGroup : false,
		AllowMatching : true,
		Regions : [{
				Id : 10,
				Code : 'Eyelets'
			}
		]
	}, {
		Id : 5,
		DisplayName : '5950 Stitching',
		Label : 'Stitching',
		SortOrder : 5,
		ForceGroup : false,
		AllowMatching : true,
		Regions : [{
				Id : 9,
				Code : 'Stitching'
			}
		]
	}
]

