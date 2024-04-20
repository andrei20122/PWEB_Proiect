using System;

public class Room : BaseEntity
{
    public int Capacity { get; set; }
    public string Building { get; set; }
    public int Floor { get; set; }
    public float MonthlyCost { get; set; }
    public int NrRoom { get; set; }
    public IEnumerable<Preference>? Preferences { get; set; }
    public IEnumerable<Appointment>? Appointments { get; set; }
    public IEnumerable<AnnouncementReceiver>? AnnouncementReceivers { get; set; }
}
