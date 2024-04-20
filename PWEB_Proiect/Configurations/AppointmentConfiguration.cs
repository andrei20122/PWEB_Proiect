namespace PWEB_Proiect.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    void IEntityTypeConfiguration<Appointment>.Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(a => a.Id)
            .IsRequired();
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Time)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Place)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.HasOne(e => e.User)
            .WithMany(e => e.Appointments)
            .HasForeignKey(e => e.UserId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
