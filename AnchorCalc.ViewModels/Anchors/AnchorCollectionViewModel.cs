using AnchorCalc.Domain.DataAccess;
using System.Collections.Generic;

namespace AnchorCalc.ViewModels.Anchors;

public class AnchorCollectionViewModel(ICsvFileAccess csvFileData) : IAnchorCollectionViewModel
{
    public List<AnchorCollectionItemViewModel>? Items { get; init; } = GetData(csvFileData);

    private static List<AnchorCollectionItemViewModel> GetData(ICsvFileAccess csvFileData)
    {
        var dataTableAnchorCollection = csvFileData.Data;
        List<AnchorCollectionItemViewModel> anchorCollection = [];
        for (var i = 0; i < dataTableAnchorCollection.Rows.Count; i++)
        {
            var item = new AnchorCollectionItemViewModel
            {
                Id = dataTableAnchorCollection.Rows[i][0].ToString() ?? string.Empty,
                Name = dataTableAnchorCollection.Rows[i][1].ToString() ?? string.Empty,
                Diameter = double.Parse(dataTableAnchorCollection.Rows[i][2].ToString() ?? string.Empty),
                SealingDepth = double.Parse(dataTableAnchorCollection.Rows[i][3].ToString() ?? string.Empty),
                NormativeResistance = double.Parse(dataTableAnchorCollection.Rows[i][4].ToString() ?? string.Empty),
                UncrackedNormativeForce = double.Parse(dataTableAnchorCollection.Rows[i][5].ToString() ?? string.Empty),
                CrackedNormativeForce = double.Parse(dataTableAnchorCollection.Rows[i][6].ToString() ?? string.Empty),
                CriticInterAxialDistance = double.Parse(dataTableAnchorCollection.Rows[i][7].ToString() ?? string.Empty),
                CriticEdgeDistance = double.Parse(dataTableAnchorCollection.Rows[i][8].ToString() ?? string.Empty),
                MinBaseHeight = double.Parse(dataTableAnchorCollection.Rows[i][9].ToString() ?? string.Empty),
                PhiC = double.Parse(dataTableAnchorCollection.Rows[i][10].ToString() ?? string.Empty),
                GammaNs = double.Parse(dataTableAnchorCollection.Rows[i][11].ToString() ?? string.Empty),
                GammaNp = double.Parse(dataTableAnchorCollection.Rows[i][12].ToString() ?? string.Empty),
                GammaNc = double.Parse(dataTableAnchorCollection.Rows[i][13].ToString() ?? string.Empty),
                GammaNsp = double.Parse(dataTableAnchorCollection.Rows[i][14].ToString() ?? string.Empty)
            };
            anchorCollection.Add(item);
        }

        return anchorCollection;
    }
}