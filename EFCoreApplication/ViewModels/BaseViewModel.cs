#region Info

// 2020/12/31  03:04
// ProductivityTracker.ViewModels

#endregion

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

#endregion

namespace EFCoreApplication.ViewModels
{
  public abstract class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
  {
    #region

    public BaseViewModel()
    {
      Errors = new Dictionary<string, List<string>>();
    }

    #endregion

    #region

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    // Returns all errors of a property. If the argument is 'null' instead of the property's name,
    // then the method will return all errors of all properties.
    public IEnumerable GetErrors(string propertyName)
      => string.IsNullOrWhiteSpace(propertyName)
        ? Errors.SelectMany(entry => entry.Value)
        : Errors.TryGetValue(propertyName, out List<string> errors)
          ? errors
          : new List<string>();

    // Returns if the view model has any invalid property
    public bool HasErrors => Errors.Any();


    /// <summary>
    ///   The event that is fired when any child property changes its value
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    protected void SetValue<TValue>(
      TValue value,
      ref TValue backingField,
      [CallerMemberName] string propertyName = default)
    {
      if (ReferenceEquals(value, backingField))
      {
        return;
      }

      backingField = value;
      OnPropertyChanged(propertyName);
    }

    protected bool TrySetValue<TValue>(
      TValue value,
      ref TValue backingField,
      [CallerMemberName] string propertyName = default)
    {
      if (ReferenceEquals(value, backingField))
      {
        return false;
      }

      backingField = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    /// <summary>
    ///   Call this to fire a <see \ cref="PropertyChanged" /> event
    /// </summary>
    /// <param \ name=\"name\"></param>
    // public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Adds the specified error to the errors collection if it is not
    // already present, inserting it in the first position if 'isWarning' is
    // false. Raises the ErrorsChanged event if the Errors collection changes.
    // A property can have multiple errors.
    private void AddError(string propertyName, string errorMessage, bool isWarning = false)
    {
      if (!Errors.TryGetValue(propertyName, out List<string> propertyErrors))
      {
        propertyErrors = new List<string>();
        Errors[propertyName] = propertyErrors;
      }

      if (!propertyErrors.Contains(errorMessage))
      {
        if (isWarning)
        {
          // Move warnings to the end
          propertyErrors.Add(errorMessage);
        }
        else
        {
          propertyErrors.Insert(0, errorMessage);
        }

        OnErrorsChanged(propertyName);
      }
    }

    public bool PropertyHasErrors(string propertyName) =>
      Errors.TryGetValue(propertyName, out List<string> propertyErrors) && propertyErrors.Any();

    protected virtual void OnErrorsChanged(string propertyName)
    {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    ///   A global lock for property checks so prevent locking on different instances of expressions.
    ///   Considering how fast this check will always be it isn't an issue to globally lock all callers.
    /// </summary>
    protected object mPropertyValueCheckLock = new object();

    // Maps a property name to a list of errors that belong to this property
    private Dictionary<string, List<string>> Errors { get; }
  }
}