namespace Jamsaz.FingerPrintPortableAPI.Common
{
    public static class PacketSizes
    {
        public const int VerifyPasswordLen = 16;
        public const int SetPasswordLen = 16;
        public const int SetModuleAddressLen = 16;
        public const int SetSystemParameterLen = 14;
        public const int SetPortLen = 13;
        public const int ReadSystemParameterLen = 12;
        public const int ReadNumberOfFingerLen = 12;
        public const int Img2TzLen = 13;
        public const int GenerateImageLen = 12;
        public const int GetImageFromImageBufferLen = 12;
        public const int InsertImageToImageBufferLen = 12;
        public const int GenerateTemplateLen = 12;
        public const int InsertTemplateToBufferLen = 13;
        public const int GetTemplateToBufferLen = 13;
        public const int StoreTemplateLen = 15;
        public const int LoadTemplateByPageIdLen = 15;
        public const int DeleteTemplateLen = 16;
        public const int EmptyDataBaseLen = 12;
        public const int SearchFingerLen = 17;
    }
}
