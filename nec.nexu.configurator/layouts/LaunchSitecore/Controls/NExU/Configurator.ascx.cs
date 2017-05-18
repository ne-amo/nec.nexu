using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace nec.nexu.configurator.NExU
{
    public partial class Configurator : System.Web.UI.UserControl
    {
        private System.Data.SqlClient.SqlConnection connection =
            new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NexuConnection"].ToString());
        public string message = "get everything ready";
        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                using (connection)
                {
                    System.Data.SqlClient.SqlCommand com = new System.Data.SqlClient.SqlCommand("GetRandomMessage", connection);
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    message = com.ExecuteScalar().ToString();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
    }
}