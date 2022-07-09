using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace AzureSQLFunction
{
    public static class GetProduct
    {
        [FunctionName("GetProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            List<Products> products = new List<Products>();
            string Query = "SELECT ProductID,ProductName,Quantity FROM Products";
            SqlConnection _sqlConnection = GetSqlConnection();
            _sqlConnection.Open();

            using (SqlCommand cmd = new SqlCommand(Query, _sqlConnection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Products product = new Products()
                        {
                            ProductId = reader.GetInt32(0),
                            ProductName= reader.GetString(1),
                            Quantity= reader.GetInt32(2)

                        };
                        products.Add(product);
                    }
                }

            }
            _sqlConnection.Close();

            return new OkObjectResult(products);
        }


        private static SqlConnection GetSqlConnection()
        {
            string con = "Data Source=sqldemoserver001.database.windows.net;Initial Catalog=SQLDbConnection;Persist Security Info=False;User ID=sqladmin;Password=India@123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(con);
        }
    }
}
