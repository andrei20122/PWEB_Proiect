using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWEB_Proiect.Configurations
{
    public class DocumentsConfiguration : IEntityTypeConfiguration<Documents>
    { 

        void IEntityTypeConfiguration<Documents>.Configure(EntityTypeBuilder<Documents> builder)
        {
            builder.Property(d => d.Id)
                .IsRequired();
            builder.HasKey(d => d.Id);

            builder.Property(d => d.UploadTime)
                .IsRequired();

            builder.Property(d => d.Type)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.UserId)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.Feedback)
                .IsRequired(false)
                .HasMaxLength(255);

            builder.HasOne(e => e.User)
             .WithMany(e => e.Documents)
             .HasForeignKey(e => e.UserId)
             .HasPrincipalKey(e => e.Id)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
