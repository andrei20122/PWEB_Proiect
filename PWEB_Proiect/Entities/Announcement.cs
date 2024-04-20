using System;

public class Announcement : BaseEntity
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid PosterId { get; set;}
    public User Poster { get; set; }
    public IEnumerable<AnnouncementReceiver>? AnnouncementReceivers { get; set; }
}

