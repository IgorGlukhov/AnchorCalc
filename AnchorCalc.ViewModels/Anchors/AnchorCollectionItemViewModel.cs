namespace AnchorCalc.ViewModels.Anchors;

public class AnchorCollectionItemViewModel()
{
    public required double CrackedNormativeForce { get; init; }
    public required double CriticEdgeDistance { get; init; }
    public required double CriticInterAxialDistance { get; init; }
    public required double GammaNc { get; init; }
    public required double GammaNp { get; init; }
    public required double GammaNs { get; init; }
    public required double GammaNsp { get; init; }
    public required double MinBaseHeight { get; init; }
    public required double NormativeResistance { get; init; }
    public required double PhiC { get; init; }
    public required double UncrackedNormativeForce { get; init; }
    public required double SealingDepth { get; init; }
    public required double Diameter { get; init; }
    public required string Id { get; init; }
    public required string Name { get; init; }
}