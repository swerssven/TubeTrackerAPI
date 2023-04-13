using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Post
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<PostComment> PostComments { get; } = new List<PostComment>();

    public virtual User User { get; set; }

    public virtual ICollection<User> Users { get; } = new List<User>();
}
