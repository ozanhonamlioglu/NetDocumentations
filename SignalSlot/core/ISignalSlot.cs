using System.Threading.Channels;

namespace SignalSlot.core;

public interface ISignalSlot<T> where T : class
{
    public delegate void Inspector(ChannelReader<T> data);
    public void SendMessage(string uniqueName, T data);
    public void ListenMessage(string uniqueName, Inspector inspector);
    public void CloseChannel(string uniqueName);
}