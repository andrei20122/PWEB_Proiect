using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace PWEB_Proiect.Configurations
{
    public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
    {
        void IEntityTypeConfiguration<Preference>.Configure(EntityTypeBuilder<Preference> builder)
        {
            builder.Property(p => p.Id)
                .IsRequired();
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.RoomId)
                .IsRequired();

            builder.Property(p => p.Priority)
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany(e => e.Preferences)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Room)
                .WithMany(e => e.Preferences)
                .HasForeignKey(e => e.RoomId)
                .HasPrincipalKey(e => e.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
