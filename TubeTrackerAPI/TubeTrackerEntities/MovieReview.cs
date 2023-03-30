using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class MovieReview
{
    public int MovieReviewId { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual Movie Movie { get; set; }

    public virtual User User { get; set; }
}
