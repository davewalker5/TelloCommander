using TelloCommander.CommandDictionaries;

namespace TelloCommander.Interfaces
{
    public interface ITelloConnection
    {
        ConnectionType ConnectionType { get; }
        int ReceiveTimeout { get; set; }
        int SendTimeout { get; set; }

        void Close();
        void Connect();
        string SendCommand(string command);
    }
}