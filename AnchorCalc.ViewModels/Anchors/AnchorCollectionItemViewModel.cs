namespace AnchorCalc.ViewModels.Anchors;

public class AnchorCollectionItemViewModel(int id, string name)
{
    public string Id { get; } = id.ToString();
    public string Name { get; } = name;
}