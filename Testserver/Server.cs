using System;
using System.Threading;
using SimpleNetwork;

namespace Testserver
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

    class Server
    {
        TCPServer server;

        public Server()
        {
            Console.Title = "Server";

            server = new TCPServer();
            server.Start(6902);

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

                    case "help":
                        Console.WriteLine("send Nachricht");
                        Console.WriteLine("spam Nachricht Wiederholungen");
                        Console.WriteLine("statistics");
                        Console.WriteLine("clear");
                        Console.WriteLine("connected");
                        Console.WriteLine("start");
                        Console.WriteLine("stop");
                        Console.WriteLine("exit");
                        break;

                    case "statistics":
                        Console.WriteLine("Empfangen: {0:n0} Bytes, Gesendet: {1:n0} Bytes", server.TotalReceivedBytes, server.TotalSendedBytes);
                        break;

                    case "stop":
                        server.Stop();
                        break;
                    case "start":
                        server.Start(6902);
                        break;

                    case "connected":
                        Console.WriteLine("{0} Verbindungen", server.ConnectedClientsCount);
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
                            if (server.CanSend)
                            {
                                string message = input[1];

                                for (int i = 2; i < input.Length; i++)
                                    message += " " + input[i];

                                server.Send(0, message);
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

            server.Exit();
        }

        private void SpamThread(object messageParam)
        {
            Spamparameter parameter = (Spamparameter)messageParam;

            int counter = 0;

            do
            {
                if (server.CanSend)
                    server.Send(0, parameter.Message);

                counter++;

            } while (counter < parameter.Amount);
        }

        private void ListenThread()
        {
            NetworkMessage message;

            while (true)
            {
                message = server.GetMessage();

                if (message.Type == NetworkMessageType.ExitRequest)
                    break;

                switch (message.Type)
                {
                    case NetworkMessageType.ServerStarted:
                        Console.WriteLine("Server gestartet");
                        break;
                    case NetworkMessageType.ServerStoped:
                        Console.WriteLine("Server beendet");
                        break;

                    case NetworkMessageType.Connected:
                        Console.WriteLine("Client hat eine Verbindung aufgebaut: {0}", message.Client);
                        break;
                    case NetworkMessageType.Disconnected:
                        Console.WriteLine("Client hat die Verbindung geschlossen: {0}", message.Client);
                        break;

                    case NetworkMessageType.Message:
                        using (BinaryMessageReader reader = new BinaryMessageReader(message.Buffer))
                        {
                            Console.WriteLine(reader.ReadByte());
                            Console.WriteLine(reader.ReadFloat());
                            Console.WriteLine(reader.ReadString());
                        }

                        break;

                    //case NetworkMessageType.Message:
                    //    Console.WriteLine("Nachricht von Client {0}: '{1}', {2} Bytes",
                    //        message.Client,
                    //        server.Encoding.GetString(message.Buffer),
                    //        message.Buffer.Length);

                    //    break;

                    case NetworkMessageType.FormatedMessage:
                        Console.WriteLine("FormatedMessage: ID: {0}, Parameter: {1}",
                            message.ID, message.Parameter.Length);

                        switch (message.ID)
                        {
                            case 10: //Login
                                if (message.Parameter.Length >= 2)
                                {
                                    if (message.Parameter[0] == "kai" && message.Parameter[1] == "1234")
                                        Console.WriteLine("Kai hat sich angemeldet");
                                }
                                break;

                            case 11: //Message
                                if (message.Parameter.Length >= 2)
                                {
                                    Console.WriteLine("Benutzer {0} hat {1} gesendet!",
                                        message.Parameter[0], message.Parameter[1]);

                                    server.Send(0, 11, message.Parameter[0], message.Parameter[1]);
                                }

                                break;
                        }

                        break;

                    default:
                        Console.WriteLine("Unbehandelte Nachricht: {0}",
                            message.Type.ToString());
                        break;
                }
            }
        }
    }
}