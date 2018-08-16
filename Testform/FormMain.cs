using PureFreak.SimpleNetwork;
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ConsoleWrapper
{
    public partial class FormMain : Form
    {
        TCPServer server;
        TCPClient client;

        public FormMain()
        {
            InitializeComponent();

            tbxServer.Font = new System.Drawing.Font(tbxServer.Font.FontFamily, 12);
            tbxClient.Font = new System.Drawing.Font(tbxClient.Font.FontFamily, 12);

            server = new TCPServer();
            client = new TCPClient();

            System.Windows.Forms.Timer timerServer = new System.Windows.Forms.Timer();
            timerServer.Interval = 10;
            timerServer.Tick += new EventHandler(ServerThread);
            timerServer.Start();

            System.Windows.Forms.Timer timerClient = new System.Windows.Forms.Timer();
            timerClient.Interval = 10;
            timerClient.Tick += new EventHandler(ClientThread);
            timerClient.Start();

            server.Start(6902);
            client.Connect(IPAddress.Parse("127.0.0.1"), 6902);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Exit();
            client.Exit();
        }

        private void AddMessage(bool server, string message, params object[] parameter)
        {
            string formatedMessage;

            if (parameter != null)
                formatedMessage = string.Format(message, parameter);
            else
                formatedMessage = message;

            TextBox control;

            if (server)
                control = tbxServer;
            else
                control = tbxClient;

            control.AppendText(formatedMessage + Environment.NewLine);
            control.Select(control.Text.Length, 0);
        }

        private void ServerThread(object sender, EventArgs args)
        {
            NetworkMessage message = server.GetAsyncMessage();
            if (message == null)
                return;

            switch (message.Type)
            {
                case NetworkMessageType.ServerStarted:
                    AddMessage(true, "Server wurde gestartet", null);
                    break;
                case NetworkMessageType.ServerStartFailed:
                    AddMessage(true, "Server konnte nicht gestartet werden: {0}", server.Encoding.GetString(message.Buffer));
                    break;
                case NetworkMessageType.ServerStoped:
                    AddMessage(true, "Server wurde beendet", null);
                    break;

                case NetworkMessageType.Connected:
                    AddMessage(true, "Client hat eine Verbindung aufgebaut: {0}", message.Client);
                    break;
                case NetworkMessageType.Disconnected:
                    AddMessage(true, "Client hat die Verbindung geschlossen: {0}", message.Client);
                    break;

                case NetworkMessageType.Message:
                    AddMessage(true, "Nachricht von Client {0}: '{1}', {2} Bytes",
                        message.Client,
                        server.Encoding.GetString(message.Buffer),
                        message.Buffer.Length);
                    break;
                case NetworkMessageType.FormatedMessage:
                    AddMessage(true, "Formatierte Nachricht: ID: {0}, Parameter: {1}", message.ID, message.Parameter.Length);
                    break;

                case NetworkMessageType.ExitRequest:
                    return;

                default:
                    AddMessage(true, "Unbehandelte Nachricht: {0}", message.Type.ToString());
                    break;
            }
        }

        private void ClientThread(object sender, EventArgs args)
        {
            NetworkMessage message = client.GetAsyncMessage();
            if (message == null)
                return;

            switch (message.Type)
            {
                case NetworkMessageType.Connected:
                    AddMessage(false, "Verbindung zum Server erfolgreich aufgebaut", null);
                    break;
                case NetworkMessageType.ConnectionFailed:
                    AddMessage(false, "Verbindung zum Server fehlgeschlagen", null);
                    break;
                case NetworkMessageType.Disconnected:
                    AddMessage(false, "Verbindung zum Server verloren", null);
                    break;

                case NetworkMessageType.Message:
                    AddMessage(false, "Nachricht: '{0}', {1} Bytes", client.Encoding.GetString(message.Buffer), message.Buffer.Length);
                    break;
                case NetworkMessageType.FormatedMessage:
                    AddMessage(false, "Formatierte Nachricht: ID: {0}, Parameter: {1}", message.ID, message.Parameter.Length);
                    break;

                default:
                    AddMessage(false, "Unbehandelte Nachricht: {0}", message.Type);
                    break;

                case NetworkMessageType.ExitRequest:
                    return;
            }
        }

        private void DoCommand(bool isServer, string command)
        {
            string[] input;

            input = command.Trim().Split(' ');
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = input[i].Trim();
            }

            switch (input[0])
            {
                case "exit":
                    this.Close();
                    break;

                case "stop":
                    if (isServer)
                        server.Stop();
                    else
                        client.Disconnect();

                    break;

                case "start":
                    if (isServer)
                        server.Start(6902);
                    else
                        client.Connect(IPAddress.Parse("127.0.0.1"), 6902);

                    break;

                case "help":
                    AddMessage(isServer, "send Nachricht");
                    AddMessage(isServer, "sendf id parameter1, parameter2, ..");
                    AddMessage(isServer, "spam Nachricht Wiederholungen");
                    AddMessage(isServer, "statistics");
                    AddMessage(isServer, "clear");
                    AddMessage(isServer, "start");
                    AddMessage(isServer, "stop");
                    AddMessage(isServer, "exit");

                    break;

                case "statistics":
                    if (isServer)
                        AddMessage(true, "Empfangen: {0:n0} Bytes, Gesendet: {1:n0} Bytes", server.TotalReceivedBytes, server.TotalSendedBytes);
                    else
                        AddMessage(false, "Empfangen: {0:n0} Bytes, Gesendet: {1:n0} Bytes", client.TotalReceivedBytes, client.TotalSendedBytes);

                    break;

                case "send":
                    if (input.Length >= 2 && !string.IsNullOrEmpty(input[1]))
                    {
                        if (client.Connected)
                        {
                            string message = input[1];

                            for (int i = 2; i < input.Length; i++)
                                message += " " + input[i];

                            if (isServer)
                            {
                                server.Send(0, message);
                            }
                            else
                            {
                                client.Send(message);
                            }
                        }
                        else
                            AddMessage(isServer, "Senden momentan nicht möglich", null);
                    }
                    else
                    {
                        AddMessage(isServer, "Syntaxfehler: send Nachricht", null);
                    }
                    break;

                case "sendf":
                    ushort id;
                    if (input.Length >= 2
                        && ushort.TryParse(input[1], out id))
                    {
                        string[] parameter = new string[input.Length - 2];
                        for (int i = 2; i < input.Length; i++)
                        {
                            parameter[i - 2] = input[i];
                        }

                        if (isServer)
                        {
                            server.Send(0, id, parameter);
                        }
                        else
                        {
                            client.Send(id, parameter);
                        }
                    }
                    else
                    {
                        AddMessage(isServer, "Syntaxfehler: sendf id parameter1, parameter2, ..");
                    }
                    break;

                case "spam":
                    int amount;
                    if (input.Length == 3 && !string.IsNullOrEmpty(input[1]) && int.TryParse(input[2], out amount))
                    {
                        Thread spamThread = new Thread(new ParameterizedThreadStart(SpamThread));
                        spamThread.Start(new Spamparameter(isServer, input[1], amount));
                    }
                    else
                    {
                        Console.WriteLine("Syntaxfehler: spam Nachricht Wiederholungen");
                    }
                    break;

                case "clear":
                case "cls":
                    if (isServer)
                        tbxServer.Clear();
                    else
                        tbxClient.Clear();

                    break;

                default:
                    AddMessage(isServer, "Unbekannter Befehl: {0}", input[0]);
                    AddMessage(isServer, "Hilfe: help", null);

                    break;
            }
        }

        private void SpamThread(object messageParam)
        {
            Spamparameter parameter = (Spamparameter)messageParam;

            int counter = 0;

            do
            {
                if (parameter.Server)
                {
                    if (server.CanSend)
                        server.Send(0, string.Format("{0:00}: '{1}'", counter + 1, parameter.Message));
                }
                else
                {
                    if (client.Connected)
                        client.Send(string.Format("{0:00}: '{1}'", counter + 1, parameter.Message));
                }

                counter++;
            } while (counter < parameter.Amount);
        }

        private void tbxServerInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(tbxServerInput.Text.Trim()))
            {
                DoCommand(true, tbxServerInput.Text);
                tbxServerInput.ResetText();
            }
        }

        private void tbxClientInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(tbxClientInput.Text.Trim()))
            {
                DoCommand(false, tbxClientInput.Text);
                tbxClientInput.ResetText();
            }
        }
    }
}