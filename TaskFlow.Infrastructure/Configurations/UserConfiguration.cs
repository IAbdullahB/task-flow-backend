using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.TaskItems)
        .WithOne(t => t.AssignedUser)
        .HasForeignKey(t => t.AssignedUserId)
        .OnDelete(DeleteBehavior.SetNull);

        builder.Property(u => u.UserName).IsRequired();
        builder.Property(u => u.Email).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        

    }

}
