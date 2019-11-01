using System.Data.SqlClient;

namespace ApexVoxApi.Infrastructure
{
    public static class DbUtils
    {
        public static void CreateDatabseIfNotExist(string connectionString)
        {
            //Get connection information from the connection string
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            //Set create db command
            var createDbCmd = string.Format("IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = N'{0}') " +
                                               "BEGIN " +
                                                   "CREATE DATABASE [{0}] " +
                                               "END;", connectionStringBuilder.InitialCatalog);

            //Set connection intial catalog as empty
            connectionStringBuilder.InitialCatalog = string.Empty;



            //Open connection and execute command
            using (var myConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                myConn.Open();
                using (SqlCommand myCommand = new SqlCommand(createDbCmd, myConn))
                {
                    myCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
