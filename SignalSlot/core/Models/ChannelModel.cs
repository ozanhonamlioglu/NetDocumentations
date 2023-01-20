using System.Threading.Channels;

namespace SignalSlot.core.Models;

public class ChannelModel<T>
{
    public string Id { get; }
    private Channel<T> ChannelObj { get; }

    public ChannelModel(string id, Channel<T> channel)
    {
        Id = id;
        ChannelObj = channel;
    }

    public Channel<T> GetChannel => ChannelObj;
}