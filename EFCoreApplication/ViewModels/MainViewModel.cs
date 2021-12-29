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
      this.startDate = DateTime.Now;
      this.numList = new List<int>();

      this.InitializeNumList();
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
          Text = this.projectText,
          StartDate = this.startDate,
          Type = this.projectType,
          Duration = this.duration,
          Progress = this.progress,
          ProjectId = projectId,
        });

        context.SaveChangesAsync();
      }

      this.ProjectType = string.Empty;
      this.ProjectText = string.Empty;

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
        Progress = Math.Round(s.Progress * 10),
        ProjectNumber = context.Projects.FirstOrDefault(c => c.Id == s.ProjectId).ProjectNumber,
        ProjectId = s.ProjectId,
        StartDate = s.StartDate,
        Text = s.Text,
        Type = s.Type,
      }).ToList();

      this.TasksTable = new ObservableCollection<TaskModel>(tasks);
    }

    private void InitializeNumList()
    {
      this.numList.Add(1);
      this.numList.Add(2);
      this.numList.Add(3);
      this.numList.Add(4);
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

    private string projectText;
    public string ProjectText
    {
      get => this.projectText;
      set
      {
        this.projectText = value;
        this.OnPropertyChanged();
      }
    }

    private DateTime startDate;
    public DateTime StartDate
    {
      get => this.startDate;
      set
      {
        this.startDate = value;
        this.OnPropertyChanged();
      }
    }

    private int duration;
    public int Duration
    {
      get => this.duration;
      set
      {
        this.duration = value;
        this.OnPropertyChanged();
      }
    }

    private double progress;
    public double Progress
    {
      get => this.progress;
      set
      {
        this.progress = value;
        this.ProgressValue = string.Concat(Math.Round(value * 10).ToString(), " %");
        this.OnPropertyChanged();
      }
    }

    private string progressValue;
    public string ProgressValue
    {
      get => this.progressValue;
      set
      {
        this.progressValue = value;
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

    private List<int> numList;
    public List<int> NumList
    {
      get => this.numList;
      set
      {
        this.numList = value;
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
