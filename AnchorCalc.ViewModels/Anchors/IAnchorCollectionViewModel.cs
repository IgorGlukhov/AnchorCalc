using AnchorCalc.Domain.Anchors;
using AnchorCalc.Domain.Rest;
using AnchorCalc.ViewModels.MainWindow;

namespace AnchorCalc.ViewModels.Anchors;

public interface IAnchorCollectionViewModel : IMainWindowContentViewModel
{
    Task InitializeAsync();
}

public class AnchorCollectionViewModel(IApiRequestExecutor apiRequestExecutor) : IAnchorCollectionViewModel
{
    public IEnumerable<AnchorCollectionItemViewModel> Items { get; private set; } = [];

    public async Task InitializeAsync()
    {
        var anchorCollectionResponse = await apiRequestExecutor.GetAsync<AnchorCollectionResponse>("anchor/all");
        Items = anchorCollectionResponse?.Items
            .Select(response => new AnchorCollectionItemViewModel(response.Id, response.Name)).ToArray() ?? [];
    }
}