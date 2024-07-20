namespace AnchorCalc.Domain.Anchors;

public class AnchorCollectionResponse
{
    public required IEnumerable<AnchorResponse> Items { get; init; }
}