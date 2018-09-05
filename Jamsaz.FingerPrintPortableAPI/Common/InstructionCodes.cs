namespace Jamsaz.FingerPrintPortableAPI.Common
{
    public static class InstructionCodes
    {
        public const byte VerifyPassword = 0x13;
        public const byte SetPassword = 0x12;
        public const byte SetAddress = 0x15;
        public const byte SetSysParameters = 0x0e;
        public const byte PortControl = 0x17;
        public const byte ReadSysParameter = 0x0f;
        public const byte TemplateNum = 0x1d;
        public const byte GenImage = 0x01;
        public const byte UpImage = 0x0a;
        public const byte DownImage = 0x0b;
        public const byte ImageToTz = 0x02;
        public const byte RegModel = 0x05;
        public const byte UpChar = 0x08;
        public const byte DownChar = 0x09;
        public const byte Store = 0x06;
        public const byte LoadChar = 0x07;
        public const byte DeleteChar = 0x0c;
        public const byte EmptyDataBase = 0x0d;
        public const byte MatchInDatabase = 0x03;
        public const byte SearchInDatabase = 0x04;
        public const byte HighSearchInDatabase = 0x1B;
        public const byte GetRandomCode = 0x01;
        public const byte WriteNotePad = 0x18;
        public const byte ReadNotePad = 0x19;
    }
}
