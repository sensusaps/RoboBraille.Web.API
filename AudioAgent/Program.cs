using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioAgent
{
    class Program
    {
        private static string[] topics = new string[] { "#" }, oldTopics;
        private static string workingDir = @"C:\AudioAgent";
        private static string consoleMessage = "Options:" + Environment.NewLine
                + " Press (c) Change binding keys." + Environment.NewLine
                + " Press (q) Change Queue name." + Environment.NewLine
                + " Press (p) to print out all the installed voices on this machine." + Environment.NewLine
                + " Press (n) to print out all voices names." + Environment.NewLine
                + " Press (Enter) to start the application.";
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.run(args);
        }

        public void run(string[] args)
        {
            string queueName = "rba16";
            if (File.Exists(workingDir + "\\agent.config"))
            {
                queueName = File.ReadAllText(workingDir + "\\agent.config");
            }
            bool skip = false;
            string input = "";
            if (args.Length > 0)
            {
                input = args[0];
                if (input.Equals("startup", StringComparison.InvariantCultureIgnoreCase))
                    skip = true;
            }
            if (File.Exists(workingDir + "\\topics.config"))
            {
                oldTopics = File.ReadAllLines(workingDir + "\\topics.config");
                if (oldTopics.Length > 0)
                    topics = oldTopics;
            }
            string res = "";
            foreach (var topic in topics)
            {
                res += topic + " ";
            }
            Console.WriteLine("Current queue is: " + queueName);
            Console.WriteLine("Current binding keys are: " + res);
            if (!skip)
            {
                Console.WriteLine(consoleMessage);
                ConsoleKeyInfo ck = Console.ReadKey();
                bool loop = true;
                do
                {
                    if (ConsoleKey.P.Equals(ck.Key))
                    {
                        AudioJobProcessor.WriteAllInstalledVoicesToFile();
                        Console.WriteLine(consoleMessage);
                        ck = Console.ReadKey();
                    }
                    if (ConsoleKey.N.Equals(ck.Key))
                    {
                        AudioJobProcessor.WriteVoiceNames();
                        Console.WriteLine(consoleMessage);
                        ck = Console.ReadKey();
                    }
                    if (ConsoleKey.C.Equals(ck.Key))
                    {
                        Console.WriteLine("Current binding keys are: " + res);
                        Console.WriteLine("Write each key separated by commas (,):");
                        string keys = Console.ReadLine();
                        topics = keys.Split(',');
                        Console.WriteLine(consoleMessage);
                        ck = Console.ReadKey();
                    }
                    if (ConsoleKey.Q.Equals(ck.Key))
                    {
                        Console.WriteLine("Current queue is: " + queueName + " Write new queue name: ");
                        queueName = Console.ReadLine();
                        Console.WriteLine(consoleMessage);
                        ck = Console.ReadKey();
                    }
                    if (ConsoleKey.Enter.Equals(ck.Key))
                    {
                        loop = false;
                        ck = Console.ReadKey();
                    }
                } while (loop);
                res = "";
                foreach (var topic in topics)
                {
                    res += topic + " ";
                }
                if (!File.Exists(workingDir + "\\agent.config"))
                {
                    File.Create(workingDir + "\\agent.config").Close();
                }
                File.WriteAllText(workingDir + "\\agent.config", queueName);

                if (!File.Exists(workingDir + "\\topics.config"))
                {
                    File.Create(workingDir + "\\topics.config").Close();
                }
                File.WriteAllLines(workingDir + "\\topics.config", topics);
            }
            Console.WriteLine("Audio Conversion Agent started listening on topics: " + res);
            QueueConsumer qc = new QueueConsumer();
            qc.Consume(queueName.Trim());
        }
    }
}
