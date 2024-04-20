using System;

public class Documents
{
    public string Id { get; set; }
    public DateTime UploadTime { get; set; }
    public string Type { get; set; } // Poate fi mai bine reprezentat ca o enumerație sau o clasă separată
    public Guid UserId { get; set; }
    public string Status { get; set; } // Poate fi mai bine reprezentat ca o enumerație
    public string Feedback { get; set; }

    // Proprietate de navigare
    public User User { get; set; }
}


