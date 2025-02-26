using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            // await channel.QueueDeclareAsync("hello-queue", true, false, false);

            var consumer = new AsyncEventingBasicConsumer(channel);
            await channel.BasicConsumeAsync("hello-queue", true, consumer);

            consumer.ReceivedAsync += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Message received: {message}");
                
                return Task.CompletedTask;
            };

            Console.ReadLine();
        }
        catch (BrokerUnreachableException ex)
        {
            Console.WriteLine($"Bağlantı hatası: {ex.Message}");
        }
    }
}