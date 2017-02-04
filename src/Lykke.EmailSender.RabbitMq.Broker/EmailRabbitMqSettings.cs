using Lykke.AzureQueueIntegration;
using Lykke.Logs;
using Lykke.RabbitMqBroker;

namespace Lykke.EmailSender.RabbitMqBroker
{
    public class EmailSettingsRabbitMqBroker
    {
        public RabbitMqSettings RabbitMqSettings { get; set; }
    }


    public class AppSettings : ILogToAzureSettings
    {
        public EmailSettingsRabbitMqBroker EmailSettingsRabbitMqBroker { get; set; }

        public AzureQueueSettings SlackNotificationsViaAzureQueue { get; set; }

        public string LogConnectionString { get; set; }
    }
}
