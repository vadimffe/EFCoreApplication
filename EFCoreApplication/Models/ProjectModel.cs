using System;

namespace EFCoreApplication.Models
{
  public class ProjectModel
  {
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public string ProjectNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
  }
}
