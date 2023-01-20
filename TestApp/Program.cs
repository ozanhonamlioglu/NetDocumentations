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
    }
}