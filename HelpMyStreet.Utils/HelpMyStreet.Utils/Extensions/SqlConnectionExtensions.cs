using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;

namespace HelpMyStreet.Utils.Extensions
{
    public static class SqlConnectionExtensions
    {
        public static void AddAzureToken(this SqlConnection connection)
        {
            if (connection.DataSource.Contains("database.windows.net"))
            {
                connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
            }
        }
    }
}
