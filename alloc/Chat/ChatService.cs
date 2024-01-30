using Interfaces;

namespace Chat
{
    public class ChatService : IChat
    {
        public string Name => "Chat";
    
        public ILogger? Logger { get; private set; }
        public IDictionary<string, object>? Config { get; private set; }

        private IStorage _storage;

        public ChatService(IStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            _storage = storage;            
        }

        public void Start(ILogger logger, IDictionary<string, object> config)
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
            Logger?.Log($"[{Name}] Starting");
        }

        public void Stop()
        {
            Logger?.Log($"[{Name}] Stopping");
        }
        
        public void SendMessage(IMessage message)
        {            
            Logger?.Log($"[{Name}] Sending message: {message}");
            _storage.Save(message);
        }
        public List<IMessage> GetMessages()
        {
            Logger?.Log($"[{Name}] Getting messages");
            return _storage.GetMessages();
        }
    }
}