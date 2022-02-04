using EFCoreApplication.Commands;
using EFCoreApplication.Data;
using EFCoreApplication.Models;
using EFCoreApplication.UserControls;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EFCoreApplication.ViewModels
{
  public class MainViewModel : BaseViewModel
  {
    public MainViewModel()
    {
      this.startDate = DateTime.Now;
      this.numList = new List<int>();

      this.GaugeTotal = 10;
      this.initialRotation = 160;
      this.MaxAngle = 30;

      this.InitializeNumList();
      this.InitializeProjectList();
      this.InitializeTaskList();
      this.InitializePieChart();
      this.InitializeColumnChart();
      this.InitializeGaugeChart();
    }

    public ICommand AddProjectToDatabaseCommand => new AsyncRelayCommand(param => this.AddProjectToDatabase());
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
          Revenue = this.revenue,
          Duration = this.duration,
          Progress = this.progress,
          ProjectId = projectId,
        });

        context.SaveChangesAsync();
      }

      this.ProjectType = string.Empty;
      this.ProjectText = string.Empty;
      this.Revenue = 0;
      this.SelectedProjectNumber = new ProjectModel();
      this.Progress = 0;
      this.ProgressValue = string.Empty;
      this.StartDate = DateTime.Now;
      this.Duration = 1;

      this.InitializeTaskList();
      this.InitializePieChart();
      this.InitializeColumnChart();
    }

    private void InitializeProjectList()
    {
      using SQLiteDBContext context = new SQLiteDBContext();
      IQueryable<ProjectModel> projectNumbers = context.Projects.Select(x => new ProjectModel
      {
        Id = x.Id,
        ProjectName = x.ProjectName,
        ProjectNumber = x.ProjectNumber,
        Tasks = x.Tasks,
      }).OrderBy(o => o.ProjectNumber);

      this.Projects = new ObservableCollection<ProjectModel>(projectNumbers);
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
        Revenue = s.Revenue,
        StartDate = s.StartDate,
        Text = s.Text,
        Type = s.Type,
      }).ToList();

      this.TasksTable = new ObservableCollection<TaskModel>(tasks);
    }

    private void InitializePieChart()
    {
      IEnumerable<ISeries> projectNumbers = this.TasksTable.Select(x =>
      new PieSeries<double>
      {
        Values = new List<double> { x.Revenue },
        Name = x.Text,
      });

      this.Series = new ObservableCollection<ISeries>(projectNumbers);
    }

    private void InitializeColumnChart()
    {
      IEnumerable<ISeries> projectNumbers = this.TasksTable.Select(x =>
      new ColumnSeries<double>
      {
        Values = new List<double> { x.Revenue },
        Name = x.Text,
      });

      this.ColumnSeries = new ObservableCollection<ISeries>(projectNumbers);
    }

    private void InitializeGaugeChart()
    {
      IEnumerable<ISeries> Series = new List<ISeries>
      {
          new PieSeries<double> { Values = new List<double> { 40 }, Name = "a" },
      };

      IEnumerable<ISeries> projectNumbers = this.TasksTable.Select(x =>
      new PieSeries<double>
      {
        Values = new List<double> { 6 },
      });

      this.GaugeSeries = new ObservableCollection<ISeries>(Series);
    }

    private void InitializeNumList()
    {
      this.numList.Add(1);
      this.numList.Add(2);
      this.numList.Add(3);
      this.numList.Add(4);
      this.numList.Add(5);
    }

    private async Task AddProjectToDatabase()
    {
      if (!string.IsNullOrEmpty(this.ProjectNumber))
      {
        using (SQLiteDBContext context = new SQLiteDBContext())
        {
          if (!string.IsNullOrEmpty(this.ProjectNumber))
          {
            context.Projects.Add(new ProjectModel { ProjectNumber = this.ProjectNumber });

            await context.SaveChangesAsync();
          }
        }

        this.InitializeProjectList();

        this.ProjectNumber = string.Empty;
      }
      else
      {
        NotificationDialog view = new NotificationDialog
        {
          DataContext = new NotificationViewModel()
        };

        object result = await DialogHost.Show(view, "MainDialogHost", this.ExtendedOpenedEventHandler, this.ExtendedNotificationClosingEventHandler);
      }
    }

    private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
    {
      Console.WriteLine("You could intercept the open and affect the dialog using eventArgs.Session.");
    }

    private void ExtendedNotificationClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
    {
      if ((bool)eventArgs.Parameter == false) return;

      eventArgs.Cancel();

      Task.Delay(TimeSpan.FromSeconds(0))
        .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
            TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
    {
      if ((bool)eventArgs.Parameter == false) return;

      //OK, lets cancel the close...
      eventArgs.Cancel();

      //...now, lets update the "session" with some new content!
      eventArgs.Session.UpdateContent(new SampleProgressDialog());
      //note, you can also grab the session when the dialog opens via the DialogOpenedEventHandler

      //lets run a fake operation for 3 seconds then close this baby.
      Task.Delay(TimeSpan.FromSeconds(3))
          .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
              TaskScheduler.FromCurrentSynchronizationContext());
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

    private double revenue;
    public double Revenue
    {
      get => this.revenue;
      set
      {
        this.revenue = value;
        this.OnPropertyChanged();
      }
    }

    private double gaugeTotal;
    public double GaugeTotal
    {
      get => this.gaugeTotal;
      set
      {
        this.gaugeTotal = value;
        this.OnPropertyChanged();
      }
    }

    private double maxAngle;
    public double MaxAngle
    {
      get => this.maxAngle;
      set
      {
        this.maxAngle = value;
        this.OnPropertyChanged();
      }
    }

    private double initialRotation;
    public double InitialRotation
    {
      get => this.initialRotation;
      set
      {
        this.initialRotation = value;
        this.OnPropertyChanged();
      }
    }

    private IEnumerable<ISeries> series;
    public IEnumerable<ISeries> Series
    {
      get => this.series;
      set
      {
        this.series = value;
        this.OnPropertyChanged();
      }
    }

    private IEnumerable<ISeries> columnSeries;
    public IEnumerable<ISeries> ColumnSeries
    {
      get => this.columnSeries;
      set
      {
        this.columnSeries = value;
        this.OnPropertyChanged();
      }
    }

    private IEnumerable<ISeries> gaugeSeries;
    public IEnumerable<ISeries> GaugeSeries
    {
      get => this.gaugeSeries;
      set
      {
        this.gaugeSeries = value;
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

    private ObservableCollection<ProjectModel> projects;
    public ObservableCollection<ProjectModel> Projects
    {
      get => this.projects;
      set
      {
        this.projects = value;
        this.OnPropertyChanged();
      }
    }

    public List<ICartesianAxis> XAxis { get; set; } = new List<ICartesianAxis>
    {
      new Axis
      {
          IsVisible = false
      }
    };
  }
}
