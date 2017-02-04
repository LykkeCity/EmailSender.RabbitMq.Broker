using System;
using Common;
using Lykke.RabbitMqBroker.Subscriber;
using Flurl.Http;
using Lykke.EmailSender.Smtp;
using Lykke.Logs;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.EmailSender.RabbitMqBroker
{
    public class Program
    {

        private static IEmailSender _emailSender;
        public static void Main(string[] args)
        {

            var emailSenderToSmtpSettings = new EmailSenderToSmtpSettings
            {
                DisplayName = Environment.GetEnvironmentVariable("DisplayName"),
                From = Environment.GetEnvironmentVariable("From"),
                Host = Environment.GetEnvironmentVariable("Host"),
                Port = int.Parse(Environment.GetEnvironmentVariable("Port")),
                LocalDomain = Environment.GetEnvironmentVariable("LocalDomain"),
                Login = Environment.GetEnvironmentVariable("Login"),
                Password = Environment.GetEnvironmentVariable("Password")
            };

            _emailSender = new EmailSenderToSmtp(emailSenderToSmtpSettings);

            var settingsUrl = Environment.GetEnvironmentVariable("SettingsUrl");

            if (string.IsNullOrEmpty(settingsUrl))
            {
                Console.WriteLine("Please specify SettingsUrl");
                return;
            }


            var settings = settingsUrl
                .GetStringAsync()
                .Result
                .DeserializeJson<AppSettings>();

            var ioc = new ServiceCollection();

            var logs = ioc.UseLogToAzureStorage(settings);

            ioc.UseSlackNotificationsSenderViaAzureQueue(settings.SlackNotificationsViaAzureQueue);

            var rabbitMqReader = new RabbitMqSubscriber<EmailModel>(settings.EmailSettingsRabbitMqBroker.RabbitMqSettings)
                .SetLogger(logs)
                .SetMessageDeserializer(new EmailRabbitMqDeserializer())
                .Subscribe(model =>
                {
                    Console.WriteLine($"Handling email for {model.Email}. Subject: {model.Subject}");
                    return _emailSender.SendEmailAsync(model);
                })
                .Start();


            Console.WriteLine("Started");
            Console.ReadLine();

            Console.WriteLine("Stopping");
            rabbitMqReader.Stop();

        }

    }
}

