﻿using System.Web;
using System.Web.Mvc;

namespace nec.nexu.site.admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}