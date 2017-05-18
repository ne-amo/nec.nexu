using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using nec.nexu.Extensions;
using nec.nexu.Models;
using nec.nexu.ViewModels;
using log4net;

namespace nec.nexu.Controllers
{
    [NexuCorsPoilcy]
    public class FaceOffsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        ILog log = log4net.LogManager.GetLogger
(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: api/FaceOffs
        public IHttpActionResult GetFaceOffs()
        {
            var fo = db.FaceOffs.Where(f => f.Active == true);
            return Ok(
                AutoMapper.Mapper.Map<List<FaceOff>, List<FaceoffViewModel>>(fo.ToList()));
        }

        // GET: api/FaceOffs/5
        [HttpGet]
        [ResponseType(typeof(FaceOff))]
        public IHttpActionResult GetFaceOff(int id)
        {
            FaceOff faceOff;
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            if (id == 0)
            {
                faceOff = db.FaceOffs.Where(x=>x.Active == true).OrderBy(x=>x.SortOrder).First();
            }
            else
            {
                faceOff  = db.FaceOffs.Find(id);
            }
            if (faceOff == null)
            {
                log.Error(string.Format("GET: api/FaceOffs/:id - Resource: Not Found"));
                return NotFound();
            }
            FaceoffViewModel model = AutoMapper.Mapper.Map<FaceOff, FaceoffViewModel>(faceOff);
            model.HasCurrentUserVotedForA = faceOff.FinishedProductAVotes.Any(x => x.IpAddress == ipAddress);
            model.HasCurrentUserVotedForB = faceOff.FinishedProductBVotes.Any(x => x.IpAddress == ipAddress);
            model.NextFaceoff = GetNextFaceOffId(faceOff.Id);
            model.PreviousFaceoff = GetPrevFaceOffId(faceOff.Id);
            return Ok(model);
        }
        private int GetNextFaceOffId(int currentId){
            // Get the next 
            int v = db.FaceOffs.Where(x => x.Active == true && x.Id > currentId).Select(x=>x.Id).FirstOrDefault();
            if(v == 0)
            {
                v = db.FaceOffs.Where(x => x.Id != currentId).Select(x => x.Id).FirstOrDefault();
            }
            return v;
        }
        private int GetPrevFaceOffId(int currentId)
        {
            int v = db.FaceOffs.Where(x => x.Active == true && x.Id < currentId).Select(x => x.Id).OrderByDescending(x => x).FirstOrDefault();
            if (v == 0)
            {
                v = db.FaceOffs.Where(x => x.Id != currentId).Select(x => x.Id).OrderByDescending(x => x).FirstOrDefault();
            }
            return v;
        }
        private string generateFakeIp()
        {
            Random r = new Random();
            return r.Next(0, 999) + "." + r.Next(0, 999) + "." + r.Next(0, 999) + "." + r.Next(0, 999);
        }

        //Put: api/faceoff/capA/:id/vote
        [Route("api/faceoffs/{id}/cap/{capid}/vote")]
        [ResponseType(typeof (void))]
        public IHttpActionResult VoteOnFaceoffCap(int capId, int id, bool returnStats = false)
        {
            string ipAddress = generateFakeIp(); //@@ TODOD undo comment   HttpContext.Current.Request.UserHostAddress;
            var faceOff = db.FaceOffs.FirstOrDefault(f => f.Id == id);


            if (faceOff == null)
            {
                log.Error(string.Format("PUT: api/faceoff/capA/:id/vote - Resource: Not Found"));
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/faceoff/capA/:id/vote - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            if (id != faceOff.Id)
            {
                log.Error(string.Format("PUT: api/faceoff/capA/:id/vote - Resource: ID Mismatch"));
                return BadRequest();
            }
            bool votingForA = faceOff.FinishedProductAId == capId;
            bool votingForB = faceOff.FinishedProductBId == capId;



            // User has voted for A before, and wants to vote for A
           //if (faceOff.FinishedProductAVotes.Any(x => x.IpAddress == ipAddress) && votingForA)
           //{
           //    // do nothing 
           //}
            // User has voted for A before, and wants to vote for B
            if(votingForB){
                if (faceOff.FinishedProductAVotes.Any(x => x.IpAddress == ipAddress))
                {
                    faceOff.FinishedProductAVotes.Remove(faceOff.FinishedProductAVotes.First(x => x.IpAddress == ipAddress));
                }
                if(!faceOff.FinishedProductBVotes.Any(x => x.IpAddress == ipAddress))
                    faceOff.FinishedProductBVotes.Add(new FaceoffVote { IpAddress = ipAddress });
            }



            // User has voted for B before, and wants to vote for B
            //if (faceOff.FinishedProductBVotes.Any(x => x.IpAddress == ipAddress) && votingForB)
            //{
            //    // do nothing 
            //}
            // User has voted for B before, and wants to vote for A
            if(votingForA){
                if (faceOff.FinishedProductBVotes.Any(x => x.IpAddress == ipAddress))
                {
                    faceOff.FinishedProductBVotes.Remove(faceOff.FinishedProductBVotes.First(x => x.IpAddress == ipAddress));
                }
                if (!faceOff.FinishedProductAVotes.Any(x => x.IpAddress == ipAddress))
                    faceOff.FinishedProductAVotes.Add(new FaceoffVote { IpAddress = ipAddress });
            }

            /*
            if (faceOff.FinishedProductA.Id == capId)
            {
                if(faceOff.FinishedProductAVotes== null)
                    faceOff.FinishedProductAVotes = new List<FaceoffVote>();
                if (faceOff.FinishedProductAVotes.FirstOrDefault(vote => vote.IpAddress == ipAddress) == null)
                {
                    faceOff.FinishedProductAVotes.Add(new FaceoffVote{ IpAddress = ipAddress});
                }
            }
            else if (faceOff.FinishedProductB.Id == capId)
            {
                if (faceOff.FinishedProductBVotes == null)
                    faceOff.FinishedProductBVotes = new List<FaceoffVote>();
                if (faceOff.FinishedProductBVotes.FirstOrDefault(vote => vote.IpAddress == ipAddress) == null)
                {
                    faceOff.FinishedProductBVotes.Add(new FaceoffVote { IpAddress = ipAddress });
                }
            }
            */
            db.Entry(faceOff).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FaceOffExists(id))
                {
                    log.Error(string.Format("PUT: api/faceoff/capA/:id/vote"), ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/faceoff/capA/:id/vote"), ex);
                    throw;
                }
            }
            if (returnStats)
            {
                FaceoffViewModel model = AutoMapper.Mapper.Map<FaceOff, FaceoffViewModel>(faceOff);
                model.HasCurrentUserVotedForA = faceOff.FinishedProductAVotes.Any(x => x.IpAddress == ipAddress);
                model.HasCurrentUserVotedForB = faceOff.FinishedProductBVotes.Any(x => x.IpAddress == ipAddress);
                model.NextFaceoff = GetNextFaceOffId(faceOff.Id);
                model.PreviousFaceoff = GetPrevFaceOffId(faceOff.Id);
                return Ok(model);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // PUT: api/FaceOffs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFaceOff(int id, FaceOff faceOff)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("PUT: api/FaceOffs/:id - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            if (id != faceOff.Id)
            {
                log.Error(string.Format("PUT: api/FaceOffs/:id - Resource: ID Mismatch"));
                return BadRequest();
            }

            FaceOff dbFaceOff = db.FaceOffs.Find(id);
            if (dbFaceOff != null)
            {
                dbFaceOff.Build(faceOff);
                db.Entry(dbFaceOff).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FaceOffExists(id))
                {
                    log.Error(string.Format("PUT: api/FaceOffs/:id"),ex);
                    return NotFound();
                }
                else
                {
                    log.Error(string.Format("PUT: api/FaceOffs/:id"),ex);
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/FaceOffs
        [ResponseType(typeof(FaceOff))]
        public IHttpActionResult PostFaceOff(FaceOff faceOff)
        {
            if (!ModelState.IsValid)
            {
                log.Error(string.Format("POST: api/FaceOffs - Resource: Not Valid"));
                return BadRequest(ModelState);
            }

            db.FaceOffs.Add(faceOff);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = faceOff.Id }, faceOff);
        }

        // DELETE: api/FaceOffs/5
        [ResponseType(typeof(FaceOff))]
        public IHttpActionResult DeleteFaceOff(int id)
        {
            FaceOff faceOff = db.FaceOffs.Find(id);
            if (faceOff == null)
            {
                log.Error(string.Format("DELETE: api/FaceOffs/:id - Resource: Not Found"));
                return NotFound();
            }

            db.FaceOffs.Remove(faceOff);
            db.SaveChanges();

            return Ok(faceOff);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FaceOffExists(int id)
        {
            return db.FaceOffs.Count(e => e.Id == id) > 0;
        }
    }
}