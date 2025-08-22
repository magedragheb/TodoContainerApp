using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace Api.DbInterceptor;

public class AzureSqlAuthInterceptor : DbConnectionInterceptor
{
    private readonly TokenCredential _credential = new DefaultAzureCredential();
    private readonly TokenRequestContext _context =
        new TokenRequestContext(new[] { "https://database.windows.net/.default" });

    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        if (connection is SqlConnection sqlConnection)
        {
            var token = _credential.GetToken(_context, default);
            sqlConnection.AccessToken = token.Token;
        }

        return base.ConnectionOpening(connection, eventData, result);
    }

}