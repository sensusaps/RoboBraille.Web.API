using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioAgent
{
    class ResponseSender
    {
        public ResponseSender()
        {

        }

        public void SendResponseToClient(IBasicProperties props, byte[] responseBytes, ulong tag, bool worked)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.Headers = new Dictionary<string, object>();
                    replyProps.Headers.Add("worked", worked);
                    replyProps.CorrelationId = props.CorrelationId;
                    replyProps.Persistent = true;
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: tag, multiple: false);
                    Console.WriteLine("Response sent to queue: " + props.ReplyTo + " Worked:" + worked + Environment.NewLine + "Time: " + DateTime.Now);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throws exception at last line but still delivers the result
            }
        }

        
    }
}
