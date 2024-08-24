using TaleKit.Network;
using PhoenixWrapped;
using PhoenixWrapped.Messaging;
using PhoenixWrapped.Messaging.Packet;

namespace TaleKit.Phoenix;

public class PhoenixNetwork : INetwork
{
    private readonly PhoenixClient client;

    public PhoenixNetwork(PhoenixClient client)
    {
        this.client = client;
        this.client.MessageReceived += OnMessageReceived;
    }

    public event Action<string>? PacketSend;
    public event Action<string>? PacketReceived;
    public event Action? Disconnected;

    public void SendPacket(string packet)
    {
        this.client.SendMessage(new PacketSend
        {
            Packet = packet
        });
    }

    public void RecvPacket(string packet)
    {
        this.client.SendMessage(new PacketReceived
        {
            Packet = packet
        });
    }

    public void Disconnect()
    {
        client.Dispose();
        Disconnected?.Invoke();
    }

    private void OnMessageReceived(Message message)
    {
        switch (message)
        {
            case PacketReceived recv:
                PacketReceived?.Invoke(recv.Packet);
                break;
            case PacketSend send:
                PacketSend?.Invoke(send.Packet);
                break;
        }
    }
}