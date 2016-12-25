using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Jamsaz.FingerPrintPortableAPI.PacketManager;

namespace Jamsaz.FingerPrintAPI.WinDemo.PackageManager
{
    public class WinAppPackageManager : IPackageManager
    {
        private SerialPort _port;
        private readonly AutoResetEvent _packetRecived;
        private byte[] _currentData;

        public WinAppPackageManager(int portNumber)
        {
            try
            {
                _packetRecived = new AutoResetEvent(false);
                if (_port != null && _port.IsOpen)
                    _port.Close();
                _port = new SerialPort
                {
                    BaudRate = 57600,
                    Parity = Parity.None,
                    DataBits = 8,
                    PortName = $"COM{portNumber}",
                    StopBits = StopBits.One,
                };

                _port.DataReceived += PortOnDataReceived;
                _port.Open();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} -->[WinAppPackageManager_Constructor]", ex.InnerException);
            }
        }

        public void Dispose()
        {
            _port.Close();
            _port = null;
        }

        private void PortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var dataLength = _port.BytesToRead;
            var data = new byte[dataLength];
            var nbrDataRead = _port.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;
            _currentData = data;
            _packetRecived.Set();
        }

        public async Task<Packet> SendPacket(Packet inputPacket)
        {
            var packetTowrite = inputPacket.ReadAll();
            _port.Write(packetTowrite, 0, packetTowrite.Length);
            _packetRecived.WaitOne(50000);
            return await Task.Run(() => new Packet(_currentData));
        }
    }
}
