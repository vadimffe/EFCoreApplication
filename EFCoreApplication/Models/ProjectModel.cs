using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCoreApplication.Models
{
  public class ProjectModel
  {
    public Guid Id { get; set; }
    public string ProjectName { get; set; }
    public string ProjectNumber { get; set; }
    public DateTime CreationDate { get; set; }

    public virtual ICollection<TaskModel> Tasks { get; set; }
  }
}
