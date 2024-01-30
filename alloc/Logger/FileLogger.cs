using Interfaces;
using System.Collections.Concurrent;
using System.IO;

namespace Logger
{
    public class FileLogger : ILogger
    {
        private static string _path = "logs/log.txt";
        private static ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private static bool _running = false;

        public void Start(IDictionary<string,object> config)
        {
            if (config != null)
            {
                if(config.TryGetValue("path", out var path))
                {
                    if (path != null)
                        _path = $"{path}";
                }
            }
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
                string? dir = Path.GetDirectoryName(_path);

                if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var file = new StreamWriter(_path, true))
                {
                    string? message;
                    while (_messages.IsEmpty == false)
                    {                        
                        if (_messages.TryDequeue(out message) && !string.IsNullOrEmpty(message))
                        {
                            file.WriteLine($"{DateTime.Now} - {message}");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _running = false;
            }            
        }
    }
}