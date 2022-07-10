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
using System.Data;

namespace AzureSQLFunction
{
    public static class AddProduct
    {
        [FunctionName("AddProduct")]
        public static async Task<IActionResult> RunAddProduct(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            Products product = JsonConvert.DeserializeObject<Products>(requestbody);

            SqlConnection sqlConnection = GetSqlConnection();
            sqlConnection.Open();

            string query = "Insert Into Products(ProductID,ProductName,Quantity) Values(@ProductID,@ProductName,@Quantity)";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.Add("@ProductID", System.Data.SqlDbType.Int).Value= product.ProductId;
                cmd.Parameters.Add("@ProductName", System.Data.SqlDbType.VarChar, 500).Value = product.ProductName;
                cmd.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = product.Quantity;
                cmd.CommandType =  CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sqlConnection.Close();

            return new OkObjectResult("Product Added");
        }

        private static SqlConnection GetSqlConnection()
        {
            string con = "Data Source=sqldemoserver001.database.windows.net;Initial Catalog=SQLDbConnection;Persist Security Info=False;User ID=sqladmin;Password=India@123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(con);
        }


    }
}