using Microsoft.EntityFrameworkCore;
using PWEB.Entities;
using PWEB_Proiect.Configurations;
using PWEB_Proiect.Entities;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Documents> Documents { get; set; }
    public DbSet<Notifications> Notifications { get; set; }
    public DbSet<Room> Rooms { get; set; }

    public DbSet<Feedback> Feedbacks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {}

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentsConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationsConfiguration());
        modelBuilder.ApplyConfiguration(new RoomConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new PreferenceConfiguration());
        modelBuilder.ApplyConfiguration(new AnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new AnnouncementReceiverConfiguration());
        modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
