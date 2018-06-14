using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleNetwork
{
    /// <summary>
    /// Stellt einen TCP-Server zur Verfügung.
    /// </summary>
    public class TCPServer : NetworkPeer
    {
        #region Fields

        //Zwischenliste der verbundenen Clients.
        private Dictionary<long, NetworkSocket> _connectedClients = null;

        #endregion

        #region Properties

        /// <summary>
        /// Liefert die Anzahl, der verbundenen Clients.
        /// </summary>
        public int ConnectedClientsCount
        {
            get
            {
                return _connectedClients.Count;
            }
        }

        /// <summary>
        /// Liefert ein Array mit den verbundenen Clients.
        /// </summary>
        public long[] ConnectedClients
        {
            get
            {
                long[] result = new long[_connectedClients.Count];
                _connectedClients.Keys.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        /// Liefert einen Wert, der angibt, ob Nachrichten gesendet werden können.
        /// </summary>
        public bool CanSend
        {
            get
            {
                return mainSocket != null && _connectedClients.Count > 0;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Stellt einen TCP-Server bereit.
        /// </summary>
        public TCPServer()
        {
            _connectedClients = new Dictionary<long, NetworkSocket>();
        }

        #endregion

        #region StartStop Server

        /// <summary>
        /// Startet den Server und wartet auf eingehende Verbindungen.
        /// </summary>
        /// <param name="port">Der Port, der für den Server reserviert wird.</param>
        public void Start(int port)
        {
            Start(IPAddress.Any, port);
        }

        /// <summary>
        /// Startet den Server und wartet auf eingehende Verbindungen.
        /// </summary>
        /// <param name="address">Die Adresse, die dem Server zugeordnet wird.</param>
        /// <param name="port">Der Port, der für den Server reserviert wird.</param>
        public void Start(IPAddress address, int port)
        {
            if (address == null)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.Exception,
                    0, null, encoding.GetBytes("Address is null")));

                return;
            }
            if (mainSocket.Socket != null)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ServerStartFailed,
                    0, null, encoding.GetBytes("Server already started")));

                return;
            }

            ipEndPoint = new IPEndPoint(address, port);

            try
            {
                mainSocket.Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                mainSocket.Socket.Bind(ipEndPoint);
                mainSocket.Socket.Listen(25);
            }
            catch (SocketException se)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ServerStartFailed, 0, null, encoding.GetBytes(se.Message)));
                return;
            }

            thread = new Thread(ServerThread);
            thread.Start();

            if (thread.IsAlive)
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ServerStarted, 0, null, null));
        }

        /// <summary>
        /// Beendet den Server und schließt alle Verbindungen.
        /// </summary>
        public void Stop()
        {
            if (mainSocket.Socket == null)
                return;

            foreach (NetworkSocket clientSocket in _connectedClients.Values)
            {
                //clientSocket.Socket.Shutdown(SocketShutdown.Both);
                clientSocket.Socket.Close();
            }

            mainSocket.Socket.Close();
            mainSocket.Socket = null;

            _connectedClients.Clear();
        }

        /// <summary>
        /// Beendet den Server, falls dieser läuft, und sendet eine ExitRequest Nachricht.
        /// </summary>
        public void Exit()
        {
            Stop();
            EnqueueMessage(new NetworkMessage(NetworkMessageType.ExitRequest, 0, string.Empty, null));
        }

        /// <summary>
        /// Schließt die Verbindung mit dem angegebenen Client.
        /// </summary>
        /// <param name="client">Client, mit dem die Verbindung beendet werden soll.</param>
        public void DisconnectClient(long client)
        {
            if (_connectedClients.ContainsKey(client))
            {
                _connectedClients[client].Socket.Close();
            }
        }

        #endregion

        #region Threads

        /// <summary>
        /// Wartet auf eingehende Verbindungen, und erstellt für jeden Client einen Thread.
        /// </summary>
        private void ServerThread()
        {
            while (true)
            {
                try
                {
                    //Neue eingehende Verbindung
                    Socket socket = mainSocket.Socket.Accept();
                    long client = socket.Handle.ToInt64();

                    //Verbindung in Liste sichern
                    if (!_connectedClients.ContainsKey(client))
                    {
                        NetworkSocket extendedSocket = new NetworkSocket(socket);

                        _connectedClients.Add(client, extendedSocket);

                        //Neuen Clientthread erzeugen
                        Thread clientThread = new Thread(ClientThread);
                        clientThread.Start(extendedSocket);
                    }
                    else
                    {
                        EnqueueMessage(new NetworkMessage(NetworkMessageType.Debug, client, string.Empty, encoding.GetBytes(
                            string.Format("Ungültiger Client '{0}': Doppelt vorhanden", client))));
                    }
                }
                catch (SocketException)
                {
                    //Fehler beim Zugriff auf den Socket.
                    break;
                }
                catch (ObjectDisposedException)
                {
                    //Das Socket wurde geschlossen.
                    break;
                }
                catch (InvalidOperationException)
                {
                    //Der annehmende Socket überwacht keine Verbindungen.
                    //Vor dem Aufruf von Accept müssen Sie Bind und Listen aufrufen.
                    break;
                }
            }

            if (_connectedClients.Count > 0)
            {
                _connectedClients.Clear();
            }

            EnqueueMessage(new NetworkMessage(NetworkMessageType.ServerStoped, 0, string.Empty, null));
        }

        /// <summary>
        /// Empfängt Daten vom Client.
        /// </summary>
        private void ClientThread(object socket)
        {
            NetworkSocket clientSocket = socket as NetworkSocket;
            if (clientSocket == null)
            {
                return;
            }

            long client = clientSocket.Socket.Handle.ToInt64();

            //Neue Verbindung, Ereignis auslösen
            EnqueueMessage(new NetworkMessage(NetworkMessageType.Connected, client, clientSocket.EndPoint, null));

            while (true)
            {
                NetworkMessage args = GetNextMessage(clientSocket);
                if (args != null)
                {
                    EnqueueMessage(args);
                }
                else
                {
                    break;
                }
            }

            //Verbindung aus Liste entfernen
            if (_connectedClients.ContainsKey(client))
            {
                _connectedClients.Remove(client);
            }

            //Falls Verbindung noch besteht, diese beenden
            if (clientSocket.Socket.Connected)
            {
                //clientSocket.Socket.Shutdown(SocketShutdown.Both);
                clientSocket.Socket.Close();
            }

            //Ereignis auslösen
            EnqueueMessage(new NetworkMessage(NetworkMessageType.Disconnected, client, clientSocket.EndPoint, null));
        }

        #endregion

        #region SendData

        private int SendData(long client, MessageDataType type, byte[] data, ushort id, params string[] parameter)
        {
            if (type != MessageDataType.FormatedMessage && (data == null || data.Length == 0))
            {
                return 0;
            }

            if (client == 0)
            {
                int result = 0;

                foreach (NetworkSocket clientSocket in _connectedClients.Values)
                {
                    if (clientSocket.Socket.Connected)
                    {
                        if (type == MessageDataType.FormatedMessage)
                        {
                            if (SendFormatedMessage(clientSocket, id, parameter))
                            {
                                result++;
                            }
                        }
                        else
                        {
                            if (SendData(clientSocket, type, data))
                            {
                                result++;
                            }
                        }
                    }
                }

                return result;
            }

            if (_connectedClients.ContainsKey(client) && _connectedClients[client].Socket.Connected)
            {
                if (type == MessageDataType.FormatedMessage)
                {
                    if (SendFormatedMessage(_connectedClients[client], id, parameter))
                    {
                        return 1;
                    }
                }
                else
                {
                    if (SendData(_connectedClients[client], type, data))
                    {
                        return 1;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Sendet Daten an einen oder alle Clients.
        /// </summary>
        /// <param name="client">Client, an dem gesendet wird, oder 0 um an alle zu senden.</param>
        /// <param name="data">Daten die gesendet werden.</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn der Parameter data leer ist.</exception>
        /// <returns>Liefert die Anzahl der Client an denen die Nachricht erfolgreich versendet wurde.</returns>
        public int Send(long client, byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Der Parameter data darf nicht leer sein.", "data");
            }

            return SendData(client, MessageDataType.Bytes, data, 0, null);
        }

        /// <summary>
        /// Sendet eine Zeichenkette an einen oder alle Clients.
        /// </summary>
        /// <param name="client">Client, an dem gesendet wird, oder 0 um an alle zu senden.</param>
        /// <param name="value">Zeichenkette die gesendet wird.</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn der Parameter value leer ist.</exception>
        /// <returns>Liefert die Anzahl der Client an denen die Nachricht erfolgreich versendet wurde.</returns>
        public int Send(long client, string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
            {
                throw new ArgumentException("Der Parameter value darf nicht leer sein.", "value");
            }

            return SendData(client, MessageDataType.Bytes, encoding.GetBytes(value), 0, null);
        }

        /// <summary>
        /// Sendet eine formatierte Zeichenkette an einen oder alle Clients.
        /// </summary>
        /// <param name="client">Client, an dem gesendet wird, oder 0 um an alle zu senden.</param>
        /// <param name="id">ID (0 - 999)</param>
        /// <param name="parameter">Beliebige Zeichenkettenparameter</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn der Parameter ID in einen ungültigen Bereich liegt.</exception>
        /// <returns>Liefert die Anzahl der Client an denen die Nachricht erfolgreich versendet wurde.</returns>
        public int Send(long client, ushort id, params string[] parameter)
        {
            if (id > 999)
            {
                throw new ArgumentException("ID muss zwischen 0 und 999 liegen.", "id");
            }

            return SendData(client, MessageDataType.FormatedMessage, null, id, parameter);
        }

        #endregion
    }
}