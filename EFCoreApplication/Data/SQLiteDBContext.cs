using EFCoreApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreApplication.Data
{
  public class SQLiteDBContext : DbContext
  {
    public virtual DbSet<ProjectModel> Appsettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(@"Data Source=sqlitedemo.db");

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
    }
  }
}
