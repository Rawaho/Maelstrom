using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Shared.Database
{
    public abstract class Database
    {
        public string ConnectionString { get; private set; }
        public abstract Type PreparedStatementType { get; }

        private readonly Dictionary<uint, StoredPreparedStatement> preparedStatements = new Dictionary<uint, StoredPreparedStatement>();

        public void Initialise(string host, uint port, string user, string password, string database)
        {
            var connectionBuilder = new MySqlConnectionStringBuilder
            {
                Server        = host,
                Port          = port,
                UserID        = user,
                Password      = password,
                Database      = database,
                IgnorePrepare = false,
                Pooling       = true
            };

            ConnectionString = connectionBuilder.ToString();

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                    connection.Open();

                Console.WriteLine($"Successfully connected to {database} database.");
            }
            catch
            {
                Console.WriteLine($"Failed to connect to {database} database!");
                throw;
            }

            InitialisePreparedStatements();
        }

        protected virtual void InitialisePreparedStatements() { }

        public virtual DatabaseTransaction BeginTransaction() { return null; }

        protected void AddPreparedStatement<T>(T id, string query, params MySqlDbType[] types)
        {
            Debug.Assert(typeof(T) == PreparedStatementType);
            Debug.Assert(types.Length == query.Count(c => c == '?'));
            Debug.Assert(query.Length != 0);

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        foreach (MySqlDbType type in types)
                            command.Parameters.Add("", type);

                        connection.Open();
                        command.Prepare();

                        uint uintId = Convert.ToUInt32(id);
                        preparedStatements.Add(uintId, new StoredPreparedStatement(uintId, query, types));
                    }
                }
            }
            catch
            {
                Console.WriteLine($"An exception occured while preparing statement {id}!");
                throw;
            }
        }

        public bool GetPrepairedStatement<T>(T id, out StoredPreparedStatement preparedStatement)
        {
            return preparedStatements.TryGetValue(Convert.ToUInt32(id), out preparedStatement);
        }

        protected int ExecutePreparedStatement<T>(T id, params object[] parameters)
        {
            Debug.Assert(typeof(T) == PreparedStatementType);

            if (!preparedStatements.TryGetValue(Convert.ToUInt32(id), out StoredPreparedStatement preparedStatement))
            {
                Debug.Assert(preparedStatement != null);
                return 0;
            }

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    using (var command = new MySqlCommand(preparedStatement.Query, connection))
                    {
                        for (int i = 0; i < preparedStatement.Types.Count; i++)
                            command.Parameters.Add("", preparedStatement.Types[i]).Value = parameters[i];

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                Console.WriteLine($"An exception occured while executing prepared statement {id}!");
                throw;
            }
        }

        protected async Task<int> ExecutePreparedStatementAsync<T>(T id, params object[] parameters)
        {
            Debug.Assert(typeof(T) == PreparedStatementType);

            if (!preparedStatements.TryGetValue(Convert.ToUInt32(id), out StoredPreparedStatement preparedStatement))
            {
                Debug.Assert(preparedStatement != null);
                return 0;
            }

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    using (var command = new MySqlCommand(preparedStatement.Query, connection))
                    {
                        for (int i = 0; i < preparedStatement.Types.Count; i++)
                            command.Parameters.Add("", preparedStatement.Types[i]).Value = parameters[i];

                        await connection.OpenAsync();
                        return await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch
            {
                Console.WriteLine($"An exception occured while executing prepared statement {id}!");
                throw;
            }
        }

        protected MySqlResult SelectPreparedStatement<T>(T id, params object[] parameters)
        {
            Debug.Assert(typeof(T) == PreparedStatementType);

            if (!preparedStatements.TryGetValue(Convert.ToUInt32(id), out StoredPreparedStatement preparedStatement))
            {
                Debug.Assert(preparedStatement != null);
                return null;
            }

            Debug.Assert(parameters.Length == preparedStatement.Types.Count);

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    using (var command = new MySqlCommand(preparedStatement.Query, connection))
                    {
                        for (int i = 0; i < preparedStatement.Types.Count; i++)
                            command.Parameters.Add("", preparedStatement.Types[i]).Value = parameters[i];

                        connection.Open();
                        using (var commandReader = command.ExecuteReader(CommandBehavior.Default))
                        {
                            using (var result = new MySqlResult())
                            {
                                result.Load(commandReader);
                                result.Count = (uint)result.Rows.Count;
                                return result;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"An exception occured while selecting prepared statement {id}!");
                throw;
            }
        }

        protected async Task<MySqlResult> SelectPreparedStatementAsync<T>(T id, params object[] parameters)
        {
            Debug.Assert(typeof(T) == PreparedStatementType);

            if (!preparedStatements.TryGetValue(Convert.ToUInt32(id), out StoredPreparedStatement preparedStatement))
            {
                Debug.Assert(preparedStatement != null);
                return null;
            }

            Debug.Assert(parameters.Length == preparedStatement.Types.Count);

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    using (var command = new MySqlCommand(preparedStatement.Query, connection))
                    {
                        for (int i = 0; i < preparedStatement.Types.Count; i++)
                            command.Parameters.Add("", preparedStatement.Types[i]).Value = parameters[i];

                        await connection.OpenAsync();
                        using (var commandReader = (MySqlDataReader)await command.ExecuteReaderAsync(CommandBehavior.Default))
                        {
                            using (var result = new MySqlResult())
                            {
                                result.Load(commandReader);
                                result.Count = (uint)result.Rows.Count;
                                return result;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"An exception occured while selecting prepared statement {id}!");
                throw;
            }
        }
    }
}
