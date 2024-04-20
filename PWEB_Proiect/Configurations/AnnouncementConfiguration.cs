using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWEB_Proiect.Configurations
{
    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        void IEntityTypeConfiguration<Announcement>.Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.Property(n => n.Id)
                .IsRequired();
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Text)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.PosterId)
                .IsRequired();


        }
    }
}
