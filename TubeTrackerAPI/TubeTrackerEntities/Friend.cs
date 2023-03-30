using System;
using System.Collections.Generic;

namespace TubeTrackerAPI.TubeTrackerEntities;

public partial class Friend
{
    public int UserId { get; set; }

    public int FriendUserId { get; set; }

    public int FriendshipStatus { get; set; }

    public virtual User FriendUser { get; set; }

    public virtual User User { get; set; }
}
