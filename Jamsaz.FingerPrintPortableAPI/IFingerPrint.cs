using System;
using System.Threading.Tasks;
using Jamsaz.FingerPrintPortableAPI.Common;

namespace Jamsaz.FingerPrintPortableAPI
{
    public interface IFingerPrint : IDisposable
    {
        Task<bool> VerifyPassword(byte[] password);

        Task SetPassword(byte[] password);

        Task SetModuleAddress(byte[] newAddress);

        Task SetSystemParameter(byte parameterNumber, byte parameterContent);

        Task SetPort(byte controleCode);

        Task<BasicParameters> ReadSystemParameter();

        Task<int> GetNumberOfFingers();

        Task GenerateImageToTz(byte bufferId);

        Task<byte> GenerateImage();

        Task<byte[]> GetImageFromImageBuffer();

        Task InsertImageToImageBuffer(byte[] fingerImage);

        Task GenerateTemplate();

        Task InsertTemplateToBuffer(byte bufferId,byte[] template);

        Task<byte[]> GetTemplateFromBuffer(byte bufferId);

        Task StoreTemplate(byte bufferId, byte[] pageNumber);

        Task LoadTemplateByPageId(byte bufferId, byte[] pageNumber);

        Task<bool> DeleteTemplate(byte[] pageId, byte len);

        Task EmptyDataBase();

        Task<int> SearchFinger(byte bufferId, byte[] startPageId, byte[] numberOfPages);

        Task Reset();
    }
}
