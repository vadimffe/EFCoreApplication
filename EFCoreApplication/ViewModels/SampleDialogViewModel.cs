using System;
using System.ComponentModel;

namespace EFCoreApplication.ViewModels
{
  public class SampleDialogViewModel : BaseViewModel
  {
    private string name;
    public string Name
    {
      get => this.name;
      set
      {
        this.name = value;
        this.OnPropertyChanged();
      }
    }
  }
}
