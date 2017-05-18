using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using nec.nexu.Models;

namespace nec.nexu.api.ApiControllers
{
	public class AuthorizationHeaderHandler : DelegatingHandler
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			IEnumerable<string> apiKeyHeaderValues = null;
			if (request.Headers.TryGetValues("X-ApiKey", out apiKeyHeaderValues)) {
				var apiKeyHeaderValue = apiKeyHeaderValues.First();

				var keys = db.ApiKeys.Where(key => key.Key == apiKeyHeaderValue).ToArray();

				if (keys.Length == 1)
				{
					// now validate the api key sent matches the rocord retreived.
					var username = (keys.Length == 1 ? keys[0].ApplicationName : "UnauthorizedUser");

					var usernameClaim = new Claim(ClaimTypes.Name, username);
					var identity = new ClaimsIdentity(new[] { usernameClaim }, "ApiKey");
					var principal = new ClaimsPrincipal(identity);

					Thread.CurrentPrincipal = principal;
				}


			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}
