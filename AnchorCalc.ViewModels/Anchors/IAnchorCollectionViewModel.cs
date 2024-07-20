using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.ViewModels.Anchors;

public interface IAnchorCollectionViewModel : IMainWindowContentViewModel
{
    List<AnchorCollectionItemViewModel>? Items { get; }
}