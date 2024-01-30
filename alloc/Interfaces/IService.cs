namespace Interfaces
{
    public interface IService
    {
        public void Start(ILogger logger, IDictionary<string,object> config);
        public void Stop();        

        public string Name { get; }

        public ILogger? Logger { get; }

        public IDictionary<string,object>? Config { get; }
    }
}