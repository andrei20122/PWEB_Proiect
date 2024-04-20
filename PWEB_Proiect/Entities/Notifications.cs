using System;

public class Notifications : BaseEntity
{
    public DateTime CreatedTime { get; set; } // Presupun că există o greșeală de tipărire în `creaedTime`
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string? Content { get; set; }
    public int Type { get; set; } // Poate fi mai bine reprezentat ca o enumerație

    // Proprietăți de navigare
    public User? Sender { get; set; }
    public User? Receiver { get; set; }
}

