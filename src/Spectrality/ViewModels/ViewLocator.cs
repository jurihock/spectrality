using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Spectrality.ViewModels;

public sealed class ViewLocator : IDataTemplate
{
  public Control? Build(object? dataContext)
  {
    if (dataContext is null)
    {
      return null;
    }

    var viewModelType = dataContext.GetType();
    var viewModelName = viewModelType.FullName!;

    var viewName = viewModelName.Replace("ViewModel", "View", StringComparison.Ordinal);
    var viewType = Type.GetType(viewName);

    if (viewType is null)
    {
      return new TextBlock { Text = $"View {viewName} not found!" };
    }

    var viewControl = Activator.CreateInstance(viewType) as Control;

    if (viewControl is null)
    {
      return new TextBlock { Text = $"View {viewName} is not a control!" };
    }

    viewControl.DataContext = dataContext;

    return viewControl;
  }

  public bool Match(object? dataContext)
  {
    return dataContext is ViewModelBase;
  }
}
