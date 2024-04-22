using System;

public class Preference : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }

    public User User { get; set; }
    public Room Room { get; set; }
}
