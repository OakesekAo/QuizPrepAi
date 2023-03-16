using Npgsql;

namespace QuizPrepAi.Helpers
{
    public class ConnectionHelper
    {
        public static string GetConnectionString(IConfiguration configuration)
        {

            var connectionString = configuration.GetSection("pgSettings")["pgConnection"];
            //Url will be empty on local dev, production it will find url heroku and railway use DATABASE_URL, we may have somethign different
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            var returnString = string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);

            return string.IsNullOrEmpty(returnString) ? string.Empty : returnString;
        }

        //build connection string from the environment (from railway/heroku/etc)
        private static string BuildConnectionString(string databaseUrl)
        {
            //break the URL down to assign
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }
    }
}
