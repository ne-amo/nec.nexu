﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using nec.nexu.Models;
using nec.nexu.Providers;
using nec.nexu.Results;
using System.Linq;

namespace nec.nexu.Controllers
{
    
	[Authorize]
	[RoutePrefix("api/Account")]
    [NexuCorsPoilcy]
	public class AccountController : ApiController
	{
		private const string LocalLoginProvider = "Local";
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager,
			ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
		{
			UserManager = userManager;
			AccessTokenFormat = accessTokenFormat;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
                if (Request == null)
                {
                    return _userManager;
                }
				return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

		// GET api/Account/UserInfo
		//[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("UserInfo")]
		public async Task<UserInfoViewModel> GetUserInfo()
		{
			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			var userInfoViewModel = new UserInfoViewModel {
				Email = User.Identity.GetUserName(),
				HasRegistered = externalLogin == null,
				LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
			};

			ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user != null)
			{
                using(ApplicationDbContext db = new ApplicationDbContext()){
				    userInfoViewModel.FirstName = user.FirstName;
				    userInfoViewModel.LastName = user.LastName;
				    userInfoViewModel.Nickname = user.Nickname;
                    userInfoViewModel.CountryName = (user.UserCountry == null) ? null : user.UserCountry.CountryDescription;
                    userInfoViewModel.ProfileImageUrl = user.ProfileImageFileName;
                    userInfoViewModel.ReceiveEmail = user.ReceiveEmail;
                    userInfoViewModel.FinishedProductsCount = db.FinishedProducts.Count(x => x.Creator.Id == user.Id && x.Active == true);
                    userInfoViewModel.Id = user.Id;
                }
			}
			return userInfoViewModel;
		}

		// POST api/Account/Logout
        //[AllowAnonymous]
		[Route("Logout")]
		public async Task<IHttpActionResult> Logout(string app = null)
		{
            var u = User.Identity;
            if (app != null)
            {
                // revoke refresh token
                using (TokenHelper t = new TokenHelper())
                {
                    await t.RemoveRefreshToken(u.GetUserName(), app);
                }
            }
			Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			return Ok();
		}

		// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
		[Route("ManageInfo")]
		public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
		{
			IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null) {
				return null;
			}

			List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

			foreach (IdentityUserLogin linkedAccount in user.Logins) {
				logins.Add(new UserLoginInfoViewModel {
					LoginProvider = linkedAccount.LoginProvider,
					ProviderKey = linkedAccount.ProviderKey
				});
			}

			if (user.PasswordHash != null) {
				logins.Add(new UserLoginInfoViewModel {
					LoginProvider = LocalLoginProvider,
					ProviderKey = user.UserName,
				});
			}

			return new ManageInfoViewModel {
				LocalLoginProvider = LocalLoginProvider,
				Email = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
			};
		}


		// POST api/Account/ChangePassword
		[Route("ChangeAccountInfo")]
		public async Task<IHttpActionResult> ChangeAccountInfo(ChangeAccountInfoBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}


			ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user != null) {
				user.FirstName = model.NewFirstname;
				user.LastName = model.NewLastname;
				user.Nickname = model.NewNickname;
			}

			IdentityResult result = await UserManager.UpdateAsync(user);

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}


		// POST api/Account/ChangePassword
		[Route("ChangePassword")]
		public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
				model.NewPassword);

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}

		// POST api/Account/SetPassword
		[Route("SetPassword")]
		public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}

		// POST api/Account/AddExternalLogin
		[Route("AddExternalLogin")]
		public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

			if (ticket == null || ticket.Identity == null || (ticket.Properties != null
				&& ticket.Properties.ExpiresUtc.HasValue
				&& ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow)) {
				return BadRequest("External login failure.");
			}

			ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

			if (externalData == null) {
				return BadRequest("The external login is already associated with an account.");
			}

			IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
				new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}

		// POST api/Account/RemoveLogin
		[Route("RemoveLogin")]
		public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			IdentityResult result;

			if (model.LoginProvider == LocalLoginProvider) {
				result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
			}
			else {
				result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
					new UserLoginInfo(model.LoginProvider, model.ProviderKey));
			}

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		[AllowAnonymous]
		[Route("ExternalLogin", Name = "ExternalLogin")]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if (error != null) {
				return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
			}

			if (!User.Identity.IsAuthenticated) {
				return new ChallengeResult(provider, this);
			}

			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			if (externalLogin == null) {
				return InternalServerError();
			}

			if (externalLogin.LoginProvider != provider) {
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new ChallengeResult(provider, this);
			}

			ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
				externalLogin.ProviderKey));

			bool hasRegistered = user != null;

			if (hasRegistered) {
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

				ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
				   OAuthDefaults.AuthenticationType);
				ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
					CookieAuthenticationDefaults.AuthenticationType);

				AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
				Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
			}
			else {
				IEnumerable<Claim> claims = externalLogin.GetClaims();
				ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
				Authentication.SignIn(identity);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
		[AllowAnonymous]
		[Route("ExternalLogins")]
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
		{
			IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
			List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

			string state;

			if (generateState) {
				const int strengthInBits = 256;
				state = RandomOAuthStateGenerator.Generate(strengthInBits);
			}
			else {
				state = null;
			}

			foreach (AuthenticationDescription description in descriptions) {
				ExternalLoginViewModel login = new ExternalLoginViewModel {
					Name = description.Caption,
					Url = Url.Route("ExternalLogin", new {
						provider = description.AuthenticationType,
						response_type = "token",
						client_id = Startup.PublicClientId,
						redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
						state = state
					}),
					State = state
				};
				logins.Add(login);
			}

			return logins;
		}

		// POST api/Account/Register
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}


			var user = new ApplicationUser() {
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.Firstname,
				LastName = model.Lastname,
				Nickname = model.Nickname
			};

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string ipAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                Country c = db.GetCountryByIp(ipAddress);
                user.UserCountry = c;
            }

			IdentityResult result = await UserManager.CreateAsync(user, model.Password);

			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			return Ok();
		}

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IHttpActionResult> SignInAsync(LocalLoginModel model, bool isPersistent = true)
        {
            try
            {
                ApplicationUser user = UserManager.Find(model.UserName, model.Password);
                if (user == null)
                    return BadRequest("Invalid username or password");

                IAuthenticationManager authenticationManager = Request.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var identity = await UserManager.CreateIdentityAsync(
                   user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(
                   new AuthenticationProperties()
                   {
                       IsPersistent = isPersistent
                   }, identity);

                return Ok();
            }
            catch
            {
                return BadRequest("There was an unexpected error while attempting login");
            }
        }
        [HttpGet]
        [Authorize]
        [Route("UserId")]
        public IHttpActionResult GetUserId()
        {
            return Ok(User.Identity.GetUserId());
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterApplication")]
        public async Task<IHttpActionResult> RegisterApplication(AppClient clientJson)
        {
            using (TokenHelper t = new TokenHelper())
                {
                    try
                    {
                        t.CreateClient(clientJson);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
            }
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("LoginLifespan")]
        public async Task<IHttpActionResult> GetRefreshLifespan(string app)
        {
            using (TokenHelper t = new TokenHelper())
            {
                try
                {
                    return Ok(t.FindClient(app).RefreshTokenLifeTime);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }


        /*
        // POST api/Account/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LocalLoginModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = UserManager.Find(model.UserName, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.Current.Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ClaimsIdentity identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = model.RememberMe;
                    authenticationManager.SignIn(props, identity);
                    //if (new System.Web.Mvc.UrlHelper().IsLocalUrl(returnUrl))
                    //{
                    return Ok();
                    //}
                    //else
                    //{
                    //    return BadRequest();
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return Ok();
        }
        */

		// POST api/Account/RegisterExternal



		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			var info = await Authentication.GetExternalLoginInfoAsync();
			if (info == null) {
				return InternalServerError();
			}

			var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

			IdentityResult result = await UserManager.CreateAsync(user);
			if (!result.Succeeded) {
				return GetErrorResult(result);
			}

			result = await UserManager.AddLoginAsync(user.Id, info.Login);
			if (!result.Succeeded) {
				return GetErrorResult(result);
			}
			return Ok();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
                if(UserManager != null)
				    UserManager.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Helpers

		private IAuthenticationManager Authentication
		{
			get { return Request.GetOwinContext().Authentication; }
		}

		private IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null) {
				return InternalServerError();
			}

			if (!result.Succeeded) {
				if (result.Errors != null) {
					foreach (string error in result.Errors) {
						ModelState.AddModelError("", error);
					}
				}

				if (ModelState.IsValid) {
					// No ModelState errors are available to send, so just return an empty BadRequest.
					return BadRequest();
				}

				return BadRequest(ModelState);
			}

			return null;
		}

		private class ExternalLoginData
		{
			public string LoginProvider { get; set; }
			public string ProviderKey { get; set; }
			public string UserName { get; set; }

			public IList<Claim> GetClaims()
			{
				IList<Claim> claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

				if (UserName != null) {
					claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
				}

				return claims;
			}

			public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
			{
				if (identity == null) {
					return null;
				}

				Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

				if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
					|| String.IsNullOrEmpty(providerKeyClaim.Value)) {
					return null;
				}

				if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer) {
					return null;
				}

				return new ExternalLoginData {
					LoginProvider = providerKeyClaim.Issuer,
					ProviderKey = providerKeyClaim.Value,
					UserName = identity.FindFirstValue(ClaimTypes.Name)
				};
			}
		}

		private static class RandomOAuthStateGenerator
		{
			private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

			public static string Generate(int strengthInBits)
			{
				const int bitsPerByte = 8;

				if (strengthInBits % bitsPerByte != 0) {
					throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
				}

				int strengthInBytes = strengthInBits / bitsPerByte;

				byte[] data = new byte[strengthInBytes];
				_random.GetBytes(data);
				return HttpServerUtility.UrlTokenEncode(data);
			}
		}

		#endregion



























	}
}
