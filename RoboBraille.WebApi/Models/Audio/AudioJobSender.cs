using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// Based on the necessary language select the appropriate queue and send it to the that queue.
    /// Topic exchange: <component>.<property1>.<property2> 
    /// Example: audio.enUS.none.none or audio.daDK.female.sara
    ///          daisy.dtbook.none or daisy.epub3.none
    /// </summary>
    public class AudioJobSender : IAudioJobSender
    {
        public AudioJobSender()
        {
         
        }

        public byte[] SendAudioJobToQueue(AudioJob auJob)
        {
            AudioReplyQueue rq = new AudioReplyQueue();
            var props = rq.Start(auJob.Id.ToString());
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>();
            string voiceProp = "";
            if (auJob.VoicePropriety.Count() > 0)
            {
                if (auJob.VoicePropriety.Count() > 1)
                {
                    foreach (VoicePropriety vp in auJob.VoicePropriety)
                        voiceProp += vp + ":";
                    voiceProp = voiceProp.Substring(0, voiceProp.Length - 1);
                }
                else voiceProp = "" + auJob.VoicePropriety.First();
            } else
            {
                voiceProp = "" + VoicePropriety.None;
            }
            props.Headers.Add("inputPropriety", (byte[]) Encoding.UTF8.GetBytes(voiceProp));
            props.Headers.Add("inputLanguage",(int) auJob.AudioLanguage);
            props.Headers.Add("voiceSpeed", (int) auJob.SpeedOptions);
            props.Headers.Add("format", (int) auJob.FormatOptions);
            //AudioJobSender c = new AudioJobSender();
            string key = GetRoutingKey(auJob);
            Thread pubTrhead = new Thread(() => PublishToTopicConsumer(props,auJob.FileContent,key));
            pubTrhead.Start();
            //c.PublishToTopicConsumer(props, auJob.FileContent, key);
            byte[] response = rq.getReply();
            return response;
        }

        private static void PublishToTopicConsumer(IBasicProperties props, byte[] messageBody, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "audio_job", type: "topic");
                //var routingKey = "audio.nl.male.no"; //make sure the topic exists or else it will wait indefinetly
                var body = messageBody;
                channel.BasicPublish(exchange: "audio_job",
                                     routingKey: routingKey,
                                     basicProperties: props,
                                     body: body);
                //Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, Environment.CurrentDirectory + "\\A2.testANSI.txt");
            }
        }

        private static string GetRoutingKey(AudioJob auJob)
        {
            var routingKey = "audio.";
            switch (auJob.AudioLanguage)
            {
                case Language.daDK:
                    routingKey += "da.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                    {
                        routingKey += "male.Carsten";
                    }
                    else
                    {
                        //routingKey += "female.Anne";
                        routingKey += "female.Sara";
                    }
                    break;
                case Language.ltLT:
                    routingKey += "lt.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                        routingKey += "male.";
                    else routingKey += "female.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Older))
                        routingKey += "older";
                    else routingKey += "younger";
                    break;
                //case Language.arEG:
                //    routingKey += "ar.";
                //    if (auJob.VoicePropriety.Contains(VoicePropriety.Bilingual))
                //        routingKey += "bilingual.none";
                //    else routingKey += "none.none";
                //    break;
                case Language.huHU: routingKey += "hu.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                        routingKey += "male.none";
                    else routingKey += "female.none";
                    break;
                case Language.isIS: routingKey += "is.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                        routingKey += "male.none";
                    else routingKey += "female.none";
                    break;
                case Language.nlNL: routingKey += "nl.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                        routingKey += "male.none";
                    else routingKey += "female.none";
                    break;
                case Language.cyGB:
                    routingKey += "cy.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Male))
                        routingKey += "male.none";
                    else routingKey += "female.none";
                    break;
                case Language.enUS: routingKey += "en.us.none"; break;
                case Language.enGB: routingKey += "en.gb.none"; break;
                case Language.enCA: routingKey += "en.ca.none"; break;
                case Language.enIN: routingKey += "en.in.none"; break;
                case Language.enAU: routingKey += "en.au.none"; break;
                case Language.frFR: routingKey += "fr.fr.none"; break;
                case Language.frCA: routingKey += "fr.ca.none"; break;
                case Language.deDE: routingKey += "de.none.none"; break;
                case Language.esES: routingKey += "es.es.none"; break;
                case Language.esCO: routingKey += "es.co.none"; break;
                case Language.bgBG: routingKey += "bg.none.none"; break;
                case Language.itIT: routingKey += "it.none.none"; break;
                case Language.nbNO: routingKey += "nb.no.none"; break;
                case Language.roRO: routingKey += "ro.none.none"; break;
                case Language.svSE: routingKey += "se.none.none"; break;
                case Language.plPL: routingKey += "pl.none.none"; break;
                case Language.ptPT: routingKey += "pt.pt.none"; break;
                case Language.ptBR: routingKey += "pt.br.none"; break;
                case Language.klGL: routingKey += "kl.none.none"; break;
                case Language.elGR: routingKey += "gr.none.none"; break;
                case Language.slSI: routingKey += "sl.none.none"; break;
                case Language.jaJP: routingKey += "jp.none.none"; break;
                case Language.koKR: routingKey += "kr.none.none"; break;
                case Language.zhHK: routingKey += "chn.hk.none"; break;
                case Language.zhCN:
                    routingKey += "chn.";
                    if (auJob.VoicePropriety.Contains(VoicePropriety.Cantonese))
                        routingKey += "cantonese.none";
                    else routingKey += "mandarin.none";
                    break;
                case Language.zhTW: routingKey += "chn.taiwanese.none"; break;
                case Language.fiFI: routingKey += "fi.none.none"; break;
                case Language.ruRU: routingKey += "ru.none.none"; break;
                case Language.esMX: routingKey += "es.mx.none"; break;
                case Language.caES: routingKey += "ca.es.none"; break;
                case Language.czCZ: routingKey += "cz.cz.none"; break;
                case Language.skSK: routingKey += "sk.sk.none"; break;
                default: routingKey += "none.none.none"; break;
            }
            return routingKey;
        }
        
    }
}