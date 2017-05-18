using System;
using System.Web;


public class NexuTokenHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //if (context.Request.QueryString["logout"] != null && bool.Parse(context.Request.QueryString["logout"].ToString()))
            //{
            //    nec.nexu.configurator.NexuLoginHelper.Logout(nec.nexu.configurator.NexuLoginHelper.GetAccessToken());
            //}
            //else
            //{
                string accessToken = nec.nexu.configurator.NexuLoginHelper.GetAccessToken();
                context.Response.Write(accessToken);
            //}
        }

        #endregion

        private bool isAllowed(Uri origin)
        {
            return true;
        }
    }
