using Microsoft.EntityFrameworkCore;
using PWEB_Proiect.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWEB_Proiect.Configurations
{
    public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
    {
        void IEntityTypeConfiguration<Feedback>.Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.Property(f => f.Id)
                .IsRequired();
            builder.HasKey(f => f.Id);

            builder.Property(f => f.UserId)
                .IsRequired();

            builder.Property(f => f.Subject)
                .HasMaxLength(255);

            builder.Property(f => f.IsSatisfied)
                .IsRequired();

            builder.Property(f => f.Features)
                .HasMaxLength(255);

            builder.Property(f => f.Comments)
                .HasMaxLength(255);
            builder.Property(f => f.CreatedTime)
                .IsRequired();

            builder.HasOne(e => e.User) // Aici se specifică o relație de unu-la-mulți.
                .WithMany(e => e.Feedbacks) // Aici se furnizează maparea inversă pentru relația de unu-la-mulți.
                .HasForeignKey(e => e.UserId) // Aici este specificată coloana cheii străine.
                .HasPrincipalKey(e => e.Id) // Aici se specifică cheia referențiată în tabela referențiată.
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
