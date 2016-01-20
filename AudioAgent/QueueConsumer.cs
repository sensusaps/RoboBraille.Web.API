using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AudioAgent
{
    public class QueueConsumer
    {
        private static string topicsDir = @"C:\AudioAgent\topics.config";
        public void Consume(string queueName)
        {
            try
            {
                string[] routingKeys = File.ReadAllLines(topicsDir);
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "audio_job", type: "topic");
                    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(0, 1, false);
                    //var queueName = channel.QueueDeclare().QueueName; //not good because it randomly generates queues
                    if (routingKeys.Length < 1)
                    {
                        Console.Error.WriteLine("Error no routing key set");
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                        Environment.ExitCode = 1;
                        return;
                    }

                    foreach (var bindingKey in routingKeys)
                    {
                        channel.QueueBind(queue: queueName,
                                          exchange: "audio_job",
                                          routingKey: bindingKey);
                    }

                    Console.WriteLine(" [*] QUEUE NAME: " + queueName);
                    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");
                   // while (true)
                   // {
                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            //var message = Encoding.UTF8.GetString(body);
                            var routingKey = ea.RoutingKey;
                            var props = ea.BasicProperties;
                            var tag = ea.DeliveryTag;
                            Console.WriteLine(" [x] Received '{0}':'{1}'",
                                              routingKey,
                                              "Audio Job");
                            object inputPropriety, inputLanguage, voiceSpeed, format;
                            props.Headers.TryGetValue("inputPropriety", out inputPropriety);
                            props.Headers.TryGetValue("inputLanguage", out inputLanguage);
                            props.Headers.TryGetValue("voiceSpeed", out voiceSpeed);
                            props.Headers.TryGetValue("format", out format);

                            //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                            AudioJobProcessor ajp = new AudioJobProcessor();
                            byte[] response = ajp.SubmitWorkItem(body, Encoding.UTF8.GetString((byte[])inputPropriety), (int)inputLanguage, (int)voiceSpeed, (int)format);
                            bool worked = true;
                            if (response == null)
                            {
                                worked = false;
                            }
                            this.SendResponse(props, response, tag, worked);
                        };
                        channel.BasicConsume(queue: queueName,
                                             noAck: true,
                                             consumer: consumer);
                        Processor.CheckForAudioAgentInstances();
                        Console.ReadLine();
                        //Thread.Sleep(800);
                   // }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Application will now exit");
                Console.ReadLine();
            }
        }

        private void SendResponse(IBasicProperties props,byte[] response,ulong tag,bool worked)
        {
            ResponseSender rs = new ResponseSender();
            Thread responseThread = new Thread(() => rs.SendResponseToClient(props, response, tag, worked));
            responseThread.Start();
            //rs.SendResponseToClient(props, response, tag, worked);
        }
    }
}
