using Microsoft.Data.Sqlite;
using Interfaces;
using Entities;

namespace Storage
{
    public class DBStorage : IStorage
    {
        public string Name => "DBStorage";

        public ILogger? Logger { get; private set; }
        public IDictionary<string, object>? Config { get; private set; }
        
        private static string _connectionString = string.Empty;
        private static string _fileName = string.Empty;
        private static SqliteConnection? _connection = null;

        private static SqliteConnection? GetConnection()
        {
            if (_connection == null)
            {
                try
                {
                    _connectionString ??= "Data Source=messages.db";

                    _connection = new SqliteConnection(_connectionString);
                    _connection.Open();
                }
                catch (Exception)
                {
                    throw;
                }

                var sql = "CREATE TABLE IF NOT EXISTS messages (id INTEGER PRIMARY KEY, message TEXT, created_at DATETIME DEFAULT CURRENT_TIMESTAMP, author TEXT)";
                using(var command = new SqliteCommand(sql, _connection))
                {
                    command.ExecuteNonQuery();                
                }
            } 

            return _connection;
        }

        public void Start(ILogger logger, IDictionary<string,object> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _fileName = config["fileName"] as string ?? "messages.db";

            string? dir = Path.GetDirectoryName(_fileName);

            if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _connectionString = $"Data Source={_fileName}";
            Config = config;
            Logger = logger;     

            Logger?.Log($"[{Name}] Starting");       
        }

        public void Stop()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }

            Logger?.Log($"[{Name}] Stopping");
        }

        public void Save(IMessage message)
        {            
            try
            {
                Logger?.Log($"[{Name}] Saving message to DB");            
                var sql = "INSERT INTO messages (message, author) VALUES (@message, @author)";
                var command = new SqliteCommand(sql, GetConnection());
                command.Parameters.AddWithValue("@message", message.Text);
                command.Parameters.AddWithValue("@author", message.Author);
                command.ExecuteNonQuery();            
            }
            catch (Exception ex)
            {
                Logger?.Log(ex.Message);
                throw;
            }            
        }

        public List<IMessage> GetMessages()
        {
            var messages = new List<IMessage>();
            try
            {
                Logger?.Log($"[{Name}]Getting messages from DB");
                var sql = "SELECT * FROM messages";
                using(var command = new SqliteCommand(sql, GetConnection()))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = new Message
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                CreatedAt = reader.GetDateTime(2),
                                Author = reader.GetString(3)
                            };
                            messages.Add(message);

                            Logger?.Log($"[{Name}] Read message: {message}");
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Logger?.Log(ex.Message);
                throw;
            }

            return messages;
        }
    }
}