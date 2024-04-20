using System;

public class AnnouncementReceiver : BaseEntity
{
    public Guid AnnouncementId { get; set; }
    public Guid ReceiverRoomId { get; set; }

    public Announcement Announcement { get; set; }
    public Room ReceiverRoom { get; set; }
}

