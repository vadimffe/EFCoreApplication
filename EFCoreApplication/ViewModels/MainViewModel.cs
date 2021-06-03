using EFCoreApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreApplication.ViewModels
{
  public class MainViewModel : BaseViewModel
  {
    public MainViewModel()
    {

    }

    private ObservableCollection<ProjectModel> projectTable;
    public ObservableCollection<ProjectModel> ProjectTable
    {
      get => this.projectTable;
      set
      {
        this.projectTable = value;
        this.OnPropertyChanged();
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
  }
}
