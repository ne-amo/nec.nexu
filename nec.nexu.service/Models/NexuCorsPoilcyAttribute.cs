using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace nec.nexu.Controllers
{
    public class NexuCorsPoilcyAttribute : Attribute, ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        public NexuCorsPoilcyAttribute()
        {
            _policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = true /*@@TODO This is for testing only */,
                SupportsCredentials = true
            };
            //_policy.Origins.Add("http://host-name.com");
        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }
    }
}