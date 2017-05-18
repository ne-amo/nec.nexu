using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using nec.nexu.Models;

namespace nec.nexu.configurator
{

    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string userName { get; set; }
    }
    internal class AccessError
    {
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public static class NexuLoginHelper
    {
        /// <summary>
        /// Grant type for acquiring bearer tokens when logging in
        /// </summary>
        private static string LOGIN_GRANT_TYPE = "password";
        /// <summary>
        /// Grant type for acquiring bearer tokens already logged in
        /// </summary>
        private static string REFRESH_GRANT_TYPE = "refresh_token";
        /// <summary>
        /// Client ID to be sent for refresh tokens
        /// </summary>
        private static string CLIENT_ID = System.Configuration.ConfigurationManager.AppSettings["auth_client_id"];
        /// <summary>
        /// Client secret (password) for refresh tokens
        /// </summary>
        private static string CLIENT_SECRET = System.Configuration.ConfigurationManager.AppSettings["auth_client_secret"];
        /// <summary>
        /// Key to be used for storing the refresh token
        /// </summary>
        private static string STORAGE_ID = System.Configuration.ConfigurationManager.AppSettings["auth_storage_id"];




        private static int _____COOKIE_LIFESPAN = 0;
        /// <summary>
        /// Minutes that the refresh token cookie will live. Default is 14 days.
        /// </summary>
        private static int COOKIE_LIFESPAN
        {
            get
            {
                if (_____COOKIE_LIFESPAN == 0)
                {
                    try
                    {
                        using (ApiConnector api = new ApiConnector())
                        {
                            HttpResponseMessage response = api.GetAsync("api/Account/LoginLifespan?app=" + CLIENT_ID).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                return JsonConvert.DeserializeObject<int>(response.Content.ReadAsStringAsync().Result);
                            }
                            else
                            {
                                return 20160;
                            }
                        }
                    }
                    catch
                    {
                        return 20160;
                    }
                }
                else
                {
                    return _____COOKIE_LIFESPAN;
                }
            }
        }


        public static string RefreshKey
        {
            get
            {
                return STORAGE_ID;
            }
        }



        /// <summary>
        /// This method returns the access token key if the user is already logged in.
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            try
            {
                HttpRequest currentRequest = HttpContext.Current.Request;
                HttpCookie cookie = currentRequest.Cookies[RefreshKey];
                if (cookie != null)
                {
                    //If there is a refresh token cookie, attempt to refresh login.
                    AccessToken token = _GetAccessToken(cookie.Value);
                    if (token != null)
                    {
                        // Login successful, store the new refresh key and return the access token.
                        StoreCookie(token);
                        return token.access_token;
                    }
                    else
                    {
                        // The refresh cookie is invalid. We should clear it
                        ClearCookie();
                        return null;
                    }
                }
                else
                {
                    // Check session to see if a non-persistent login is active
                    string refreshKey = (string)HttpContext.Current.Session[RefreshKey];
                    if (string.IsNullOrEmpty(refreshKey))
                    {
                        return null;
                    }
                    else
                    {
                        AccessToken token = _GetAccessToken(refreshKey);
                        if (token != null)
                        {
                            // Login successful, store the new refresh key and return the access token.
                            HttpContext.Current.Session[RefreshKey] = token.refresh_token;
                            return token.access_token;
                        }
                        else
                        {
                            // Login refresh failed. Clear the session variable.
                            HttpContext.Current.Session.Remove(RefreshKey);
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClearCookie();
                HttpContext.Current.Session.Remove(RefreshKey);
                throw ex;
            }
        }
        /// <summary>
        /// Handles logging the user in.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="persist"></param>
        /// <returns></returns>
        public static string GetAccessToken(string userName, string password, bool persist)
        {
            try
            {
                AccessToken token = _GetAccessToken(userName, password);
                if (token == null)
                    return null;
                if (persist)
                {
                    StoreCookie(token);
                }
                else
                {
                    HttpContext.Current.Session[RefreshKey] = token.refresh_token;
                }
                return token.access_token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Logs out the current user. Removes any persistence of login.
        /// </summary>
        /// <param name="accessToken"></param>
        public static void Logout(string accessToken)
        {
            using (ApiConnector api = new ApiConnector(accessToken))
            {
                HttpResponseMessage response = api.GetAsync("api/Account/Logout?app=" + CLIENT_ID).Result;
                HttpContext.Current.Session.Remove(RefreshKey);
                ClearCookie();
            }
        }

        private static AccessToken _GetAccessToken(string refreshToken)
        {
            return _GetToken(GetTokenParameters(refreshToken));
        }
        private static AccessToken _GetAccessToken(string userName, string password)
        {

            return _GetToken(GetTokenParameters(userName, password));
        }

        private static AccessToken _GetToken(FormUrlEncodedContent parameters)
        {
            using (ApiConnector api = new ApiConnector())
            {
                api.UseRootUrl();
                var content = parameters;
                HttpResponseMessage response = api.PostAsync("token", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(response.Content.ReadAsStringAsync().Result);
                    return token;
                }
                else
                {
                    string jsonError = response.Content.ReadAsStringAsync().Result;
                    AccessError errorMessage = JsonConvert.DeserializeObject<AccessError>(jsonError);
                    throw new Exception(errorMessage.error_description);
                }
            }
        }
        private static FormUrlEncodedContent GetTokenParameters(string userName, string password)
        {
            //@@TODO add client secrets
            return new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("grant_type", LOGIN_GRANT_TYPE),
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("client_id", CLIENT_ID)
            });
        }
        private static FormUrlEncodedContent GetTokenParameters(string refreshToken)
        {
            //@@TODO add client secrets
            return new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>("grant_type", REFRESH_GRANT_TYPE),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", CLIENT_ID)
            });
        }

        private static void StoreCookie(AccessToken token)
        {
            HttpResponse response = HttpContext.Current.Response;
            var cookie = new HttpCookie(NexuLoginHelper.RefreshKey, token.refresh_token);
            cookie.Expires = DateTime.Now.AddMinutes(COOKIE_LIFESPAN);
            response.Cookies.Add(cookie);
        }

        private static void ClearCookie()
        {
            HttpResponse response = HttpContext.Current.Response;
            var cookie = new HttpCookie(NexuLoginHelper.RefreshKey);
            cookie.Expires = DateTime.Now.AddDays(-1d);
            response.Cookies.Add(cookie);
        }
    }
}