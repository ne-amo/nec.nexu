using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nec.nexu.Models;

namespace nec.nexu.ViewModels
{
    public class FaceoffViewModel
    {
        public int Id { get; set; }

        public int FinishedProductAVoteCount { get; set; }

        public int FinishedProductBVoteCount { get; set; }

        public virtual FinishedProductGalleryDetailView FinishedProductA { get; set; }

        public virtual int FinishedProductAViewId { get; set; }

        public string FinishedProductABackgroundColor { get; set; }

        public bool HasCurrentUserVotedForA = false;

        public virtual FinishedProductGalleryDetailView FinishedProductB { get; set; }

        public virtual int FinishedProductBViewId { get; set; }

        public string FinishedProductBBackgroundColor { get; set; }

        public bool HasCurrentUserVotedForB = false;

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual string Creator { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int NextFaceoff { get; set; }

        public virtual int PreviousFaceoff { get; set; }
    }
}