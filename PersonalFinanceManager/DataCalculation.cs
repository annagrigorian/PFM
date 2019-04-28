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
        public static List<Summary> Calculate(DateTime start, DateTime end)
        {
           return SortData(start, end);
        }

        private static List<Summary> SortData(DateTime start, DateTime end)
        {
            string connectionString = "Data Source=(localdb)\\mssqllocaldb;" +"Initial Catalog=PFM;" +"Integrated Security=true;";

            string query = $@"SELECT Day, SUM(CASE WHEN KindOfTurnover = 1 THEN  [Amount] ELSE -[Amount] END) AS Total
                        FROM PFM.dbo.Wallet                       
                        WHERE [Day] BETWEEN '{String.Format("{0:yyyy-MM-dd}", start)}' AND '{String.Format("{0:yyyy-MM-dd}",end)}'
                        GROUP BY [Day] 
                        ORDER BY [Day]";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                List<Summary> summaries = new List<Summary>();

                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Summary summary = new Summary
                            {
                                Day = (DateTime)reader["Day"],
                                Total = (decimal)reader["Total"]
                            };
                            summaries.Add(summary);
                        }
                    }
                    //for (int i = 1; i < summaries.Count; i++)
                    //{
                    //    summaries[i].Total = summaries[i - 1].Total + summaries[i].Total;
                    //}
                    return summaries;
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
