namespace ConsoleWrapper
{
    class Spamparameter
    {
        public Spamparameter(bool server, string message, int amount)
        {
            this.Message = message;
            this.Amount = amount;
            this.Server = server;
        }

        public bool Server { get; private set; }
        public int Amount { get; private set; }
        public string Message { get; private set; }
    }
}