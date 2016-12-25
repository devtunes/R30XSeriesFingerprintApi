namespace Jamsaz.FingerPrintPortableAPI.Common
{
    public static class ReturnCode
    {
        /// <summary>
        /// Commad execution complete
        /// </summary>
        public const byte Ok = 0x00;

        /// <summary>
        ///  Error when receiving data package
        /// </summary>
        public const byte CommError = 0x01;

        /// <summary>
        ///  No finger on the sensor
        /// </summary>
        public const byte NoFinger = 0x02;

        /// <summary>
        ///  Fail to enroll the finger
        /// </summary>
        public const byte GetImageError = 0x03;

        /// <summary>
        /// Finger print scanner is dry.
        /// </summary>
        public const byte FingerPrintTooDry = 0x04;

        /// <summary>
        /// Finger print scanner is wet.
        /// </summary>
        public const byte FingerPrintTooWet = 0x05;

        /// <summary>
        ///  Fail to generate character file due to the over-disorderly fingerprint image
        /// </summary>
        public const byte FingerPrintDisOrder = 0x06;

        /// <summary>
        /// Fail to generate character file due to lackness of character point or over-smallness of fingerprint image 
        /// </summary>
        public const byte LittleFeature = 0x07;

        /// <summary>
        /// Finger doesn’t match
        /// </summary>
        public const byte NotMatch = 0x08;

        /// <summary>
        ///  Fail to find the matching finger
        /// </summary>
        public const byte NotSearched = 0x09;

        /// <summary>
        /// Fail to combine the character files
        /// </summary>
        public const byte MergError = 0x0a;

        /// <summary>
        /// Addressing PageID is beyond the finger library
        /// </summary>
        public const byte AddressOver = 0x0b;

        /// <summary>
        ///  Error when reading template from library or the template is invalid
        /// </summary>
        public const byte ReadError = 0x0c;

        /// <summary>
        ///  Error when uploading template
        /// </summary>
        public const byte UpTempError = 0x0d;

        /// <summary>
        ///  Module can’t receive the following data packages
        /// </summary>
        public const byte ReciveError = 0x0e;

        /// <summary>
        ///  Error when uploading image
        /// </summary>
        public const byte UpImageError = 0x0f;

        /// <summary>
        ///  Fail to delete the templat
        /// </summary>
        public const byte DeleteTempError = 0x10;

        /// <summary>
        /// Fail to clear finger library
        /// </summary>
        public const byte ClearTempError = 0x11;

        /// <summary>
        ///  Wrong password
        /// </summary>
        public const byte InvalidPassword = 0x13;

        /// <summary>
        ///  Fail to generate the image for the lackness of valid primary image
        /// </summary>
        public const byte InvalidImage = 0x15;

        /// <summary>
        /// Wrong register number
        /// </summary>
        public const byte WrongRegisterNumber = 0x1a;

        /// <summary>
        ///  Fail to operate the communication port
        /// </summary>
        public const byte PortOpenFailed = 0x1d;

        /// <summary>
        /// Error when writing Flash
        /// </summary>
        public const byte WritingFlashError = 0x18;
    }
}
