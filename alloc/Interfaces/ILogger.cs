namespace Interfaces
{
    public interface ILogger
    {
        public void Start(IDictionary<string,object> config);
        public void Log(string message);
        public void Log(Exception exception);
        public void Log(string message, Exception exception);
    }
}