using System;
using System.Collections.Generic;

namespace nec.nexu.Models
{
    public class FaceOff: EntityBase<int>
    {
        public FaceOff()
        {
            this.CreateDate = DateTime.Now;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;
            this.Active = true;
            this.FinishedProductAVotes = new List<FaceoffVote>();
            this.FinishedProductBVotes = new List<FaceoffVote>();
        }

        public virtual FinishedProduct FinishedProductA { get; set; }
        public int? FinishedProductAId { get; set; }
        public string FinishedProductABackgroundColor { get; set; }
        public int FinishedProductAViewId { get; set; }

        public virtual FinishedProduct FinishedProductB { get; set; }
        public int? FinishedProductBId { get; set; }
        public string FinishedProductBBackgroundColor { get; set; }
        public int FinishedProductBViewId { get; set; }

        public virtual List<FaceoffVote> FinishedProductAVotes { get; set; }

        public virtual List<FaceoffVote> FinishedProductBVotes { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual string Creator { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual int SortOrder { get; set; }


        /// <summary>
        /// Use for PUT(update) requests to build object from transfer object
        /// </summary>
        /// <param name="faceoff"></param>
        public void Build(FaceOff faceoff)
        {
            this.Active = faceoff.Active;
            this.Creator = faceoff.Creator;
            this.Description = faceoff.Description;
            this.DisplayName = faceoff.DisplayName;
            this.EndDate = faceoff.EndDate;
            this.FinishedProductAId = faceoff.FinishedProductAId;
            this.FinishedProductBId = faceoff.FinishedProductBId;
            this.FinishedProductABackgroundColor = faceoff.FinishedProductABackgroundColor;
            this.FinishedProductBBackgroundColor = faceoff.FinishedProductBBackgroundColor;
            this.Title = faceoff.Title;
            this.SortOrder = faceoff.SortOrder;
            this.FinishedProductBViewId = faceoff.FinishedProductBViewId;
            this.FinishedProductAViewId = faceoff.FinishedProductAViewId;
        }
    }
}