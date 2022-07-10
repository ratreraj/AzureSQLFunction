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
    public static class DeleteProduct
    {
        [FunctionName("DeleteProduct")]
        public static async Task<IActionResult> RunDeleteProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string request = await new StreamReader(req.Body).ReadToEndAsync();
            Products product = JsonConvert.DeserializeObject<Products>(request);

            SqlConnection sqlConnection = GetSqlConnection();
            sqlConnection.Open();

            string query = "Delete from Products where ProductID=@ProductID ";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.Add("@ProductID", System.Data.SqlDbType.Int).Value= product.ProductId;
              
                cmd.CommandType =  CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            sqlConnection.Close();

            return new OkObjectResult("Item is delete from product");
        }


        private static SqlConnection GetSqlConnection()
        {
            string con = "Data Source=sqldemoserver001.database.windows.net;Initial Catalog=SQLDbConnection;Persist Security Info=False;User ID=sqladmin;Password=India@123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(con);
        }
    }
}
