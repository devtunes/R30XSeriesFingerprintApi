namespace Jamsaz.FingerPrintPortableAPI.Common
{
    public class BasicParameters
    {
        /// <summary>
        /// Contents of system status register 
        /// </summary>
        public int StatusRegister { get; set; }

        /// <summary>
        /// Fixed value: 0x0009 
        /// </summary>
        public string SystemIdentifierCode { get; set; }

        /// <summary>
        /// Finger library size 
        /// </summary>
        public int FingerLibrarySize { get; set; }

        /// <summary>
        /// Security level (1, 2, 3, 4, 5) 
        /// </summary>
        public int SecurityLevel { get; set; }

        /// <summary>
        /// 32-bit device address 
        /// </summary>
        public string DeviceAddress { get; set; }

        /// <summary>
        /// Size code (0, 1, 2, 3)
        /// </summary>
        public int DevicePacketSize { get; set; }

        /// <summary>
        /// N (baud = 9600*N bps)
        /// </summary>
        public int BaudSetting { get; set; }
    }
}
