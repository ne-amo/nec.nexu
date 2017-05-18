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

    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string token = NexuLoginHelper.GetAccessToken();
                if (token != null)
                {
                    setError(null);
                    nx_hfnexu.Value = token;
                }
            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
        }

        protected void nx_click_login(object sender, EventArgs e)
        {
            string email = nx_login_email.Value;
            string password = nx_login_password.Value;
            bool persist = bool.Parse(nx_login_persist.Value);
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // error
            }
            try
            {
                string token = NexuLoginHelper.GetAccessToken(email, password, persist);
                if (token != null)
                {
                    setError(null);
                    nx_hfnexu.Value = token;
                }
            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
        }
        protected void nx_click_register(object sender, EventArgs e)
        {
            string email = nx_register_email.Value;
            string name = nx_register_name.Value;
            string country = nx_register_country.Value;
            string password = nx_register_pw.Value;
            string confirm = nx_register_cpw.Value;
            bool persist = bool.Parse(nx_register_persist.Value);
        }
        protected void nx_click_logout(object sender, EventArgs e)
        {
            NexuLoginHelper.Logout(nx_hfnexu.Value);
            nx_hfnexu.Value = string.Empty;
        }
        protected void setError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                lblError.Text = string.Empty;
                lblError.Visible = false;
                nx_hfnexu.Value = string.Empty;
            }
            else
            {
                lblError.Text = message;
                lblError.Visible = true;
                nx_hfnexu.Value = string.Empty;
            }
        }
    }
}