using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWEB_Proiect.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        void IEntityTypeConfiguration<Notifications>.Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.Property(n => n.Id)
                .IsRequired();
            builder.HasKey(n => n.Id);

            builder.Property(n => n.CreatedTime)
                .IsRequired();

            builder.Property(n => n.SenderId)
                .IsRequired();

            builder.Property(n => n.ReceiverId)
                .IsRequired();

            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(n => n.Type)
                .IsRequired();

            builder.HasOne(e => e.Sender) // Aici se specifică o relație de unu-la-mulți.
            .WithMany(e => e.SentNotifications) // Aici se furnizează maparea inversă pentru relația de unu-la-mulți.
            .HasForeignKey(e => e.SenderId) // Aici este specificată coloana cheii străine.
            .HasPrincipalKey(e => e.Id) // Aici se specifică cheia referențiată în tabela referențiată.
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Receiver) // Aici se specifică o relație de unu-la-mulți.
            .WithMany(e => e.ReceivedNotifications) // Aici se furnizează maparea inversă pentru relația de unu-la-mulți.
            .HasForeignKey(e => e.ReceiverId) // Aici este specificată coloana cheii străine.
            .HasPrincipalKey(e => e.Id) // Aici se specifică cheia referențiată în tabela referențiată.
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            /*builder.HasOne(e => e.Receiver) // Aici se specifică o relație de unu-la-mulți.
            .WithMany(e => e.Notifications) // Aici se furnizează maparea inversă pentru relația de unu-la-mulți.
            .HasForeignKey(e => e.ReceiverId) // Aici este specificată coloana cheii străine.
            .HasPrincipalKey(e => e.Id) // Aici se specifică cheia referențiată în tabela referențiată.
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);*/
        }
    }
}
