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
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> RunProducts(
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
        [FunctionName("GetProduct")]
        public static async Task<IActionResult> RunProduct(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
           ILogger log)
        {
            int productid = Convert.ToInt32(req.Query["productid"]);
            string query = $"SELECT ProductID,ProductName,Quantity FROM Products where ProductID={productid}";
            SqlConnection _sqlConnection = GetSqlConnection();
            _sqlConnection.Open();

            try
            {
              
                using (SqlCommand cmd = new SqlCommand(query, _sqlConnection))
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Products product = new Products();
                        product.ProductId = reader.GetInt32(0);
                        product.ProductName = reader.GetString(1);
                        product.Quantity    =  reader.GetInt32(2);

                        _sqlConnection.Close();
                        var res = product;
                        return new OkObjectResult(res);

                    }
                }


            }
            catch (Exception)
            {
                var res = "No Record Found";
                _sqlConnection.Close();
                return new OkObjectResult(res);
            }


        }

    }
}
