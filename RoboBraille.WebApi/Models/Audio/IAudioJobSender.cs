using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Models
{
    public interface IAudioJobSender
    {
        byte[] SendAudioJobToQueue(AudioJob auJob);
    }
}
