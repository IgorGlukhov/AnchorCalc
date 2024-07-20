using AnchorCalc.Domain.Collections;

namespace AnchorCalc.ViewModels.DevTools;

public interface ILogEntryViewModelRepository
{
    IRotatableReadonlyCollection<LogEntryViewModel> Items { get; }
    void Clear();
}