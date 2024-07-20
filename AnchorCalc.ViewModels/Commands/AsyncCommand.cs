using System.Windows.Input;

namespace AnchorCalc.ViewModels.Commands;

internal class AsyncCommand(Func<Task> execute) : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await execute.Invoke();
    }

    public event EventHandler? CanExecuteChanged;
}