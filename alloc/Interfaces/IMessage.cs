namespace Interfaces
{
    public interface IMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ToJson();
        public void FromJson(string json);
    }
}