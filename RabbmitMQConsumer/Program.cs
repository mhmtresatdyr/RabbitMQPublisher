using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQMessage;
using System;
using System.Text;

namespace RabbmitMQConsumer
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
                channel.QueueDeclare(queue: "StudentSave",//Queue kuyrukta hangi isimde tutulacağı bilgisi operasyon istek zamanı gönderilebilir
                                     durable: false,// ile in-memory mi yoksa fiziksel olarak mı saklanacağı belirlenir.
                                     exclusive: false,// yalnızca bir bağlantı tarafından kullanılır ve bu bağlantı kapandığında sıra silinir - özel olarak işaretlenirse silinmez
                                     autoDelete: false, // en son bir abonelik iptal edildiğinde en az bir müşteriye sahip olan kuyruk silinir
                                     arguments: null);// isteğe bağlı; eklentiler tarafından kullanılır ve TTL mesajı, kuyruk uzunluğu sınırı, vb. 

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    StudentCommon st = JsonConvert.DeserializeObject<StudentCommon>(message); //Herhangi bir tipte gönderilebilir where koşullaırına uyan
                    Console.WriteLine($" Adı: {st.Name} Soyadı:{st.LastName} Yaş:{st.Age}");
                };
                channel.BasicConsume(queue: "StudentSave", //Queue kuyrukta hangi isimde tutulacağı bilgisi operasyon istek zamanı gönderilebilir.
                                    autoAck: false,
                                     /* autoAck: bir mesajı aldıktan sonra bunu anladığına       
                                        dair(acknowledgment) kuyruğa bildirimde bulunur ya da timeout gibi vakalar oluştuğunda 
                                        mesajı geri çevirmek(Discard) veya yeniden kuyruğa aldırmak(Re-Queue) için dönüşler yapar*/
                                     consumer: consumer);

                Console.WriteLine(" Kaydınız Yapıldı. Teşekkürler :)");
                StudentEvent studentEvent = new StudentEvent() { Result = "Başarılı" };
                string message = JsonConvert.SerializeObject(studentEvent);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "",
                                     routingKey: "StudentSaveEvent",
                                     basicProperties: null,
                                     body: body);
                Console.ReadLine();
                //try catch ile veya farklı algoritmalarınızla loglama yapılabilir
            }
        }
    }
}
