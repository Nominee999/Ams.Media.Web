using System.Data;
using Microsoft.Data.SqlClient;

namespace Ams.Media.Web.Data;

public sealed class HybridDbScope : IAsyncDisposable
{
    private readonly string _connStr;
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; }

    private HybridDbScope(string connStr, bool beginTx)
    {
        _connStr = connStr;
        var conn = new SqlConnection(_connStr);
        conn.Open();
        Connection = conn;
        if (beginTx) Transaction = conn.BeginTransaction();
    }

    public static HybridDbScope Create(string connectionString, bool beginTransaction = false)
        => new HybridDbScope(connectionString, beginTransaction);

    public ValueTask DisposeAsync()
    {
        try { Transaction?.Dispose(); } catch { }
        try { Connection?.Dispose(); } catch { }
        return ValueTask.CompletedTask;
    }
}
