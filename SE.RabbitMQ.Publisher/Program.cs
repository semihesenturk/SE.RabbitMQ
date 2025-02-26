using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672"),
            UserName = "guest",
            Password = "guest"
        };

        try
        {
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("hello-queue", true, false, false);

            const string message = "hello world";
            var messageBody = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(string.Empty, "hello-queue", false, messageBody);
            Console.WriteLine(" [x] Sent {0}", message);
            Console.ReadLine();
        }
        catch (BrokerUnreachableException ex)
        {
            Console.WriteLine($"Bağlantı hatası: {ex.Message}");
        }
    }
}