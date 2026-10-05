using Rss.TmFramework.Base.Modules;
using Rss.TmFramework.Base.Protocols.Map;
using Rss.TmFramework.Modules;
using Rss.TmFramework.Modules.RkPku;
using System;

namespace CheckingAnotherDevice.Models.Modules;

public class RkPkuModule : MapEthernetModule
{
    private const uint RkBaseAddress = 0x30020000;
    private const uint PkuBaseAddress = 0x30030000;
    private const int PkuModeBaseAddress = 0x30030080;
    private const int ChannelCount = 48;

    public int ModuleId { get; set; }
    public string ModuleName { get; set; }
    public string ConnectionString { get; set; }
    public string MapVersion { get; set; }

    public event Action<uint, ushort, int> PkuReceived;
    public event Action<uint, EPkuMode> PkuModeReceived;

    public RkPkuModule() { }
    public RkPkuModule(int id, string moduleName, string connectionString, string mapVersion)
    {
        ModuleId = id;
        ModuleName = moduleName;
        ConnectionString = connectionString;
        MapVersion = mapVersion;

        WriteRequestReceived += RequestReceived;
    }

    public bool GetPkuChannelMode(uint channelIndex, out EPkuMode mode)
    {
        TryReadMemory(PkuModeBaseAddress + channelIndex, 1, EMapFlagsV5.None, out var data, out var errMessage);
        if (data == null || data.Length != 1)
        {
            mode = EPkuMode.Err;
            return false;
        }

        mode = (EPkuMode)data[0];
        PkuModeReceived?.Invoke(channelIndex + 1, mode);
        return true;
    }
    public void SetPkuChannelMode(uint channelIndex, EPkuMode mode)
    {
        TryWriteMemory(PkuModeBaseAddress + channelIndex, new[] { (byte)mode }, EMapFlagsV5.None, out var message);
    }
    public bool TrySendRk(int indexRk, ushort duration, out string message)
    {
        uint addr = GetRkAddress(indexRk);
        byte[] durationBuffer = BitConverter.GetBytes(duration);

        return TryWriteMemory(addr, durationBuffer, EMapFlagsV5.None, out message);
    }
    /// <summary>
    /// Для их уебанских приборов которые на модулях РК принимают ПКУ.
    /// </summary>
    public void UnsubscribeRequestReceived()
    {
        WriteRequestReceived -= RequestReceived;
    }
    /// <summary>
    /// Для их уебанских приборов которые на модулях РК принимают ПКУ.
    /// Так можно было бы анонимкой написать.
    /// </summary>
    private void RequestReceived(IModule module, MapPacketV5 packet)
    {
        var addr = packet.Address;

        if (addr >= PkuBaseAddress && addr <= GetPkuAddress(ChannelCount - 1))
        {
            if (packet.Data == null || packet.Data.Length != 2)
                return;

            var duration = BitConverter.ToUInt16(packet.Data, 0);

            PkuReceived?.Invoke((addr - PkuBaseAddress) / 2, duration, ModuleId);
        }
    }
    private uint GetRkAddress(int number) => (uint)(RkBaseAddress + number * 2);
    private uint GetPkuAddress(int number) => (uint)(PkuBaseAddress + number * 2);
    private uint GetPkuModeAddress(int number) => (uint)(PkuModeBaseAddress + number);
}

