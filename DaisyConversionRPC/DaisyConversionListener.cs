using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaisyConversionRPC
{
    public class DaisyConversionListener
    {
        public static void run()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("DaisyPipeline.rpc_queue", false, false, false, null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("DaisyPipeline.rpc_queue", false, consumer);
                    Console.WriteLine(" [x] Awaiting RPC requests");

                    while (true)
                    {
                        byte[] response = null;
                        var ea =
                            (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        object isEpub3 = false;
                        props.Headers.TryGetValue("isEpub3",out isEpub3);
                        try
                        {
                            DaisyPipelineConverter dpc = new DaisyPipelineConverter(props.CorrelationId);
                            response = dpc.ManageDaisyConversion(body, (bool) isEpub3);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(" [.] " + e.Message);
                        }
                        finally
                        {
                            if (response == null)
                                Console.WriteLine("An error was encountered while converting your document. Content is null.");
                            else Console.WriteLine("Process: "+props.CorrelationId+" Finished! Success!");
                            channel.BasicPublish("", props.ReplyTo, replyProps, response);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                }
            }
        }
    }
}