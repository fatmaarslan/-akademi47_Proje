using Microsoft.Data.SqlClient;

namespace İakademi47_Proje.Models
{
    public class Connection
    {

        public static SqlConnection ServerConnect
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection("Server=DESKTOP-8T9FE1G;Database=iakademi47Core_Proje;trusted_connection=True;TrustServerCertificate=True;");
                return sqlConnection;
            }
        }
    }
}
