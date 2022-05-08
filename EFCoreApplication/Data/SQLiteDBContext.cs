using EFCoreApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApplication.Data
{
  public class SQLiteDBContext : DbContext
  {
    public virtual DbSet<ProjectModel> Projects { get; set; }
    public virtual DbSet<TaskModel> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(@"Data Source=sqlitedemo.db");

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      // configures one-to-many relationship
      builder.Entity<TaskModel>()
          .HasOne<ProjectModel>(s => s.ProjectModel)
          .WithMany(g => g.Tasks)
          .HasForeignKey(k => k.ProjectId);
    }
  }
}
