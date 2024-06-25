using System.Windows.Input;

namespace AnchorCalc.ViewModels.Commands;

public class Command : ICommand
{
    private readonly Action _execute;

    public Command(Action execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _execute.Invoke();
    }

    public event EventHandler? CanExecuteChanged;

    protected void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}