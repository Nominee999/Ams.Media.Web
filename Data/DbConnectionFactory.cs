using System.Data;
using Microsoft.Data.SqlClient;

namespace Ams.Media.Web.Data;

// Primary constructor (C# 12)
public sealed class DbConnectionFactory(string connStr)
{
    public IDbConnection Create() => new SqlConnection(connStr);
}
