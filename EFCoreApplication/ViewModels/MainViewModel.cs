using EFCoreApplication.Commands;
using EFCoreApplication.Data;
using EFCoreApplication.Models;
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
    }

    public ICommand AddProjectToDatabaseCommand => new RelayCommand(param => this.AddProjectToDatabase());

    public ICommand AddTasksToDatabaseCommand => new RelayCommand(param => this.AddTasksToDatabase());

    private void AddTasksToDatabase()
    {
      Random rnd = new Random();

      using (var db = new SQLiteDBContext())
      {
        if (this.SelectedProjectNumber!= null)
        {
          var tasks = db.Set<TaskModel>();

          for (int i = 1; i < rnd.Next(6, 15); i++)
          {
            tasks.Add(new TaskModel
            {
              ProjectNumber = this.SelectedProjectNumber,
              Text = string.Format("{0} {1}", "Task text", i),
              StartDate = DateTime.Now,
              Duration = 2 + i,
            });
          }

          db.SaveChangesAsync();
        }
      }
    }

    private void InitializeProjectList()
    {
      using (var db = new SQLiteDBContext())
      {
        var projectNumbers = from projectDB in db.ProjectModel
                             select projectDB.ProjectNumber;

        List<string> queryResults = new List<string>();
        queryResults.AddRange(projectNumbers);

        this.ProjectNumberList = new ObservableCollection<string>(queryResults);
      }
    }

    private void InitializeTaskList()
    {
      using (var db = new SQLiteDBContext())
      {
        var projectNumbers = from projectDB in db.TaskModel
                             where projectDB.ProjectNumber == this.SelectedProjectNumber
                             select new TaskModel { 
                               Id = projectDB.Id,
                               ProjectNumber = projectDB.ProjectNumber,
                               StartDate = projectDB.StartDate,
                             };

        List<TaskModel> queryResults = new List<TaskModel>();
        queryResults.AddRange(projectNumbers);

        this.ProjectTable = new ObservableCollection<TaskModel>(queryResults);
      }
    }

    private void AddProjectToDatabase()
    {
      using (var db = new SQLiteDBContext())
      {
        if (this.ProjectNumber != null)
        {
          var projects = db.Set<ProjectModel>();
          projects.Add(new ProjectModel { ProjectNumber = this.ProjectNumber });

          db.SaveChangesAsync();
        }
      }

      InitializeProjectList();

      this.ProjectNumber = string.Empty;
    }

    private ObservableCollection<TaskModel> projectTable;
    public ObservableCollection<TaskModel> ProjectTable
    {
      get => this.projectTable;
      set
      {
        this.projectTable = value;
        this.OnPropertyChanged();
      }
    }

    private string selectedProjectNumber;
    public string SelectedProjectNumber
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

    private ObservableCollection<string> projectNumberList;
    public ObservableCollection<string> ProjectNumberList
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
