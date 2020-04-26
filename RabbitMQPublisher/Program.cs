using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQMessage;
using System;
using System.Text;

namespace RabbitMQPublisher
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            StudentCommon st = new StudentCommon() { Name = "Resat", LastName = "Duyar", Age = 28 };//Herhangi bir tipte gönderilebilir where koşullaırına uyan
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "StudentSave",//Queue kuyrukta hangi isimde tutulacağı bilgisi operasyon istek zamanı gönderilebilir
                                     durable: false,// ile in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir.
                                     exclusive: false,// yalnızca bir bağlantı tarafından kullanılır ve bu bağlantı kapandığında sıra silinir - özel olarak işaretlenirse silinmez
                                     autoDelete: false, // en son bir abonelik iptal edildiğinde en az bir müşteriye sahip olan kuyruk silinir
                                     arguments: null);// isteğe bağlı; eklentiler tarafından kullanılır ve TTL mesajı, kuyruk uzunluğu sınırı, vb. 

                string message = JsonConvert.SerializeObject(st);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "StudentSave",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"Gönderilen Öğrenci: {st.Name}-{st.LastName}");
            }

            Console.WriteLine("Öğrenci Gönderildi...");
            Console.ReadLine();
            //try catch ile veya farklı algoritmalarınızla loglama yapılabilir
        }
    }
}
