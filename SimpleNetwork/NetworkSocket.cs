using System.Net.Sockets;
using System;

namespace SimpleNetwork
{
    /// <summary>
    /// Stellt einen erweiterten Netzwerksocket dar.
    /// </summary>
    public class NetworkSocket
    {
        private long _receivedBytes;
        private long _sendedBytes;
        private Socket _socket;

        /// <summary>
        /// Stellt einen erweiterten Netzwerksocket dar.
        /// </summary>
        /// <param name="socket">Socket</param>
        public NetworkSocket(Socket socket)
        {
            Socket = socket;
        }

        /// <summary>
        /// Liefert den Socket.
        /// </summary>
        public Socket Socket
        {
            get { return _socket; }
            set
            {
                _receivedBytes = 0;
                _sendedBytes = 0;

                _socket = value;
            }
        }

        /// <summary>
        /// Liefert die Anzahl an Bytes, die dieser Socket empfangen hat.
        /// </summary>
        public long ReceivedBytes
        {
            get
            {
                return _receivedBytes;
            }
            set
            {
                if (value > 0)
                {
                    _receivedBytes = value;
                }
            }
        }

        /// <summary>
        /// Liefert die Anzahl an Bytes, die dieser Socket gesendet hat.
        /// </summary>
        public long SendedBytes
        {
            get
            {
                return _sendedBytes;
            }
            set
            {
                if (value > 0)
                {
                    _sendedBytes = value;
                }
            }
        }

        /// <summary>
        /// Liefert den Endpunkt des Sockets.
        /// </summary>
        public string EndPoint
        {
            get
            {
                if (_socket == null)
                    return string.Empty;
                
                try
                {
                    return _socket.RemoteEndPoint.ToString();
                }
                catch (ObjectDisposedException)
                {
                    return string.Empty;
                }
                catch (SocketException)
                {
                    return string.Empty;
                }
            }
        }
    }
}