using System.Threading.Channels;
using SignalSlot.core;
using SignalSlot.core.Models;

namespace SignalSlot.libs;

public class SignalSlot<T> : ISignalSlot<T> where T : class
{
    private List<ChannelModel<T>> _channels = new List<ChannelModel<T>>(); // store channels in list

    public SignalSlot(string uniqueName)
    {
        var channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        });
        _channels.Add(new ChannelModel<T>(uniqueName, channel));
    }

    public async void SendMessage(string uniqueName, T data)
    {
        var model = FindChannel(uniqueName);
        if (model is null)
        {
            throw new Exception("Channel is not found!");
        }
        
        await model.GetChannel.Writer.WriteAsync(item: data);
    }

    public void ListenMessage(string uniqueName, ISignalSlot<T>.Inspector inspector)
    {
        var model = FindChannel(uniqueName);
        if (model is null)
        {
            throw new Exception("Channel is not found!");
        }

        inspector(model.GetChannel.Reader);
    }

    public void CloseChannel(string uniqueName)
    {
        var model = FindChannel(uniqueName);
        if (model is null)
        {
            throw new Exception("Channel is not found!");
        }
        
        model.GetChannel.Writer.Complete();
    }

    private ChannelModel<T>? FindChannel(string id) => _channels.FirstOrDefault(x => x.Id == id);
}