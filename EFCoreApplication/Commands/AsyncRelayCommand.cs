using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EFCoreApplication.Commands
{
  public class AsyncRelayCommand : ICommand
  {
    private readonly Func<object, Task> execute;
    private readonly Func<object, bool> canExecute;

    private long isExecuting;

    public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
    {
      this.execute = execute;
      this.canExecute = canExecute ?? (o => true);
    }

    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public void RaiseCanExecuteChanged()
    {
      CommandManager.InvalidateRequerySuggested();
    }

    public bool CanExecute(object parameter)
    {
      if (Interlocked.Read(ref this.isExecuting) != 0)
        return false;

      return this.canExecute(parameter);
    }

    public async void Execute(object parameter)
    {
      Interlocked.Exchange(ref this.isExecuting, 1);
      this.RaiseCanExecuteChanged();

      try
      {
        await this.execute(parameter);
      }
      finally
      {
        Interlocked.Exchange(ref this.isExecuting, 0);
        this.RaiseCanExecuteChanged();
      }
    }
  }
}
