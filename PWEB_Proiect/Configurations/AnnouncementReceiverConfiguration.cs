namespace PWEB_Proiect.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AnnouncementReceiverConfiguration : IEntityTypeConfiguration<AnnouncementReceiver>
{
    public void Configure(EntityTypeBuilder<AnnouncementReceiver> builder)
    {
        builder.Property(an => an.Id)
                .IsRequired();
        builder.HasKey(an => an.Id);

        builder.Property(an => an.AnnouncementId)
            .IsRequired();

        builder.Property(an => an.ReceiverRoomId)
            .IsRequired();

        builder.HasOne(ar => ar.Announcement)
            .WithMany(a => a.AnnouncementReceivers)
            .HasForeignKey(ar => ar.AnnouncementId);
    }
}
