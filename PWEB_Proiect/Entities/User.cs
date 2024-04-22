using PWEB_Proiect.Entities;
using System;

public class User : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; } 
    public string Firstname { get; set; } = default!;
    public string? Lastname { get; set; } 
    public string Cnp { get; set; } = default!;
    public Guid? CurrentRoomId { get; set; } // Presupunând că acesta poate fi null
    public string Sex { get; set; } = default!;
    public Guid? AssignedRoom { get; set; } // Presupunând că acesta poate fi null
    public byte[]? Salt { get; set; } // Pentru stocarea salt-ului ca array de bytes
    public string? StudentPhoto { get; set; }
    public double Medie { get; set; }
    public int An_studiu { get; set; }
    public int? Programa { get; set; }
    public bool Admin_approve { get; set; }
    public bool SpecialRights { get; set; }
    public IEnumerable<Documents>? Documents { get; set; } // Proprietate de navigare
    public IEnumerable<Notifications>? SentNotifications { get; set; }
    public IEnumerable<Notifications>? ReceivedNotifications { get; set; }
    public IEnumerable<Preference> Preferences { get; set; }
    public IEnumerable<Appointment>? Appointments { get; set; }
    public IEnumerable<Feedback>? Feedbacks { get; set; }


}
