using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboBraille.WebApi.Test
{
    class RabbitMQCluster
    {
        public static void ClusterConnect()
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "cmd.exe",

                // The Process object must have the UseShellExecute property set to false in order to redirect IO streams.
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var proc = new Process();
            // The "using" is more safe alternative for "proc.Close()" to release resources in the process' wrapper.
            using (proc)
            {
                proc.StartInfo = info;
                proc.Start();

                proc.StandardInput.WriteLine("rabbitmqctl stop_app");
                proc.StandardInput.WriteLine();
                proc.StandardInput.WriteLine("rabbitmqctl join_cluster rabbit@RBA16");
                proc.StandardInput.WriteLine();
                proc.StandardInput.WriteLine("rabbitmqctl start_app");
                proc.StandardInput.WriteLine();

                // Allow not-blocking use of ReadToEnd().
                // Use the CMD EXIT command vs "proc.StandardInput.Close()" to pass the exit code to .NET proc.ExitCode below;
                proc.StandardInput.WriteLine("EXIT");
                // No more the used CMD process\ here.

                var waitSeconds = 1;
                var interrupted = !proc.WaitForExit(waitSeconds * 1000);

                // Remember to use async reads if you wish.
                //proc.BeginOutputReadLine
                //proc.BeginErrorReadLine

                var output = proc.StandardOutput.ReadToEnd();
                var errorOutput = proc.StandardError.ReadToEnd();

                var exitCode = proc.ExitCode;
                if (exitCode != 0)
                {
                    // Your actions.
                }
            }
        }

        public static void ClusterDisconnect()
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "cmd.exe",

                // The Process object must have the UseShellExecute property set to false in order to redirect IO streams.
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            var proc = new Process();
            // The "using" is more safe alternative for "proc.Close()" to release resources in the process' wrapper.
            using (proc)
            {
                proc.StartInfo = info;
                proc.Start();

                proc.StandardInput.WriteLine("rabbitmqctl stop_app");
                proc.StandardInput.WriteLine();
                proc.StandardInput.WriteLine("rabbitmqctl reset");
                proc.StandardInput.WriteLine();
                proc.StandardInput.WriteLine("rabbitmqctl start_app");
                proc.StandardInput.WriteLine();

                // Allow not-blocking use of ReadToEnd().
                // Use the CMD EXIT command vs "proc.StandardInput.Close()" to pass the exit code to .NET proc.ExitCode below;
                proc.StandardInput.WriteLine("EXIT");
                // No more the used CMD process\ here.

                var waitSeconds = 1;
                var interrupted = !proc.WaitForExit(waitSeconds * 1000);

                // Remember to use async reads if you wish.
                //proc.BeginOutputReadLine
                //proc.BeginErrorReadLine

                var output = proc.StandardOutput.ReadToEnd();
                var errorOutput = proc.StandardError.ReadToEnd();

                var exitCode = proc.ExitCode;
                if (exitCode != 0)
                {
                    // Your actions.
                }
            }
        }
    }
}
