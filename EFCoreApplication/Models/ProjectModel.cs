using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCoreApplication.Models
{
  public class ProjectModel
  {
    [Key]
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string ProjectNumber { get; set; }

    public virtual ICollection<TaskModel> Tasks { get; set; }
  }
}
