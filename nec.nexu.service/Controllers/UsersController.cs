using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using nec.nexu.Models;
using nec.nexu.ViewModels;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class UsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("api/users/topusers")]
        public IHttpActionResult GetTopUsers(int pageNumber = 1, int pageSize = 25)
        {
            var topUsers = (pageNumber > 1)
                ? db.Users
                    .Where(u => u.Active)
                    .OrderByDescending(tp => tp.DesignScore)
                    .Skip(pageSize*(pageNumber - 1))
                    .Take(pageSize).ToList()
                : db.Users
                    .Where(u => u.Active)
                    .OrderByDescending(tp => tp.DesignScore)
                    .Take(pageSize).ToList();

            return Ok(new PagedResponse<UserDesignerRankViewModel>
            {
                Items = AutoMapper.Mapper.Map<List<ApplicationUser>, List<UserDesignerRankViewModel>>(
                topUsers),
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = db.Users.Count(u => u.Active && u.DesignScore > 0)
            });
        }


        /*
        [HttpGet]
        [Route("api/users/test/")]
        public IHttpActionResult GetLocalized(string lang = "en")
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            var obj = db.LocalizationTests.First();
            return Ok(AutoMapper.Mapper.Map<nec.nexu.Models.LocalizedTest, nec.nexu.ViewModels.LocalizedTest>(obj));
        }
        [HttpGet]
        [Route("api/users/addtest/")]
        public IHttpActionResult CreateLocalized(string en = null, string fr = null, string de = null)
        {
            var d = new nec.nexu.Models.LocalizedTest();
            d.DisplayName = en;
            d.LocalizedDisplayName["en"] = en;
            d.LocalizedDisplayName["fr"] = fr;
            d.LocalizedDisplayName["de"] = de;
            db.LocalizationTests.Add(d);
            db.SaveChanges();
            return Ok();
        }
        */





    }
}
