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

        Dictionary<string, Guid> Categories = new Dictionary<string, Guid>
            {
                { "Salary", Guid.NewGuid() },
                { "Credit", Guid.NewGuid() },
                { "Gasoline", Guid.NewGuid() },
                { "Entertainment", Guid.NewGuid() },
                { "Food", Guid.NewGuid() },
                { "Household Items", Guid.NewGuid() },
                { "Utility Fee", Guid.NewGuid() },
                { "Clothes", Guid.NewGuid() },
                { "Cafe Fee", Guid.NewGuid() },
                { "Perfume", Guid.NewGuid() },
                { "Electronic Goods", Guid.NewGuid() },
                { "Hygiene Products", Guid.NewGuid() },
                { "Medicine", Guid.NewGuid() },
            };

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
                                [Kind] [bit] NOT NULL,
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
                                [KindOfTurnover] bit NOT NULL,
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

                    string query;
                    SqlCommand command;

                    foreach (var category in Categories)
                    {
                        query = $"INSERT INTO [dbo].[Category]([Id], [Title],[Kind]) VALUES('{category.Value}', '{category.Key}',0)";

                        command = new SqlCommand(query, connection);

                        command.ExecuteNonQuery();
                    }

                    query = $@"UPDATE [dbo].[Category]
                                    SET Kind = 1
                                    WHERE Title = 'Salary'";

                    command = new SqlCommand(query, connection);

                    command.ExecuteNonQuery();

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

                    string sql = "";
                    int year1 = 2018;
                    int year2 = 2019;

                    for (int month = 1; month <= 12; month++)
                    {
                        sql = $"INSERT INTO PFM.[dbo].[Wallet] ([KindOfTurnover], [Amount], [Day],[CategoryId]) VALUES(1, {750000}, '{year1}-{month.ToString().PadLeft(2, '0')}-01','{Categories["Salary"]}')";
                        sql = $"INSERT INTO PFM.[dbo].[Wallet] ([KindOfTurnover], [Amount], [Day],[CategoryId]) VALUES(1, {750000}, '{year2}-{month.ToString().PadLeft(2, '0')}-01','{Categories["Salary"]}')";
                    }

                    using (SqlCommand command = new SqlCommand(sql,connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    for (int i = 0; i < 10_000; i++)
                    {
                        decimal amount = rd.Next(0, 5_000);

                        int year = rd.Next(2018, 2020);
                        int month = rd.Next(1, 13);
                        int day = rd.Next(2, 28);
                        string dayData = $"{year}-{month}-{day}"; //YYYY - MM - DD hh: mm: ss[.fractional seconds]

                        string datecreated = $"{DateTime.Now}";

                        SqlCommand command;

                        int ifspend = rd.Next(0, 2);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Gasoline"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(1_000, 10_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Food"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(1_000, 2_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Utility Fee"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(0, 2_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Household Items"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(0, 800);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Medicine"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(0, 20_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Clothes"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(0, 20_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Entertainment"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }

                        ifspend = rd.Next(0, 2);
                        amount = rd.Next(0, 1_000);

                        if (ifspend == 0)
                        {
                            string query = "INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                            $"VALUES('{amount}','{dayData}','{datecreated}','{Categories["Hygiene Products"]}',0)";

                            command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                        }
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
