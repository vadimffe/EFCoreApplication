using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreApplication.ViewModels
{
  public class NotificationViewModel : BaseViewModel
  {
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
