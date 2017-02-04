using System.Text;
using Common;
using Lykke.EmailSender.RabbitMQ;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.EmailSender.RabbitMqBroker
{
    public class EmailRabbitMqDeserializer : IMessageDeserializer<EmailModel>
    {
        public EmailModel Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            var contract = json.DeserializeJson<EmailRabbitMqContract>();

            return contract.ToDomain();
        }
    }
}
