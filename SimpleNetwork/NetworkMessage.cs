using System;
using System.Net;

namespace SimpleNetwork
{
    /// <summary>
    /// Stellt eine Netzwerknachricht dar.
    /// </summary>
    public class NetworkMessage
    {
        private NetworkMessageType type;
        private long client;
        private byte[] buffer;
        private ushort id;
        private string[] parameter;
        private DateTime receiveDataTime;
        private string endPoint;

        /// <summary>
        /// Stellt eine Netzwerknachricht dar.
        /// </summary>
        /// <param name="type">Typ der Nachricht.</param>
        /// <param name="client">Client, der das Ereigniss gesendet hat.</param>
        /// <param name="endPoint">Der Endpunkt, von dem die Nachricht stammt.</param>
        /// <param name="buffer">Byte-Array, das die gesendeten Daten enthält.</param>
        public NetworkMessage(NetworkMessageType type, long client, string endPoint, byte[] buffer)
        {
            this.type = type;
            this.client = client;
            this.endPoint = endPoint == null ? string.Empty : endPoint;
            this.buffer = buffer == null ? new byte[0] : buffer;
            this.parameter = new string[0];
            this.receiveDataTime = DateTime.Now;
        }

        /// <summary>
        /// Stellt eine Netzwerknachricht dar.
        /// </summary>
        /// <param name="type">Typ der Nachricht.</param>
        /// <param name="client">Client, der das Ereigniss gesendet hat.</param>
        /// <param name="buffer">Byte-Array, das die gesendeten Daten enthält.</param>
        /// <param name="id">ID (0 - 999) der formatierten Nachricht.</param>
        /// <param name="endPoint">Der Endpunkt, von dem die Nachricht stammt.</param>
        /// <param name="parameter">Parameter der formatierten Nachricht.</param>
        public NetworkMessage(NetworkMessageType type, long client, string endPoint, byte[] buffer, ushort id, string[] parameter)
            : this(type, client, endPoint, buffer)
        {
            this.id = id;
            this.parameter = parameter == null ? parameter = new string[0] : parameter;
        }

        /// <summary>
        /// Liefert den Typ, der Nachricht.
        /// </summary>
        public NetworkMessageType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Liefert den Client, der die Nachricht versendet hat.
        /// </summary>
        public long Client
        {
            get { return this.client; }
        }

        /// <summary>
        /// Liefert die Daten der Nachricht.
        /// </summary>
        public byte[] Buffer
        {
            get { return this.buffer; }
        }

        /// <summary>
        /// Liefert die ID einer formatierten Nachricht.
        /// </summary>
        public ushort ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Liefert die Parameter einer formatierten Nachricht.
        /// </summary>
        public string[] Parameter
        {
            get { return this.parameter; }
        }

        /// <summary>
        /// Liefert das Datum sowie die Zeit, als die Nachricht empfangen wurde.
        /// </summary>
        public DateTime ReceiveDateTime
        {
            get { return this.receiveDataTime; }
        }

        /// <summary>
        /// Liefert den Endpunkt, des Clients, der die Nachricht gesendet hat.
        /// </summary>
        public string EndPoint
        {
            get { return this.endPoint; }
        }
    }
}