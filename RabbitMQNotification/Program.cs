

using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQMessage;
using System;
using System.Text;

namespace RabbitMQNotification
{
    static class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "StudentSaveEvent",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>//bu kısmı metodlaştırarak kullanmanızı tavsiye ederim.
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    StudentEvent se = JsonConvert.DeserializeObject<StudentEvent>(message);
                    Console.WriteLine($" Durum:{se.Result}");
                };
                channel.BasicConsume(queue: "StudentSaveEvent",//Queue kuyrukta hangi isimde tutulacağı bilgisi operasyon istek zamanı gönderilebilir
                                    autoAck: false,
                                     /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                        dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                        mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                     consumer: consumer);

                Console.WriteLine(" Bildirim alınmıştır.)");
                Console.ReadLine();
                //try catch ile veya farklı algoritmalarınızla loglama yapılabilir
            }
        }
    }
}
