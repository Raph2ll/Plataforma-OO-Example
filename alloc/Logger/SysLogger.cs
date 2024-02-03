using System;
using Interfaces;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.IO;

namespace Logger
{
    public class SysLogger : ILogger
    {
        private static string _path = "/dev/kmsg";
        private static ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private static bool _running = false;

        public void Start(IDictionary<string, object> config)
        {
            if (config != null)
            {
                if (config.TryGetValue("path", out var path))
                {
                    if (path != null)
                        _path = $"{path}";
                }
            }
        }

        public void Log(string message)
        {
            _messages.Enqueue(message);
            Write();
        }

        public void Log(Exception exception)
        {
            _messages.Enqueue(exception.Message);
            Write();
        }

        public void Log(string message, Exception exception)
        {
            _messages.Enqueue($"{message} {exception.Message}");
            Write();
        }

        private static void Write()
        {



            if (_running)
            {
                return;
            }

            _running = true;

            try
            {
                string? dir = Path.GetDirectoryName(_path);

                if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var file = new StreamWriter(_path, true))
                {
                    string? message;
                    while (_messages.IsEmpty == false)
                    {
                        if (_messages.TryDequeue(out message) && !string.IsNullOrEmpty(message))
                        {
                            string comand = $"echo \"{message}\" | sudo tee /dev/kmsg";
                            
                            ProcessStartInfo psi = new ProcessStartInfo("bash", $"-c \"{comand}\"")
                            {
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };

                            using (Process process = new Process { StartInfo = psi })
                            {
                                process.Start();
                                process.WaitForExit();

                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();

                                if (!string.IsNullOrWhiteSpace(output))
                                    Console.WriteLine($"Saída padrão: {output}");

                                if (!string.IsNullOrWhiteSpace(error))
                                    Console.WriteLine($"Erro: {error}");
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                _running = false;
            }
        }
    }
}