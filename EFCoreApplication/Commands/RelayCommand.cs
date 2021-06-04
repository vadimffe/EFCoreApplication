#region Info

// 2020/12/31  03:04
// ProductivityTracker.ViewModels

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Windows.Input;

#endregion

namespace EFCoreApplication.Commands
{
  public class RelayCommand : ICommand
  {
    #region

    public RelayCommand(Action<object> execute)
      : this(execute, null)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    #endregion

    #region

    [DebuggerStepThrough]
    public bool CanExecute(object parameter) => _canExecute == null ? true : _canExecute(parameter);

    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
        InternalCanExecuteChanged += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
        InternalCanExecuteChanged -= value;
      }
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    #endregion

    public void InvalidateCommand() => InternalCanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private event EventHandler InternalCanExecuteChanged;

    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;
  }
}