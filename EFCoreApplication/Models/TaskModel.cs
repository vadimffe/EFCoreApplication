using System;
using System.ComponentModel.DataAnnotations;

namespace EFCoreApplication.Models
{
  public class TaskModel
  {
    public Guid Id { get; set; }
    public string ProjectNumber { get; set; }
    public string Text { get; set; }
    public DateTime StartDate { get; set; }
    public int Duration { get; set; }
    public double Progress { get; set; }
    public int? ParentId { get; set; }
    public string Type { get; set; }

    public Guid ProjectId { get; set; }
    public virtual ProjectModel ProjectModel { get; set; }
  }
}
