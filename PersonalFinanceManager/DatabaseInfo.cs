using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PersonalFinanceManager
{
    public class DatabaseInfo
    {
        public SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder("Data Source=(localdb)\\mssqllocaldb; Integrated Security = True");
        private readonly string table1Name = "Category";
        private readonly string table2Name = "Wallet";
        private readonly string databaseName = "PFM";

        Random rd = new Random();

        private string[] Categories = { "Food", "Household Items", "Utility Fee", "Clothes", "Cafe Fee", "Cigarette",
                "Perfume", "Electronic Goods", "Furniture","Hygiene Products","Medicine"};

        public DatabaseInfo()
        {
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            string query = $"SELECT database_id FROM sys.databases WHERE Name= '{databaseName}'";
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        builder.InitialCatalog = "PFM";

                        query = $"CREATE DATABASE {databaseName}";
                        command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        query = $@"CREATE TABLE {databaseName}.dbo.{table1Name}
                                (
                                [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                [Title] [nvarchar](MAX) NOT NULL,
                                )";
                        command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        query = $@"CREATE TABLE {databaseName}.dbo.{table2Name}

                                (
                                [Id] [int] IDENTITY PRIMARY KEY,
                                [Amount] [money] NOT NULL,
                                [Comment] [nvarchar](200) NULL,
                                [Day] [datetime2](7) NOT NULL,
                                [DateCreated] [datetime2](7) NOT NULL DEFAULT getdate(),
                                [CategoryId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE

                                )";
                        command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        AddCategories();
                        AddWallets();

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
            }
        }

        private void AddCategories()
        {
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    foreach (string item in Categories)
                    {
                        string query = $"INSERT INTO PFM.dbo.Category(Title,Id) VALUES('{item}',newid())";

                        SqlCommand command = new SqlCommand(query, connection);

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

        private void AddWallets()
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {

                try
                {
                    connection.Open();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        decimal amount = rd.Next(100_000, 1_000_000);

                        int year = rd.Next(2015, 2019);
                        int month = rd.Next(1, 13);
                        int day = rd.Next(1, 28);
                        string dayData = $"{year}-{month}-{day}"; //YYYY - MM - DD hh: mm: ss[.fractional seconds]

                        string datecreated = $"{DateTime.Now}";

                        int randomCategory = rd.Next(0, 11);

                        SqlCommand command = new SqlCommand("SELECT Id FROM PFM.dbo.Category", connection);
                        
                        SqlDataReader reader = command.ExecuteReader();
                        string categoryId = "";
                        while (reader.Read() && randomCategory >= 0)
                        {
                            randomCategory--;
                            categoryId = reader[0].ToString();
                        }
                        reader.Close();

                        string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId)" +
                        $"VALUES('{amount}','{dayData}','{datecreated}','{categoryId}')";

                        command = new SqlCommand(query, connection);
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
