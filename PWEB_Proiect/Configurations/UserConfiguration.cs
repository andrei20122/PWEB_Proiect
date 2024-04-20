using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PWEB.Entities
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Id)
                .IsRequired();
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
            .IsRequired();

            builder.Property(e => e.Firstname)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Lastname)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Cnp)
                .IsRequired()
                .HasMaxLength(13);

            builder.Property(e => e.CurrentRoomId)
                .IsRequired(false);

            builder.Property(e => e.Sex)
                .IsRequired()
                .HasMaxLength(1);

            builder.Property(e => e.AssignedRoom)
                .IsRequired(false);

            builder.Property(e => e.Salt)
                .IsRequired();

            builder.Property(e => e.StudentPhoto)
                .IsRequired(false)
                .HasMaxLength(255);


        }
    }
}
