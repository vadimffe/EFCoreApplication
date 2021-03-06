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

    private string errorText;
    public string ErrorText
    {
      get => this.errorText;
      set
      {
        this.errorText = value;
        this.OnPropertyChanged();
      }
    }
  }
}
