using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleNetwork
{
    /// <summary>
    /// Stellt einen TCP-Client zur Verfügung.
    /// </summary>
    public class TCPClient : NetworkPeer
    {
        #region Private Member

        //Connection-Callback ist im Gange
        private bool _connecting;

        #endregion

        #region Properties

        /// <summary>
        /// Liefert einen Wert, der angibt, ob Daten versendet werden können.
        /// </summary>
        public bool Connected
        {
            get
            {
                return (mainSocket.Socket != null && mainSocket.Socket.Connected);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Stellt einen TCP-Client zur Verfügung.
        /// </summary>
        public TCPClient()
        {
        }

        #endregion

        #region StartStop Client

        /// <summary>
        /// Stellt die Verbindung mit dem Server her, und wartet auf eingehende Daten.
        /// </summary>
        /// <param name="address">Die Adresse, mit der verbunden wird.</param>
        /// <param name="port">Der Port, mit welchem eine Verbindung hergestellt wird.</param>
        public void Connect(IPAddress address, int port)
        {
            if ((mainSocket.Socket != null && mainSocket.Socket.Connected) || _connecting)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ConnectionFailed,
                    0, null, encoding.GetBytes("Client already connected.")));

                return;
            }

            try
            {
                mainSocket.Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _connecting = true;
                mainSocket.Socket.BeginConnect(address, port, ConnectCallback, null);
            }
            catch (SocketException ex)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ConnectionFailed,
                    0, null, encoding.GetBytes(ex.Message)));

                _connecting = false;
            }
        }

        private void ConnectCallback(object socket)
        {
            if (mainSocket.Socket.Connected)
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.Connected, 0, string.Empty, null));

                thread = new Thread(ClientThread);
                thread.Start();
            }
            else
            {
                EnqueueMessage(new NetworkMessage(NetworkMessageType.ConnectionFailed, 0, string.Empty, null));
            }

            _connecting = false;
        }

        /// <summary>
        /// Trennt die Verbindung zum Server.
        /// </summary>
        public void Disconnect()
        {
            if (mainSocket.Socket != null && mainSocket.Socket.Connected)
            {
                //mainSocket.Socket.Shutdown(SocketShutdown.Both);
                mainSocket.Socket.Close();
            }
        }

        /// <summary>
        /// Trennt die Verbindung zum Server, falls eine besteht, und sendet eine ExitRequest Nachricht.
        /// </summary>
        public void Exit()
        {
            Disconnect();
            EnqueueMessage(new NetworkMessage(NetworkMessageType.ExitRequest, 0, string.Empty, null));
        }

        #endregion

        #region Threads

        private void ClientThread()
        {
            while (true)
            {
                NetworkMessage message = GetNextMessage(mainSocket);
                if (message != null)
                {
                    EnqueueMessage(message);
                }
                else
                {
                    break;
                }
            }

            EnqueueMessage(new NetworkMessage(NetworkMessageType.Disconnected, 0, string.Empty, null));
            mainSocket.Socket = null;
        }

        #endregion

        #region SendData

        private bool SendData(MessageDataType type, byte[] data, ushort id, params string[] parameter)
        {
            if (type == MessageDataType.FormatedMessage)
            {
                return SendFormatedMessage(mainSocket, id, parameter);
            }

            return SendData(mainSocket, type, data);
        }

        /// <summary>
        /// Sendet Daten an den Server.
        /// </summary>
        /// <param name="data">Daten, die gesendet werden.</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn der Parameter data leer ist.</exception>
        /// <returns>Liefert true falls die Daten erfolgreich versendet wurden.</returns>
        public bool Send(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Der Parameter data darf nicht leer sein.", "data");
            }

            return SendData(MessageDataType.Bytes, data, 0, null);
        }

        /// <summary>
        /// Sendet eine Zeichenkette an den Server.
        /// </summary>
        /// <param name="value">Zeichenkette, die gesendet wird.</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn der Parameter value leer ist.</exception>
        /// <returns>Liefert true falls die Zeichenkette erfolgreich versendet wurde.</returns>
        public bool Send(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Der Parameter value darf nicht leer sein.", "value");
            }

            return SendData(MessageDataType.Bytes, encoding.GetBytes(value), 0, null);
        }

        /// <summary>
        /// Sendet eine formatierte Zeichenkette an den Server.
        /// </summary>
        /// <param name="id">ID (0 - 999)</param>
        /// <param name="parameter">Beliebige Zeichenkettenparameter</param>
        /// <exception cref="System.ArgumentException">Wird ausgelöst, wenn ID einen ungültigen Wert enthält.</exception>
        /// <returns>Liefert true falls die formatierte Zeichenkette erfolgreich versendet wurde.</returns>
        public bool Send(ushort id, params string[] parameter)
        {
            if (id > 999)
            {
                throw new ArgumentException("Die ID muss zwischen 0 und 999 liegen.", "id");
            }

            return SendData(MessageDataType.FormatedMessage, null, id, parameter);
        }

        #endregion
    }
}