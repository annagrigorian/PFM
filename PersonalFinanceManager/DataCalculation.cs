using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace PersonalFinanceManager
{
    public class DataCalculation
    {
        public static void Calculate(DateTime start, DateTime end)
        {
            SortData(start, end);

        }

        private static void SortData(DateTime start, DateTime end)
        {
            string connectionString = "Data Source=(localdb)\\mssqllocaldb;" +"Initial Catalog=PFM;" +"Integrated Security=true;";
            string query = $@"SELECT Day FROM PFM.dbo.Wallet
                WHERE [Day] BETWEEN '{String.Format("{0:yyyy-MM-dd}", start)}' AND '{String.Format("{0:yyyy-MM-dd}",end)}'
                --GROUP BY [Day] 
                ORDER BY [Day]";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    throw;
                }              
            }
        }
    }
}
