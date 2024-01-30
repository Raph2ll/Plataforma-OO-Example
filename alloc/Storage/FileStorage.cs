using Entities;
using Interfaces;

namespace Storage
{
    public class FileStorage : IStorage
    {
        public string Name => "FileStorage";

        public ILogger? Logger { get; private set; }
        public IDictionary<string, object>? Config { get; private set; }


        private string _path = string.Empty;
        private string _fileName = string.Empty;

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

            Logger = logger;
            Config = config;

            _path = (string)config["path"];
            _fileName = (string)config["fileName"];                    

            Logger?.Log($"[{Name}] Starting with path: {_path} and fileName: {_fileName}");
        }

        public void Stop()
        {
            Logger?.Log($"[{Name}] Stopping");
        }   

        public void Save(IMessage message)
        {
            Logger?.Log($"[{Name}] Saving message: {message.ToJson()}");

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            using(var writer = new StreamWriter(Path.Combine(_path, _fileName), true))
            {
                writer.WriteLine(message.ToJson());
            }
        }

        public List<IMessage> GetMessages()
        {
            var file = Path.Combine(_path, _fileName);
            Logger?.Log($"[{Name}]Getting messages from file [{file}]");
            var messages = new List<IMessage>();
            string? line;

            using(var reader = new StreamReader(file))
            {
                line = reader.ReadLine();
                while(!string.IsNullOrEmpty(line))
                {
                    Logger?.Log($"[{Name}] Parsing message: {line}");
                    var message = new Message();
                    message.FromJson(line);
                    messages.Add(message);
                    line = reader.ReadLine();
                }
            }
            return messages;
        }
    }
}