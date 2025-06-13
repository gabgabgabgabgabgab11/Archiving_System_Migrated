using MySql.Data.MySqlClient;

namespace Archiving_System_Migrated
{
    public class Database
    {
        private string connectionString = "Server=localhost;Database=academic_archive;Uid=root;Pwd=;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}