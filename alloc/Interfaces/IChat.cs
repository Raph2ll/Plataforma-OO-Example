namespace Interfaces
{
    public interface IChat: IService
    {
        public void SendMessage(IMessage message);
        public List<IMessage> GetMessages();        
    }
}