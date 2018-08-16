using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PureFreak.SimpleNetwork
{
    /// <summary>
    /// Stellt die Basisklasse für Socketbasierte TCP Komponenten dar.
    /// </summary>
    public abstract class NetworkPeer
    {
        #region Constructor

        /// <summary>
        /// Stellt die Basisklasse für Socketbasierte TCP Komponenten dar.
        /// </summary>
        protected NetworkPeer()
        {
            this.mainSocket = new NetworkSocket(null);
            this.encoding = Encoding.UTF8;
            this.identifiy = new byte[] { 83, 78 };
            this.messages = new Queue<NetworkMessage>();
            this.messageLock = new object();
            this.messageOptions = NetworkMessageOptions.Compressed;
        }

        #endregion

        #region Fields

        private byte[] identifiy;
        private Queue<NetworkMessage> messages;
        private object messageLock;

        /// <summary>
        /// Der Sockel der verwendet wird.
        /// </summary>
        protected NetworkSocket mainSocket;

        /// <summary>
        /// Das Encoding für Texte.
        /// </summary>
        protected Encoding encoding;

        /// <summary>
        /// Passwort für Verschlüsselung.
        /// </summary>
        protected string password;

        /// <summary>
        /// Hauptthread, der auf eingehende Nachrichten lauscht.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// Der Endpunkt
        /// </summary>
        protected IPEndPoint ipEndPoint;

        /// <summary>
        /// Debugging
        /// </summary>
        protected bool enableDebuging;

        /// <summary>
        /// Nachrichten-Einstellungen
        /// </summary>
        protected NetworkMessageOptions messageOptions;

        /// <summary>
        /// Nachrichten-Datentyp
        /// </summary>
        protected enum MessageDataType : byte
        {
            /// <summary>
            /// Rohdaten
            /// </summary>
            Bytes,

            /// <summary>
            /// Formatierte Nachricht
            /// </summary>
            FormatedMessage
        }

        /// <summary>
        /// Nachrichten-Einstellungen
        /// </summary>
        [Flags]
        protected enum NetworkMessageOptions : byte
        {
            /// <summary>
            /// Standard
            /// </summary>
            None = 0,

            /// <summary>
            /// Verschlüsselt
            /// </summary>
            Cliphered = 1,

            /// <summary>
            /// Komprimiert
            /// </summary>
            Compressed = 2
        }

        #endregion

        #region Properties

        /// <summary>
        /// Liefert die Zeichenkodierung für eingehende und ausgehende Nachrichten, oder legt diese fest.
        /// </summary>
        public Encoding Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Liefert die Menge der gesamten Bytes die empfangen wurden.
        /// </summary>
        public long TotalReceivedBytes
        {
            get { return this.mainSocket.ReceivedBytes; }
        }

        /// <summary>
        /// Liefert die Menge der gesamten Bytes die gesendet wurden.
        /// </summary>
        public long TotalSendedBytes
        {
            get { return this.mainSocket.SendedBytes; }
        }

        /// <summary>
        /// Liefert einen Wert, der angibt, ob Debugmeldungen eingeschaltet sind.
        /// </summary>
        public bool Debuging
        {
            get { return this.enableDebuging; }
            set { this.enableDebuging = value; }
        }

        /// <summary>
        /// Liefert die IPAdresse.
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                if (ipEndPoint != null)
                {
                    return ipEndPoint.Address;
                }

                return null;
            }
        }

        /// <summary>
        /// Liefert den Port.
        /// </summary>
        public int Port
        {
            get
            {
                if (ipEndPoint != null)
                {
                    return ipEndPoint.Port;
                }

                return -1;
            }
        }

        /// <summary>
        /// Legt das Passwort zum verschlüsseln und entschlüsseln von Nachrichten fest.
        /// </summary>
        public string ClipheringKey
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Liefert einen Wert der angibt ob Nachrichten verschlüsselt werden, oder legt diesen fest.
        /// </summary>
        public bool Cliphering
        {
            get
            {
                return ((messageOptions & NetworkMessageOptions.Cliphered) == NetworkMessageOptions.Cliphered); // .HasFlag(NetworkMessageOptions.Cliphered);
            }

            set
            {
                if (value)
                {
                    messageOptions |= NetworkMessageOptions.Cliphered;
                }
                else
                {
                    messageOptions &= ~NetworkMessageOptions.Cliphered;
                }
            }
        }

        /// <summary>
        /// Liefert einen Wert, der angibt, ob Nachrichten komprimiert werden, oder legt diesen fest.
        /// </summary>
        public bool Compressing
        {
            get
            {
                return ((messageOptions & NetworkMessageOptions.Compressed) == NetworkMessageOptions.Compressed); // .HasFlag(NetworkMessageOptions.Compressed);
            }

            set
            {
                if (value)
                {
                    messageOptions |= NetworkMessageOptions.Compressed;
                }
                else
                {
                    messageOptions &= ~NetworkMessageOptions.Compressed;
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Fügt die Nachricht in der Warteschlange hinzu, und signalisiert das eine neue Nachricht eingetroffen ist.
        /// </summary>
        /// <param name="message">Netzwerknachricht, die hinzugefügt wird.</param>
        protected void EnqueueMessage(NetworkMessage message)
        {
            if (message != null)
            {
                lock (messageLock)
                {
                    messages.Enqueue(message);
                    Monitor.Pulse(messageLock);
                }
            }
        }

        /// <summary>
        /// Wartet auf die nächste eintreffende Nachricht und gibt diese zurück.
        /// </summary>
        /// <returns>Die eingetroffene Nachricht</returns>
        public NetworkMessage GetMessage()
        {
            lock (messageLock)
            {
                while (messages.Count == 0)
                {
                    Monitor.Wait(messageLock);
                }

                return messages.Dequeue();
            }
        }

        /// <summary>
        /// Liefert die nächste eintreffende Nachricht.
        /// </summary>
        /// <returns>Liefert eine Nachricht, oder null falls keine vorhanden ist.</returns>
        public NetworkMessage GetAsyncMessage()
        {
            lock (messageLock)
            {
                if (messages.Count == 0)
                {
                    return null;
                }

                return messages.Dequeue();
            }
        }

        /// <summary>
        /// Wartet auf eingehende Daten von dem angegebenen Socket, bis das Array vollkommen gefüllt ist.
        /// </summary>
        /// <param name="socket">Das Socket, von dem empfangen wird.</param>
        /// <param name="buffer">Das Byte-Array, das vollständig befüllt werden soll.</param>
        protected bool FillBuffer(NetworkSocket socket, byte[] buffer)
        {
            if (socket == null || socket.Socket.Connected == false || buffer == null || buffer.Length == 0)
            {
                return false;
            }

            int receivedTotalBytes = 0;
            do
            {
                try
                {
                    int receivedBytes = socket.Socket.Receive(buffer, receivedTotalBytes, 1, SocketFlags.None);
                    if (receivedBytes == 0)
                    {
                        return false;
                    }

                    receivedTotalBytes += receivedBytes;
                }
                catch (SocketException)
                {
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }

            } while (receivedTotalBytes < buffer.Length);

            socket.ReceivedBytes += receivedTotalBytes;

            return true;
        }

        /// <summary>
        /// Liefert die nächste verfügbare Nachricht, oder null falls die Verbindung abbricht.
        /// </summary>
        /// <param name="socket">Der Socket, von dem die Nachricht erwartet wird.</param>
        protected NetworkMessage GetNextMessage(NetworkSocket socket)
        {
            /* Format:
             * Identifikation:  byte, byte
             * Datentyp:        byte
             * Options:         byte [Flags]
             * Datenlänge:      ushort
             * Daten:           byte[Datenlänge]
             * Datenbegrenzung: byte
             */

            if (socket.Socket == null || !socket.Socket.Connected)
            {
                return null;
            }

            ushort dataLength;
            byte[] dataIdentify = new byte[2];
            byte[] dataLengthBuffer = new byte[sizeof(ushort)];
            byte[] dataTypeBuffer = new byte[1];
            byte[] dataOptions = new byte[1];
            byte[] dataEndtagBuffer = new byte[1];
            byte[] dataBuffer = null;
            MessageDataType dataType;
            NetworkMessageOptions options;

            while (true)
            {
                //Identifikationsmarkierung lesen
                if (!FillBuffer(socket, dataIdentify))
                {
                    return null;
                }

                //Identifikationsmarkierung überprüfen
                for (int i = 0; i < identifiy.Length; i++)
                {
                    if (identifiy[i] != dataIdentify[i])
                    {
                        continue;
                    }
                }

                //Datentyp auslesen
                if (!FillBuffer(socket, dataTypeBuffer))
                {
                    return null;
                }
                dataType = (MessageDataType)dataTypeBuffer[0];

                //Einstellungen auslesen
                if (!FillBuffer(socket, dataOptions))
                {
                    return null;
                }
                options = (NetworkMessageOptions)dataOptions[0];

                //Datenlänge auslesen
                if (!FillBuffer(socket, dataLengthBuffer))
                {
                    return null;
                }

                dataLength = BitConverter.ToUInt16(dataLengthBuffer, 0);

                //Eigentliche Daten auslesen
                if (dataLength > 0 && dataLength <= ushort.MaxValue)
                {
                    dataBuffer = new byte[dataLength];
                    if (!FillBuffer(socket, dataBuffer))
                    {
                        return null;
                    }

                    // Falls komprimiert, die Daten wieder dekomprimieren.
                    if ((options & NetworkMessageOptions.Compressed) == NetworkMessageOptions.Compressed)
                    {
                        dataBuffer = DecompressData(dataBuffer);
                    }
                }

                //Datenbegrenzung auslesen
                if (!FillBuffer(socket, dataEndtagBuffer))
                {
                    return null;
                }

                if (dataEndtagBuffer[0] != 255)
                {
                    continue;
                }

                //Nachricht auswerten
                switch (dataType)
                {
                    case MessageDataType.Bytes:
                        return new NetworkMessage(NetworkMessageType.Message,
                            socket.Socket.Handle.ToInt64(), socket.EndPoint, dataBuffer);

                    case MessageDataType.FormatedMessage:
                        string[] parameter = new string[0];
                        string ex;

                        try
                        {
                            using (BinaryReader reader = new BinaryReader(new MemoryStream(dataBuffer)))
                            {
                                ushort id = reader.ReadUInt16();
                                int paraCount = reader.ReadUInt16();

                                if (paraCount > 0)
                                {
                                    parameter = new string[paraCount];
                                    for (int i = 0; i < paraCount; i++)
                                    {
                                        parameter[i] = reader.ReadString();
                                    }
                                }

                                return new NetworkMessage(NetworkMessageType.FormatedMessage,
                                    socket.Socket.Handle.ToInt64(), socket.EndPoint, dataBuffer, id, parameter);
                            }
                        }
                        catch (EndOfStreamException eos)
                        {
                            ex = eos.Message;
                        }
                        catch (IOException io)
                        {
                            ex = io.Message;
                        }

                        return new NetworkMessage(NetworkMessageType.Exception,
                            0, string.Empty, encoding.GetBytes(string.Format("Invalid Formated Message: {0}", ex)), 0, parameter);
                }
            }
        }

        /// <summary>
        /// Sendet Daten an den angegebenen Socket.
        /// </summary>
        /// <param name="socket">Socket, an dem die Daten gesendet werden.</param>
        /// <param name="type">Typ der Nachricht, die versendet wird.</param>
        /// <param name="data">Daten, die gesendet werden.</param>
        /// <returns></returns>
        protected bool SendData(NetworkSocket socket, MessageDataType type, byte[] data)
        {
            if (socket.Socket == null || !socket.Socket.Connected || data == null || data.Length == 0)
            {
                return false;
            }

            return Send(socket, type, data);
        }

        /// <summary>
        /// Sendet eine formatierte Zeichenkette an den angegebenen Socket.
        /// </summary>
        /// <param name="socket">Socket, an dem die formatierte Zeichenkette gesendet wird.</param>
        /// <param name="id">ID der Nachricht.</param>
        /// <param name="parameter">Beliebige Anzahl an Parametern.</param>
        protected bool SendFormatedMessage(NetworkSocket socket, ushort id, params string[] parameter)
        {
            if (socket.Socket == null || !socket.Socket.Connected || id < 0 || id > 999)
            {
                return false;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(id);
                    writer.Write((ushort)parameter.Length);

                    for (int i = 0; i < parameter.Length; i++)
                    {
                        if (parameter[i] == null)
                        {
                            parameter[i] = string.Empty;
                        }

                        writer.Write(parameter[i]);
                    }

                    return Send(socket, MessageDataType.FormatedMessage, stream.ToArray());
                }
            }
        }

        /// <summary>
        /// Versendet Daten über einen Socket.
        /// </summary>
        private bool Send(NetworkSocket socket, MessageDataType dataType, byte[] data)
        {
            if (socket == null || data == null || data.Length == 0 || data.Length > ushort.MaxValue)
            {
                return false;
            }

            /* Format:
             * Identifikation:  byte, byte
             * Datentyp:        byte
             * Options:         byte
             * Datenlänge:      ushort
             * Daten:           byte[Datenlänge]
             * Datenbegrenzung: byte
             */

            // Komprimieren der Daten
            NetworkMessageOptions options = messageOptions;
            if ((options & NetworkMessageOptions.Compressed) == NetworkMessageOptions.Compressed)
            {
                if (data.Length >= 1000)
                {
                    byte[] compressedData = CompressData(data);
                    if (compressedData.Length > data.Length)
                    {
                        options &= ~NetworkMessageOptions.Compressed;
                    }
                    else
                    {
                        data = compressedData;
                    }
                }
                else
                {
                    options &= ~NetworkMessageOptions.Compressed;
                }
            }

            //Gesamtlänge (Identifikation, Datentyp, Datenlänge, Daten, Endmarkierung
            int fullLength = identifiy.Length + 2 + sizeof(ushort) + data.Length + 1;
            //Längenpuffer
            byte[] lengthBuffer = BitConverter.GetBytes((ushort)data.Length);
            //Nachrichtenpuffer
            byte[] buffer = new byte[fullLength];

            //Identifikationsmarkierung
            identifiy.CopyTo(buffer, 0);

            //Datentyp
            buffer[identifiy.Length] = (byte)dataType;

            //Options
            buffer[identifiy.Length + 1] = (byte)options;

            //Datenlänge
            lengthBuffer.CopyTo(buffer, identifiy.Length + 2);
            //Daten
            data.CopyTo(buffer, identifiy.Length + 2 + lengthBuffer.Length);

            //Endmarkierung
            buffer[buffer.Length - 1] = 255;

            //Statistiken aktualisieren
            socket.SendedBytes += buffer.Length;

            //Daten senden
            socket.Socket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            return true;
        }

        /// <summary>
        /// Komprimiert ein Array.
        /// </summary>
        private byte[] CompressData(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(mem, CompressionMode.Compress, true))
                {
                    zip.Write(data, 0, data.Length);
                }

                return mem.ToArray();
            }
        }

        /// <summary>
        /// Dekomprimiert ein Array.
        /// </summary>
        private byte[] DecompressData(byte[] data)
        {
            using (GZipStream zip = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, false))
            {
                byte[] readBuffer = new byte[1024];

                using (MemoryStream mem = new MemoryStream())
                {
                    int readedBytes;
                    do
                    {
                        readedBytes = zip.Read(readBuffer, 0, readBuffer.Length);
                        if (readedBytes > 0)
                        {
                            mem.Write(readBuffer, 0, readedBytes);
                        }
                    } while (readedBytes > 0);

                    return mem.ToArray();
                }
            }
        }

        #endregion
    }
}