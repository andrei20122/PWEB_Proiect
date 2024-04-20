using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace PWEB_Proiect.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    void IEntityTypeConfiguration<Room>.Configure(EntityTypeBuilder<Room> builder)
    {
        builder.Property(r => r.Id)
            .IsRequired();
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Capacity)
            .IsRequired();

        builder.Property(r => r.Building)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.Floor)
            .IsRequired();

        builder.Property(r=> r.MonthlyCost)
            .IsRequired();

        builder.Property(r => r.NrRoom)
            .IsRequired();

    }
}


