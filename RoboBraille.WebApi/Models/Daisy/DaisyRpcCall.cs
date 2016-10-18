using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    public class DaisyRpcCall : IDisposable, IDaisyRpcCall
    {
        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        /// <summary>
        /// Constructor. A new class instance for each request for conversion.
        /// </summary>
        public DaisyRpcCall()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare();
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(replyQueueName, true, consumer);
        }

        /// <summary>
        /// Calls the backend component.
        /// </summary>
        /// <param name="parameters">A valid InputParameters class.</param>
        /// <returns>The converted and contracted result as a string.</returns>
        public byte[] Call(byte[] document, bool isEpub3, string guid)
        {
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = guid;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("isEpub3",isEpub3);
            var messageBytes = document;
            channel.BasicPublish("", "DaisyPipeline.rpc_queue", props, messageBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == guid)
                {
                    return ea.Body;
                }
            }
        }
        /// <summary>
        /// Must be called in order to close the connection once the result has been received.
        /// </summary>
        public void Dispose()
        {
            connection.Close();
        }
    }
}