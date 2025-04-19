
using System;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
namespace MovieRecommendations_DataAccess
{
    static class clsDataAccessSettings
    {
        public static string ConnectionString = "Server=localhost;Database=MovieRecommendations;User Id=sa;Password=123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

    }
}
