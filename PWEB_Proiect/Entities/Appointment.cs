using System;

public class Appointment : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime Time { get; set; }
    public string Place { get; set; } = default!;
    public string Status { get; set; } = default!; // Poate fi o enumerație în codul real

    public User User { get; set; }

    public Room Room { get; set; }
}

