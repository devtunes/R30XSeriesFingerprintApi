using System;

namespace Jamsaz.FingerPrintPortableAPI.PacketManager
{
    public class Packet
    {
        private readonly byte[] _packet;
        private int _lastIndex;

        public byte[] Header => new byte[] { 0xef, 0x01 };
        public byte[] DeviceAddress { get; set; }
        public byte[] DevicePassword { get; set; }

        public Packet(int packetLen)
        {
            DeviceAddress = new byte[] { 0xff, 0xff, 0xff, 0xff };
            DevicePassword = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            _packet = new byte[packetLen];
            SetPacketDefault();
        }

        public Packet(byte[] deviceAddress, byte[] devicePassword, int packetLen)
        {
            DeviceAddress = deviceAddress;
            DevicePassword = devicePassword;
            _packet = new byte[packetLen];
            SetPacketDefault();
        }

        public Packet(byte[] deviceAddress, int packetLen)
        {
            DeviceAddress = deviceAddress;
            DevicePassword = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            _packet = new byte[packetLen];
            SetPacketDefault();
        }

        public Packet(byte[] packet)
        {
            DevicePassword = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            _packet = packet;
            DeviceAddress = new byte[4];
            DeviceAddress[0] = _packet[2];
            DeviceAddress[1] = _packet[3];
            DeviceAddress[2] = _packet[4];
            DeviceAddress[3] = _packet[5];
        }

        public void Write(byte data)
        {
            if (_lastIndex >= _packet.Length) throw new Exception("Index was out of range -->[Packet_Write(byte data)]");
            _packet[_lastIndex] = data;
            _lastIndex++;
        }

        public void Write(byte[] data)
        {
            for (var index = 0; index < data.Length && _lastIndex < _packet.Length; index++, _lastIndex++)
            {
                _packet[_lastIndex] = data[index];
            }
        }

        public byte Read()
        {
            if (_lastIndex >= _packet.Length) throw new Exception("Index was out of range -->[Packet_Read()]");
            var bit = _packet[_lastIndex];
            _lastIndex++;
            return bit;
        }

        public byte Read(int index)
        {
            if (index >= _packet.Length) throw new Exception("Index was out of range -->[Packet_Read(int index)]");
            return _packet[index];
        }

        public byte[] ReadBytes(int offset, int len)
        {
            var result = new byte[len];
            for (int i = offset, j = 0; j < len; i++, j++)
            {
                result[j] = _packet[i];
            }
            return result;
        }

        public byte[] ReadAll()
        {
            return _packet;
        }

        public byte[] CalculateCheckSum()
        {
            var startIndex = Header.Length + DeviceAddress.Length;
            byte sum = 0;
            unchecked
            {
                for (var i = startIndex; i < _packet.Length; i++)
                {
                    sum += _packet[i];
                }
            }
            return new byte[] { 0x00, sum };
        }

        private void SetPacketDefault()
        {
            _packet[0] = Header[0];
            _packet[1] = Header[1];
            _packet[2] = DeviceAddress[0];
            _packet[3] = DeviceAddress[1];
            _packet[4] = DeviceAddress[2];
            _packet[5] = DeviceAddress[3];
            _lastIndex = 6;
        }
    }
}
