using Newtonsoft.Json;
using Interfaces;

namespace Entities
{
    public class Message : IMessage
    {   
        public int Id { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Text { get; set; } = string.Empty;
        public string Author  { get; set; } = string.Empty;

        public Message()
        {
        }

        public override string ToString()
        {
            return $"[{this.CreatedAt}] {this.Author}: {this.Text}";
        }      

        public string ToJson()
        {
            return $"{{\"Author\":\"{this.Author}\",\"CreatedAt\":\"{this.CreatedAt}\",\"Text\":\"{this.Text}\"}}";
        }

        public void FromJson(string json)
        {
            var message = JsonConvert.DeserializeObject<Message>(json);
            if (message == null)
            {
                throw new System.Exception("Invalid json");
            }
            
            this.Author = message.Author;
            this.CreatedAt = message.CreatedAt;
            this.Text = message.Text;
            this.Id = message.Id;
        }            
    }
}