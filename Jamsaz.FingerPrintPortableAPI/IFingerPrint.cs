using System;
using System.Threading.Tasks;
using Jamsaz.FingerPrintPortableAPI.Common;

namespace Jamsaz.FingerPrintPortableAPI
{
    public interface IFingerPrint : IDisposable
    {
        Task<bool> VerifyPassword(byte[] password);

        void SetPassword(byte[] password);

        void SetModuleAddress(byte[] newAddress);

        void SetSystemParameter(byte parameterNumber, byte parameterContent);

        void SetPort(byte controleCode);

        Task<BasicParameters> ReadSystemParameter();

        Task<int> GetNumberOfFingers();

        Task GenerateImageToTz(byte bufferId);

        Task<byte> GenerateImage();

        Task<byte[]> GetImageFromImageBuffer();

        void InsertImageToImageBuffer(byte[] fingerImage);

        Task GenerateTemplate();

        void InsertTemplateToBuffer(byte bufferId,byte[] template);

        Task<byte[]> GetTemplateFromBuffer(byte bufferId);

        Task StoreTemplate(byte bufferId, byte[] pageNumber);

        Task LoadTemplateByPageId(byte bufferId, byte[] pageNumber);

        Task<bool> DeleteTemplate(byte[] pageId, byte len);

        void EmptyDataBase();

        Task<int> SearchFinger(byte bufferId, byte[] startPageId, byte[] numberOfPages);

        Task Reset();
    }
}
