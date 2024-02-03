using Interfaces;
using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;

namespace Logger
{
    public class DBLogger : ILogger
    {
        private static string _connectionString = string.Empty;
        private static SqliteConnection? _connection = null;
        private static string _fileName = string.Empty;
        private static ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        private static bool _running = false;

        private static SqliteConnection? GetConnection()
        {
            if (_connection == null)
            {
                try
                {
                    _connection = new SqliteConnection(_connectionString);                    
                    _connection.Open();
                }
                catch (Exception)
                {
                    throw;
                }

                var sql = """
                CREATE TABLE IF NOT EXISTS logs 
                (
                    id INTEGER PRIMARY KEY, 
                    message TEXT, 
                    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
                );
                """;

                using (var command = new SqliteCommand(sql, _connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }
        public void Start(IDictionary<string, object> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _fileName = config["fileName"] as string ?? "logs.db";

            string? dir = Path.GetDirectoryName(_fileName);

            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _connectionString = $"Data Source={_fileName}";
        }

        public void Log(string message)
        {
            _messages.Enqueue(message);
            Write();
        }

        public void Log(Exception exception)
        {
            _messages.Enqueue(exception.Message);
            Write();
        }

        public void Log(string message, Exception exception)
        {
            _messages.Enqueue($"{message} {exception.Message}");
            Write();
        }

        private static void Write()
        {
            if (_running)
            {
                return;
            }

            _running = true;
            try
            {
                var sql = "INSERT INTO logs (message) VALUES ($message)";
                string? message;

                while (_messages.IsEmpty == false)
                {
                    if (_messages.TryDequeue(out message) && !string.IsNullOrEmpty(message))
                    {
                        using (var connection = GetConnection())
                        {
                            using (var command = new SqliteCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("$message", message);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _running = false;
            }
        }
    }
}