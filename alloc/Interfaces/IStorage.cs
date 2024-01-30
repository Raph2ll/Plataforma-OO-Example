namespace Interfaces
{
    public interface IStorage: IService
    {
        public void Save(IMessage message);
        public List<IMessage> GetMessages();        
    }
}