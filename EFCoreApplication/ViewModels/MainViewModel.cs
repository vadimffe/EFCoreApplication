using EFCoreApplication.Commands;
using EFCoreApplication.Data;
using EFCoreApplication.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace EFCoreApplication.ViewModels
{
  public class MainViewModel : BaseViewModel
  {
    public MainViewModel()
    {
      this.InitializeProjectList();
      this.InitializeTaskList();
    }

    public ICommand AddProjectToDatabaseCommand => new RelayCommand(param => this.AddProjectToDatabase());

    public ICommand AddTaskToDatabaseCommand => new RelayCommand(param => this.AddTaskToDatabase());

    private void AddTaskToDatabase()
    {
      Random rnd = new Random();

      using SQLiteDBContext context = new SQLiteDBContext();
      if (this.SelectedProjectNumber != null && this.SelectedProjectNumber.Id != Guid.Empty)
      {
        Guid projectId = context.Projects.FirstOrDefault(c => c.ProjectNumber == this.selectedProjectNumber.ProjectNumber).Id;

        context.Tasks.Add(new TaskModel
        {
          ProjectNumber = this.SelectedProjectNumber.ProjectNumber,
          Text = string.Format("{0} {1}", "Random task number", rnd.Next(1, 55)),
          StartDate = DateTime.Now,
          Type = this.projectType,
          Duration = 2,
          ProjectId = projectId,
        }) ;

        context.SaveChangesAsync();
      }

      this.ProjectType = string.Empty; 

      this.InitializeTaskList();
    }

    private void InitializeProjectList()
    {
      using SQLiteDBContext context = new SQLiteDBContext();
      IQueryable<ProjectModel> projectNumbers = context.Projects.Select(x => new ProjectModel
      {
        Id = x.Id,
        ProjectName = x.ProjectName,
        ProjectNumber = x.ProjectNumber,
      });

      this.ProjectNumberList = new ObservableCollection<ProjectModel>(projectNumbers);
    }

    private void InitializeTaskList()
    {
      using SQLiteDBContext context = new SQLiteDBContext();
      List<TaskModel> tasks = context.Tasks.Select(s => new TaskModel
      {
        Id = s.Id,
        Duration = s.Duration,
        ParentId = s.ParentId,
        Progress = s.Progress,
        ProjectNumber = context.Projects.FirstOrDefault(c => c.Id == s.ProjectId).ProjectNumber,
        ProjectId = s.ProjectId,
        StartDate = s.StartDate,
        Text = s.Text,
        Type = s.Type,
      }).ToList();

      this.TasksTable = new ObservableCollection<TaskModel>(tasks);
    }

    private void AddProjectToDatabase()
    {
      using (SQLiteDBContext context = new SQLiteDBContext())
      {
        if (!string.IsNullOrEmpty(this.ProjectNumber))
        {
          context.Projects.Add(new ProjectModel { ProjectNumber = this.ProjectNumber });

          context.SaveChangesAsync();
        }
      }

      this.InitializeProjectList();

      this.ProjectNumber = string.Empty;
    }

    private ObservableCollection<TaskModel> tasksTable;
    public ObservableCollection<TaskModel> TasksTable
    {
      get => this.tasksTable;
      set
      {
        this.tasksTable = value;
        this.OnPropertyChanged();
      }
    }

    private ProjectModel selectedProjectNumber;
    public ProjectModel SelectedProjectNumber
    {
      get => this.selectedProjectNumber;
      set
      {
        this.selectedProjectNumber = value;
        this.OnPropertyChanged();
        this.InitializeTaskList();
      }
    }

    private string projectNumber;
    public string ProjectNumber
    {
      get => this.projectNumber;
      set
      {
        this.projectNumber = value;
        this.OnPropertyChanged();
      }
    }

    private string projectType;
    public string ProjectType
    {
      get => this.projectType;
      set
      {
        this.projectType = value;
        this.OnPropertyChanged();
      }
    }

    private ObservableCollection<ProjectModel> projectNumberList;
    public ObservableCollection<ProjectModel> ProjectNumberList
    {
      get => this.projectNumberList;
      set
      {
        this.projectNumberList = value;
        this.OnPropertyChanged();
      }
    }
  }
}
