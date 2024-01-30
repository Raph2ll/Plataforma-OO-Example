using Interfaces;

namespace Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Start(IDictionary<string,object> config)
        {
            Console.WriteLine($"{DateTime.Now} - ConsoleLogger started");            
        }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message}");
        }

        public void Log(Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} - {exception.Message}");
        }

        public void Log(string message, Exception exception)
        {
            Console.WriteLine($"{DateTime.Now} - {message} {exception.Message}");
        }
    }
}