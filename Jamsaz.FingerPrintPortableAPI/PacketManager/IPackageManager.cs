using System;
using System.Threading.Tasks;

namespace Jamsaz.FingerPrintPortableAPI.PacketManager
{
    public interface IPackageManager : IDisposable
    {
        Task<Packet> SendPacket(Packet inputPacket);

        Task WritePacket(Packet inputPacket);
    }
}
