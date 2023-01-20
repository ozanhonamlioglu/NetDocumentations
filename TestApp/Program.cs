using System.Threading.Channels;
using SignalSlot.libs;

namespace MyNamespace;

internal readonly record struct Coordinates(string DeviceId, double Latitude, double Longitude);

class ProcessStatus
{
    public bool Loading { get; set; } = false;

    public void UpdateStatus()
    {
        Loading = !Loading;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var data = new ProcessStatus();
        var Channel1 = new SignalSlot<ProcessStatus>("channel 1");
        
        Channel1.ListenMessage("channel 1", async (x) =>
        {
            while (await x.WaitToReadAsync())
            {
                while (x.TryRead(out ProcessStatus status))
                {
                    Console.WriteLine($"Status of loading -> {status.Loading}");
                }
            }
        });

        Channel1.SendMessage("channel 1", data);
        
        data.UpdateStatus();
        Channel1.SendMessage("channel 1", data);
        
        data.UpdateStatus();
        Channel1.SendMessage("channel 1", data);
        
        Channel1.CloseChannel("channel 1");

        /*
        var coordinates = new Coordinates(Guid.NewGuid().ToString(), 2,3);
        var channel = Channel.CreateUnbounded<Coordinates>(
            new UnboundedChannelOptions()
            {
                SingleReader = false,
                SingleWriter = false,
                AllowSynchronousContinuations = true
            });
        
        ProduceWithTryWrite(channel.Writer, coordinates);
        ConsumeWithReadAsync(channel.Reader);
        */
    }
    
    static void ProduceWithTryWrite(
        ChannelWriter<Coordinates> writer, Coordinates coordinates)
    {
        while (coordinates is { Latitude: < 90, Longitude: < 180 })
        {
            var tempCoordinates = coordinates with
            {
                Latitude = coordinates.Latitude + .5,
                Longitude = coordinates.Longitude + 1
            };

            if (writer.TryWrite(item: tempCoordinates))
            {
                coordinates = tempCoordinates;
            }
        }
    }
    
    static async ValueTask ProduceWithWriteAsync(
        ChannelWriter<Coordinates> writer, Coordinates coordinates)
    {
        while (coordinates is { Latitude: < 90, Longitude: < 180 })
        {
            await writer.WriteAsync(
                item: coordinates = coordinates with
                {
                    Latitude = coordinates.Latitude + .5,
                    Longitude = coordinates.Longitude + 1
                });
        }

        writer.Complete();
    }
    
    static async ValueTask ConsumeWithReadAsync(
        ChannelReader<Coordinates> reader)
    {
        while (true)
        {
            // May throw ChannelClosedException if
            // the parent channel's writer signals complete.
            Coordinates coordinates = await reader.ReadAsync();
            Console.WriteLine(coordinates);
        }
    }
}