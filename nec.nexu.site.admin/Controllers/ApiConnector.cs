using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace nec.nexu.site.admin.Controllers
{
    public class ApiConnector : HttpClient
    {
        private string apiConnection = System.Configuration.ConfigurationManager.AppSettings["api_url"];
        public ApiConnector()
        {
            this.BaseAddress = new Uri(apiConnection);
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
        public async Task<bool> Update<T>(string url, int id, T entity)
        {
            HttpResponseMessage response = await this.PutAsJsonAsync(url + id.ToString(), entity);
            return response.IsSuccessStatusCode;
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

    }
}