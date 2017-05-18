
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace nec.nexu.bridge
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string userName { get; set; }
    }

    public class ApiConnector : HttpClient
    {
        private string apiConnection = System.Configuration.ConfigurationManager.AppSettings["api_url"];

        public ApiConnector()
        {
            this.BaseAddress = new Uri(apiConnection);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public ApiConnector(AccessToken token)
        {
            this.BaseAddress = new Uri(apiConnection);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
        }
        public ApiConnector(string accessToken)
        {
            this.BaseAddress = new Uri(apiConnection);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<T> Read<T>(string url)
        {
            HttpResponseMessage response = await this.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                T ret = await response.Content.ReadAsAsync<T>();
                return ret;
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// Executes a read synchronously.
        /// </summary>
        public T ReadValue<T>(string url)
        {
            return this.Read<T>(url).Result;
        }
        public async Task<bool> Update<T>(string url, int id, T entity)
        {
            HttpResponseMessage response = await this.PutAsJsonAsync(url + id.ToString(), entity);
            return response.IsSuccessStatusCode;
        }
        /// <summary>
        /// Executes a update synchronously.
        /// </summary>
        public bool UpdateValue<T>(string url, int id, T entity)
        {
            return this.Update<T>(url, id, entity).Result;
        }


        public async Task<T> Create<T>(string url, T entity)
        {
            HttpResponseMessage response = await this.PostAsJsonAsync(url, entity);
            if (response.IsSuccessStatusCode)
            {
                T ret = await response.Content.ReadAsAsync<T>();
                return ret;
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// Executes a Create synchronously.
        /// </summary>
        public T CreateValue<T>(string url, T entity)
        {
            return this.Create<T>(url, entity).Result;
        }



        public async Task<S> PostRead<T, S>(string url, T entity)
        {
            HttpResponseMessage response = await this.PostAsJsonAsync(url, entity);
            if (response.IsSuccessStatusCode)
            {
                S ret = await response.Content.ReadAsAsync<S>();
                return ret;
            }
            else
            {
                return default(S);
            }
        }
        /// <summary>
        /// Executes a POST synchronously.
        /// </summary>
        public S PostReadValue<T, S>(string url, T entity)
        {
            return this.PostRead<T, S>(url, entity).Result;
        }


        public void UseRootUrl()
        {
            this.BaseAddress = new Uri(apiConnection.Replace("api/", string.Empty));
        }
    }
}
