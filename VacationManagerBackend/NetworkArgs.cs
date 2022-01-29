namespace VacationManagerBackend
{
    public class NetworkArgs : EventArgs
    {
        public string Message { get; set; }
        public NetworkArgs(string message) => Message = message;
    }
}