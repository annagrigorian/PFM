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
            //EnsureDatabaseCreatedAsync().Wait();
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            //SqlTransaction connectionTransaction = null;
            string query = $"SELECT database_id FROM sys.databases WHERE Name= '{databaseName}'";
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                try
                {
                    await connection.OpenAsync();
                   // connectionTransaction = connection.BeginTransaction();
                    var result = command.ExecuteScalarAsync();
                    if (result == null)
                    {
                        builder.InitialCatalog = "PFM";

                        query = $"CREATE DATABASE {databaseName}";
                        command = new SqlCommand(query, connection);
                        await command.ExecuteNonQueryAsync();

                        query = $@"CREATE TABLE {databaseName}.dbo.{table1Name}
                                (
                                [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                [Kind] [bit] NOT NULL,
                                [Title] [nvarchar](MAX) NOT NULL,
                                )";
                        command = new SqlCommand(query, connection);
                        await command.ExecuteNonQueryAsync();
                      

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
                        await command.ExecuteNonQueryAsync();

                        //await AddCategoriesAsync();
                        //await AddWalletsAsync();
                        AddCategories();
                        AddWallets();
                    

                       // connectionTransaction.Commit();
                    }
                }
                catch (Exception e)
                {
                   // connectionTransaction.Rollback();
                    MessageBox.Show(e.Message);
                    throw;
                }
                finally
                {
                    command.Dispose();
                }
            }
        }

        private void EnsureDatabaseCreated()
        {
            SqlTransaction connectionTransaction = null;
          
            string query = $"SELECT database_id FROM sys.databases WHERE Name= '{databaseName}'";
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                var command = new SqlCommand(query, connection);
                StringBuilder sql = new StringBuilder();
                try
                {
                    connection.Open();
                    command.Connection = connection;
                   
                    connectionTransaction = connection.BeginTransaction();
                    command.Transaction = connectionTransaction;

                    var result = command.ExecuteScalar();

                    if (result == null)
                    {
                        builder.InitialCatalog = "PFM";

                        sql.AppendLine($"CREATE DATABASE {databaseName}");
                        
                        sql.AppendLine($@"CREATE TABLE {databaseName}.dbo.{table1Name}
                                (
                                [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                [Kind] [bit] NOT NULL,
                                [Title] [nvarchar](MAX) NOT NULL,
                                )");
                       
                        sql.AppendLine($@"CREATE TABLE {databaseName}.dbo.{table2Name}
                                (
                                [Id] [int] IDENTITY PRIMARY KEY,
                                [Amount] [money] NOT NULL,                                
                                [Comment] [nvarchar](200) NULL,
                                [Day] [datetime2](7) NOT NULL,
                                [DateCreated] [datetime2](7) NOT NULL DEFAULT getdate(),
                                [KindOfTurnover] bit NOT NULL,
                                [CategoryId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [dbo].[Category] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE                             
                                )");
                        
                       
                        command = new SqlCommand(sql.ToString(), connection);
                        command.ExecuteNonQuery();
                        connectionTransaction.Commit();

                        AddCategories();
                        AddWallets();
                        
                        
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        connectionTransaction.Rollback();
                        MessageBox.Show(e.Message);
                        throw;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        throw;
                    } 
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
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

                    StringBuilder sql = new StringBuilder();
                    int year1 = 2018;
                    int year2 = 2019;
                    int ifspend;
                    decimal amount;
                    string datecreated = $"{DateTime.Now}";

                    for (int year = year1; year <= year2; year++)
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            sql.AppendLine($"INSERT INTO [dbo].[Wallet] ([CategoryId], [Amount], [Day],[KindOfTurnover]) VALUES('{Categories["Salary"]}', {750000}, '{year}-{month.ToString().PadLeft(2, '0')}-01',1);");

                            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                            {
                                sql.AppendLine($"INSERT INTO [dbo].[Wallet] ([CategoryId], [Amount], [Day],[KindOfTurnover]) VALUES('{Categories["Food"]}', {rd.Next(10, 50) * 100}, '{year}-{month}-{day}',0);");

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Food"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Gasoline"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Gasoline"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Utility Fee"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Household Items"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 4) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Medicine"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Clothes"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Entertainment"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Hygiene Products"]}',0)");
                                }
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
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

        private async Task AddCategoriesAsync()
        {
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query;
                    SqlCommand command;

                    foreach (var category in Categories)
                    {
                        query = $"INSERT INTO [dbo].[Category]([Id], [Title],[Kind]) VALUES('{category.Value}', '{category.Key}',0)";

                        command = new SqlCommand(query, connection);

                        await command.ExecuteNonQueryAsync();
                    }

                    query = $@"UPDATE [dbo].[Category]
                                    SET Kind = 1
                                    WHERE Title = 'Salary'";

                    command = new SqlCommand(query, connection);

                    await command.ExecuteNonQueryAsync();

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    throw;
                }
            }
        }

        private async Task AddWalletsAsync()
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    StringBuilder sql = new StringBuilder();
                    int year1 = 2018;
                    int year2 = 2019;
                    int ifspend;
                    decimal amount;
                    string datecreated = $"{DateTime.Now}";

                    for (int year = year1; year <= year2; year++)
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            sql.AppendLine($"INSERT INTO [dbo].[Wallet] ([CategoryId], [Amount], [Day],[KindOfTurnover]) VALUES('{Categories["Salary"]}', {750000}, '{year}-{month.ToString().PadLeft(2, '0')}-01',1);");

                            for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                            {
                                sql.AppendLine($"INSERT INTO [dbo].[Wallet] ([CategoryId], [Amount], [Day],[KindOfTurnover]) VALUES('{Categories["Food"]}', {rd.Next(10, 50) * 100}, '{year}-{month}-{day}',0);");

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Food"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Gasoline"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Gasoline"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Utility Fee"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 5) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Household Items"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(0, 4) * 100;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Medicine"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Clothes"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Entertainment"]}',0)");
                                }

                                ifspend = rd.Next(0, 2);
                                amount = rd.Next(1, 7) * 1000;

                                if (ifspend == 0)
                                {
                                    sql.AppendLine("INSERT INTO PFM.dbo.Wallet(Amount,Day,DateCreated,CategoryId,KindOfTurnover)" +
                                   $"VALUES('{amount}','{year}-{month}-{day}','{datecreated}','{Categories["Hygiene Products"]}',0)");
                                }
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand(sql.ToString(), connection))
                    {
                        await command.ExecuteNonQueryAsync();
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
