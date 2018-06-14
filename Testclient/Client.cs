using System;
using System.Net;
using System.Threading;
using SimpleNetwork;

namespace Testclient
{
    class Spamparameter
    {
        public Spamparameter(string message, int amount)
        {
            this.Message = message;
            this.Amount = amount;
        }

        public int Amount { get; private set; }
        public string Message { get; private set; }
    }

    class Client
    {
        TCPClient client;

        public Client()
        {
            Console.Title = "Client";

            client = new TCPClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 6902);

            Thread listenThread = new Thread(new ThreadStart(ListenThread));
            listenThread.Start();

            string[] input;
            while (true)
            {
                input = Console.ReadLine().Trim().Split(' ');

                for (int i = 0; i < input.Length; i++)
                    input[i] = input[i].Trim();

                if (input[0] == "exit")
                    break;

                switch (input[0])
                {
                    case "stop":
                        client.Disconnect();
                        break;
                    case "start":
                        client.Connect(IPAddress.Parse("127.0.0.1"), 6902);
                        break;

                    case "help":
                        Console.WriteLine("send Nachricht");
                        Console.WriteLine("spam Nachricht Wiederholungen");
                        Console.WriteLine("statistics");
                        Console.WriteLine("clear");
                        Console.WriteLine("start");
                        Console.WriteLine("stop");
                        Console.WriteLine("exit");
                        break;

                    case "statistics":
                        Console.WriteLine("Empfangen: {0:n0} Bytes, Gesendet: {1:n0} Bytes", client.TotalReceivedBytes, client.TotalSendedBytes);
                        break;

                    case "spam":
                        int amount;
                        if (input.Length == 3 && !string.IsNullOrEmpty(input[1]) && int.TryParse(input[2], out amount))
                        {
                            Thread spamThread = new Thread(new ParameterizedThreadStart(SpamThread));
                            spamThread.Start(new Spamparameter(input[1], amount));
                        }
                        else
                        {
                            Console.WriteLine("Syntaxfehler: spam Nachricht Wiederholungen");
                        }
                        break;

                    case "send":
                        if (input.Length >= 2 && !string.IsNullOrEmpty(input[1]))
                        {
                            if (client.Connected)
                            {
                                string message = input[1];

                                for (int i = 2; i < input.Length; i++)
                                    message += " " + input[i];

                                client.Send(message);
                            }
                            else
                                Console.WriteLine("Senden momentan nicht möglich");
                        }
                        else
                        {
                            Console.WriteLine("Syntaxfehler: send Nachricht");
                        }
                        break;

                    case "clear":
                    case "cls":
                        Console.Clear();
                        break;

                    default:
                        Console.WriteLine("Unbekannter Befehl: {0}", input[0]);
                        Console.WriteLine("Hilfe: help");
                        break;
                }
            }

            client.Exit();
        }

        private void SpamThread(object messageParam)
        {
            Spamparameter parameter = (Spamparameter)messageParam;

            int counter = 0;

            do
            {
                if (client.Connected)
                    client.Send(parameter.Message);

                counter++;

            } while (counter < parameter.Amount);
        }

        private void ListenThread()
        {
            NetworkMessage message;

            while (true)
            {
                message = client.GetMessage();

                if (message.Type == NetworkMessageType.ExitRequest)
                    break;

                switch (message.Type)
                {
                    case NetworkMessageType.Connected:
                        Console.WriteLine("Verbindung erfolgreich aufgebaut");
                        break;
                    
                    case NetworkMessageType.ConnectionFailed:
                        Console.WriteLine("Verbindung fehlgeschlagen");
                        break;

                    case NetworkMessageType.Disconnected:
                        Console.WriteLine("Verbindung geschlossen");
                        break;

                    case NetworkMessageType.Message:
                        Console.WriteLine("Nachricht: '{0}', {1} Bytes", client.Encoding.GetString(message.Buffer), message.Buffer.Length);
                        break;

                    default:
                        Console.WriteLine("Unbehandelte Nachricht: {0}", message.Type);
                        break;
                }
            }
        }
    }
}