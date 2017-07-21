using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Shared.Database
{
    public class DatabaseTransaction
    {
        private readonly Database database;
        private readonly List<(StoredPreparedStatement Statement, object[] Parameters)> queries = new List<(StoredPreparedStatement Statement, object[] Parameters)>();

        public DatabaseTransaction(Database database)
        {
            this.database = database;
        }

        public void AddPreparedStatement<T>(T id, params object[] parameters)
        {
            Debug.Assert(typeof(T) == database.PreparedStatementType);
            
            if (!database.GetPrepairedStatement(id, out StoredPreparedStatement preparedStatement))
            {
                Debug.Assert(preparedStatement != null);
                return;
            }

            Debug.Assert(preparedStatement.Types.Count == parameters.Length);
            queries.Add((preparedStatement, parameters));
        }

        public async Task Commit()
        {
            if (queries.Count == 0)
                return;

            using (MySqlConnection connection = new MySqlConnection(database.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        foreach ((StoredPreparedStatement Statement, object[] Parameters) query in queries)
                        {
                            using (MySqlCommand command = new MySqlCommand(query.Statement.Query, connection, transaction))
                            {
                                for (int i = 0; i < query.Parameters.Length; i++)
                                    command.Parameters.Add("", query.Statement.Types[i]).Value = query.Parameters[i];

                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        Console.WriteLine($"An exception occured while commiting a transaction of {queries.Count} queries, a rollback will be performed!");

                        try
                        {
                            // serious problem if rollback also fails
                            transaction?.Rollback();
                        }
                        catch
                        {
                            Console.WriteLine($"An exception occured while rolling back transaction of {queries.Count} queries!");
                            throw;
                        }

                        throw;
                    }
                    finally
                    {
                        queries.Clear();
                    }
                }
            }
                
        }
    }
}
