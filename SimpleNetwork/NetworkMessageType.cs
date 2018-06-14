namespace SimpleNetwork
{
    /// <summary>
    /// Stellt den Typ der Nachricht dar.
    /// </summary>
    public enum NetworkMessageType
    {
        /// <summary>
        /// Verbindung wurde aufgebaut. (Server, Client)
        /// </summary>
        Connected,

        /// <summary>
        /// Verbindung fehlgeschlagen (Client)
        /// </summary>
        ConnectionFailed,

        /// <summary>
        /// Verbindung wurde unterbrochen. (Server, Client)
        /// </summary>
        Disconnected,

        /// <summary>
        /// Server wurde gestartet (Server)
        /// </summary>
        ServerStarted,

        /// <summary>
        /// Server konnte nicht gestartet werden (Server)
        /// </summary>
        ServerStartFailed,

        /// <summary>
        /// Server wurde beendet (Server)
        /// </summary>
        ServerStoped,

        /// <summary>
        /// Eingehende Nachricht (Server, Client)
        /// </summary>
        Message,

        /// <summary>
        /// Eingehende formatierte Nachricht (Server, Client)
        /// </summary>
        FormatedMessage,

        /// <summary>
        /// Debugmeldung (Server, Client)
        /// </summary>
        Debug,

        /// <summary>
        /// Fehlermeldung (Server, Client)
        /// </summary>
        Exception,

        /// <summary>
        /// Anfrage zum Beenden der Anwendung (Server, Client)
        /// </summary>
        ExitRequest
    }
}