using System.Data;

namespace AnchorCalc.Domain.DataAccess;

public interface ICsvFileAccess
{
    DataTable Data { get; }
}