using System;
using System.Collections.Generic;

namespace nec.nexu.JsonModels
{
    public class FaceOff: nec.nexu.JsonModels.EntityBase
    {
        public FaceOff()
        {
           // this.CreateDate = DateTime.Now;
        }

        public virtual FinishedProduct FinishedProductA { get; set; }

        public virtual FinishedProduct FinishedProductB { get; set; }

        public virtual List<FaceoffVote> FinishedProductAVotes { get; set; }

        public virtual List<FaceoffVote> FinishedProductBVotes { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual string Creator { get; set; }

        public virtual DateTime CreateDate { get; set; }

        // public virtual bool IsActive { get; set; }
    }
}