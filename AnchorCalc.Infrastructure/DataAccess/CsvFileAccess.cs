using AnchorCalc.Domain.DataAccess;
using System.Data;

namespace AnchorCalc.Infrastructure.DataAccess;

public class CsvFileAccess : ICsvFileAccess
{
    private readonly string _baseAddress = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory())?.FullName ?? string.Empty, "AnchorData.csv");

    public CsvFileAccess()
    {
        Data = ConvertCsvToDataTable();
    }

    public DataTable Data { get; }

    public DataTable ConvertCsvToDataTable()
    {
        var dt = new DataTable();
        using var sr = new StreamReader(_baseAddress);
        var headers = sr.ReadLine()?.Split(';');
        if (headers == null) return dt;
        foreach (var header in headers) dt.Columns.Add(header);

        while (!sr.EndOfStream)
        {
            var rows = sr.ReadLine()?.Split(';');
            var dr = dt.NewRow();
            for (var i = 0; i < headers.Length; i++) dr[i] = rows?[i];

            dt.Rows.Add(dr);
        }

        return dt;
    }
}