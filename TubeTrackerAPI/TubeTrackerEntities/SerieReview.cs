using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class SerieReview
{
    public int SerieReviewId { get; set; }

    public int UserId { get; set; }

    public int SerieId { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual Series Serie { get; set; }

    public virtual User User { get; set; }
}
