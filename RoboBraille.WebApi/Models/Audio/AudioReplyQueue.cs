using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class AudioReplyQueue
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;
        private string corrId;

        public AudioReplyQueue()
        {

        }
        public IBasicProperties Start(string jobId)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            channel.BasicQos(0, 1, false);
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueueName,
                                 noAck: true,
                                 consumer: consumer);

            corrId = jobId;
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;
            return props;
        }
        public byte[] getReply()
        {
            while (true)
            {
                //Console.WriteLine("Waiting for response on queue: "+replyQueueName);
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == corrId)
                {
                    byte[] response = ea.Body;
                    object worked;
                    ea.BasicProperties.Headers.TryGetValue("worked", out worked);
                    channel.BasicAck(ea.DeliveryTag, false);
                    this.Close();
                    if ((bool)worked)
                        return response;
                    else return null;
                }
            }
        }
        ///// <summary>
        ///// Wait for reply 30 minutes and publish failure after
        ///// </summary>
        ///// <returns></returns>
        //public byte[] getReply()
        //{
        //    //choose the best action? timeout after 30 minutes?
        //    bool wait = true;
        //    byte[] result = null;
        //    long i = 0;
        //    while (wait)
        //    {
        //        Console.WriteLine("Waiting for response on queue: " + replyQueueName);
        //        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
        //        if (ea.BasicProperties.CorrelationId == corrId)
        //        {
        //            byte[] response = ea.Body;
        //            object worked;
        //            ea.BasicProperties.Headers.TryGetValue("worked", out worked);
        //            channel.BasicAck(ea.DeliveryTag, false);
        //            this.Close();
        //            if ((bool)worked)
        //                result = response;
        //            else result = null;
        //        }
        //        Thread.Sleep(500);
        //        i++;
        //        if (i > 3600)
        //        {
        //            wait = false;
        //            result = null;
        //        }
        //    }
        //    return result;
        //}

        private void Close()
        {
            connection.Close();
        }
    }
}