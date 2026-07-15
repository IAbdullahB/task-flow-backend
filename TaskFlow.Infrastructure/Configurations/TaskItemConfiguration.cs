using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(i => i.Title).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Description).HasMaxLength(500);
    }
}
