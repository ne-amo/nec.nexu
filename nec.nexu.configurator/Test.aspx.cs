using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft;
using nec.nexu.Models;

namespace nec.nexu.configurator
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //getToken();
        }
        public void getToken()
        {
            string token = NexuLoginHelper.GetAccessToken();
            nx_hfnexu.Value = token;
        }
    }
}