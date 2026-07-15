using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure;
public class ApplicationDbContext(DbContextOptions<DbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }   

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
}
