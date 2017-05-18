using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nec.nexu.site.admin.Models
{
    public static class ApiRoutes
    {
        public static string FACEOFF_API_URL = "api/faceoffs/";

        public static string FINISHEDPRODUCT_API_URL = "api/finishedproducts/";

        public static string HIERARCHY_API_URL = "api/hierarchies/";

        public static string CONFIGURATOR_LINK_API_URL = "api/ConfiguratorDirectLinks/";

        public static string PRODUCT_TEMPLATES_API_URL = "api/ProductTemplates/";

        public static string COLORLIST_API_URL = "api/ColorLists/";

        public static string MATERIALS_API_URL = "api/masterdata/materials/";

        public static string DESIGNS_API_URL = "api/designs/";

        public static string PRODUCT_REGION_GROUPS_API_URL = "api/masterdata/productregiongroups/";

        public static string DESIGN_LOCATION_API_URL = "api/masterdata/designlocations/";

        public static string COLOR_DIFF_RULE_API_URL = "api/rule/colordifferentiation/";

        public static string COLORWAY_RULE_API_URL = "api/rule/colorway/";

        public static string DESIGN_COLORS_API_URL = "api/masterdata/designcolors";

        public static string IMAGEDATA = "api/masterdata/imagedata/";

        public static string ROAL_DESIGNCOLOR = "api/roal/designcolor/";
        public static string ROAL_MATERIAL = "api/roal/material/";
        public static string ROAL_DESIGN = "api/roal/design/";
    }
}