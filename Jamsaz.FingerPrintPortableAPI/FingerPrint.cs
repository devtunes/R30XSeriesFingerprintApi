using System;
using System.Threading.Tasks;
using Jamsaz.FingerPrintPortableAPI.Common;
using Jamsaz.FingerPrintPortableAPI.PacketManager;

namespace Jamsaz.FingerPrintPortableAPI
{
    public class FingerPrint : IFingerPrint
    {
        private readonly byte[] _deviceAddress;
        private readonly IPackageManager _packageManager;

        public FingerPrint(byte[] deviceAddress, IPackageManager packageManager)
        {
            _deviceAddress = deviceAddress;
            _packageManager = packageManager;
        }

        /// <summary>
        /// Validate your password with the FingerPrint device password. Default password of R305 finger print sensor is 0(0x00000000).
        /// </summary>
        /// <param name="password">your password as a byte array with 4 byte length. For Example password = new byte[]{ 0x00,0x00,0x00,0x00 }</param>
        /// <returns>If password is valid return true</returns>
        public async Task<bool> VerifyPassword(byte[] password)
        {
            try
            {
                var packet = new Packet(_deviceAddress, password, PacketSizes.VerifyPasswordLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x07);
                packet.Write(InstructionCodes.VerifyPassword);
                packet.Write(password);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                return result.Read(9) == ReturnCode.Ok;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_VerifyPassword]", ex.InnerException);
            }
        }

        /// <summary>
        /// Change password of your device.
        /// </summary>
        /// <param name="password">your new password</param>
        public async Task SetPassword(byte[] password)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SetPasswordLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x07);
                packet.Write(InstructionCodes.SetPassword);
                packet.Write(password);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) != ReturnCode.Ok) throw new Exception("[Error] when receiving package -->[FingerPrint_SetPassword]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SetPassword]", ex.InnerException);
            }
        }

        /// <summary>
        /// Change address of the device. Default address of R305 finger print is 0xffffffff.
        /// </summary>
        /// <param name="newAddress">Address shuold as a byte array with 4 byte length. For Example newAddress = new byte[]{0xff,0xff,0xff,0xff}</param>
        public async Task SetModuleAddress(byte[] newAddress)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SetModuleAddressLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x07);
                packet.Write(InstructionCodes.SetAddress);
                packet.Write(newAddress);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) != ReturnCode.Ok) throw new Exception("[Error] when receiving package -->[FingerPrint_SetModuleAddress]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SetPassword]", ex.InnerException);
            }
        }

        /// <summary>
        ///  Operation parameter settings
        /// </summary>
        /// <param name="parameterNumber">4,5,6</param>
        /// <param name="parameterContent">Content of the parameter</param>
        public async Task SetSystemParameter(byte parameterNumber, byte parameterContent)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SetSystemParameterLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x05);
                packet.Write(InstructionCodes.SetSysParameters);
                packet.Write(parameterNumber);
                packet.Write(parameterContent);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.WrongRegisterNumber
                    ? "[Error] wrong register number"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_SetModuleAddress]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SetModuleAddress]", ex.InnerException);
            }
        }

        /// <summary>
        /// For UART protocol, it control the "on/off" of USB port;  For USB protocol, it control the "on/off" of UART port
        /// </summary>
        /// <param name="controleCode">Control code "0" means turns off the port;  Control code "1" means turns on the port</param>
        public async Task SetPort(byte controleCode)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SetPortLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x04);
                packet.Write(InstructionCodes.PortControl);
                packet.Write(controleCode);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.PortOpenFailed
                    ? "[Error] fail to operate the communication port"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_SetPort]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SetPort]", ex.InnerException);
            }
        }

        /// <summary>
        /// Get device status register and system basic configuration parameters.
        /// </summary>
        /// <returns>Return a BasicParameters object that consist system parameters and registers</returns>
        public async Task<BasicParameters> ReadSystemParameter()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.ReadSystemParameterLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.ReadSysParameter);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                var resultOfMethod = new BasicParameters
                {
                    StatusRegister = TypeConverter.ToUnInt16(result.ReadBytes(10, 2)),
                    SystemIdentifierCode = TypeConverter.ToUnInt16(result.ReadBytes(12, 2)).ToString(),
                    FingerLibrarySize = TypeConverter.ToUnInt16(result.ReadBytes(14, 2)),
                    DeviceAddress = TypeConverter.ToUnInt32(result.ReadBytes(18, 4)).ToString(),
                    DevicePacketSize = TypeConverter.ToUnInt16(result.ReadBytes(22, 2)),
                    BaudSetting = TypeConverter.ToUnInt16(result.ReadBytes(24, 2))
                };

                if (result.Read(9) == ReturnCode.Ok) return resultOfMethod;
                throw new Exception("[Error] when receiving package -->[FingerPrint_ReadSystemParameter]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_ReadSystemParameter]", ex.InnerException);
            }
        }

        /// <summary>
        /// Get number of finger which is registered in device.
        /// </summary>
        /// <returns>Number of finger template</returns>
        public async Task<int> GetNumberOfFingers()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.ReadNumberOfFingerLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.TemplateNum);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return TypeConverter.ToUnInt16(result.ReadBytes(10, 2));
                throw new Exception("[Error] when receiving package -->[FingerPrint_GetNumberOfFingers]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GetNumberOfFingers]", ex.InnerException);
            }
        }

        /// <summary>
        /// To generate character file from the original finger image in ImageBuffer and store the file in CharBuffer1 or CharBuffer2
        /// </summary>
        /// <param name="bufferId">BufferID of CharBuffer1 and CharBuffer2 are 1h and 2h respectively</param>
        public async Task GenerateImageToTz(byte bufferId)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.Img2TzLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x04);
                packet.Write(InstructionCodes.ImageToTz);
                packet.Write(bufferId);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.FingerPrintDisOrder
                    ? "[Error] fail to generate character file due to the over-disorderly fingerprint image"
                    : result.Read(9) == ReturnCode.LittleFeature
                        ? "[Error] fail to generate character file due to lackness of character point or over-smallness of fingerprint image"
                        : result.Read(9) == ReturnCode.InvalidImage
                            ? "[Error] fail to generate the image for the lackness of valid primary image"
                            : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_Enroll]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GenerateImageToTz]", ex.InnerException);
            }
        }

        /// <summary>
        /// Detecting finger and store the detected finger image in ImageBuffer while returning successfull confirmation code
        /// </summary>
        /// <returns>if detect finger return true</returns>
        public async Task<byte> GenerateImage()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.GenerateImageLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.GenImage);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                return result.Read(9);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GenerateImage]", ex.InnerException);
            }
        }

        /// <summary>
        /// To get the image in image buffer to host computer.
        /// </summary>
        /// <returns>Get a byte array of finger which is in image buffe</returns>
        public async Task<byte[]> GetImageFromImageBuffer()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.GetImageFromImageBufferLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.UpImage);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok)
                {
                    var recivedImage = result.ReadAll();
                    if (recivedImage.Length <= 12) return new byte[0];
                    var resultArray = new byte[recivedImage.Length - 12];
                    for (int index = 12, i = 0; index < recivedImage.Length - 12; index++, i++)
                    {
                        resultArray[i] = recivedImage[index];
                    }
                    return recivedImage;
                }
                var error = result.Read(9) == ReturnCode.UpImageError
                    ? "[Error]  fail to transfer the following data packet"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_GetImageFromImageBuffer]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GetImageFromImageBuffer]", ex.InnerException);
            }
        }

        /// <summary>
        /// To get the image from host computer.
        /// </summary>
        /// <param name="fingerImage">byte array of a valid finger image</param>
        public async Task InsertImageToImageBuffer(byte[] fingerImage)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.InsertImageToImageBufferLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.DownImage);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok)
                {
                    var imagePacket = new Packet(fingerImage);
                    var sendingResult = await _packageManager.SendPacket(imagePacket);
                    if (sendingResult.Read(9) == ReturnCode.Ok) return;
                }
                var error = result.Read(9) == ReturnCode.ReciveError
                    ? "[Error]  fail to transfer the following data packet"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_InsertImageToImageBuffer]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_InsertImageToImageBuffer]", ex.InnerException);
            }
        }

        /// <summary>
        ///  To combine information of character files from CharBuffer1 and CharBuffer2 and generate a template 
        ///  which is stroed back in both CharBuffer1 and CharBuffer2
        /// </summary>
        public async Task GenerateTemplate()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.GenerateTemplateLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.RegModel);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.MergError
                    ? "[Error] fail to combine the character files. That’s, the character files don’t belong to one finger"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_GenerateTemplate]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GenerateTemplate]", ex.InnerException);
            }
        }

        /// <summary>
        /// To insert template character to specefic buffer.
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        /// <param name="template">Byte arry to transfer template to one of the buffers</param>
        public async Task InsertTemplateToBuffer(byte bufferId, byte[] template)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.InsertTemplateToBufferLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x04);
                packet.Write(InstructionCodes.DownChar);
                packet.Write(bufferId);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok)
                {
                    var templatePacket = new Packet(template);
                    await _packageManager.WritePacket(templatePacket);
                    return;
                }
                var error = result.Read(9) == ReturnCode.ReciveError
                    ? "[Error] fail to receive the following data packages"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_InsertTemplateToBuffer]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_InsertTemplateToBuffer]", ex.InnerException);
            }
        }

        /// <summary>
        /// To get template character from specefic buffer.
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        public async Task<byte[]> GetTemplateFromBuffer(byte bufferId)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.GetTemplateToBufferLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x04);
                packet.Write(InstructionCodes.UpChar);
                packet.Write(bufferId);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok)
                {
                    var recivedImage = result.ReadAll();
                    if (recivedImage.Length <= 12) return new byte[0];
                    var resultArray = new byte[recivedImage.Length - 12];
                    for (int index = 12, i = 0; index < recivedImage.Length - 12; index++, i++)
                    {
                        resultArray[i] = recivedImage[index];
                    }
                    return recivedImage;
                }
                var error = result.Read(9) == ReturnCode.ReciveError
                    ? "[Error] fail to receive the following data packages"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_GetTemplateFromBuffer]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_GetTemplateFromBuffer]", ex.InnerException);
            }
        }

        /// <summary>
        ///  To store the template of specified buffer (Buffer1/Buffer2) at the designated location of Flash library
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        /// <param name="pageNumber">Flash location of the template, two bytes with high byte front and low byte behind.</param>
        public async Task StoreTemplate(byte bufferId, byte[] pageNumber)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.StoreTemplateLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x06);
                packet.Write(InstructionCodes.Store);
                packet.Write(bufferId);
                packet.Write(pageNumber);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.AddressOver
                    ? "[Error] addressing PageID is beyond the finger library"
                    : result.Read(9) == ReturnCode.WritingFlashError ? "error when writing Flash" : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_StoreTemplate]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_StoreTemplate]", ex.InnerException);
            }
        }

        /// <summary>
        /// To load template at the specified location (PageID) of Flash library to template buffer CharBuffer1/CharBuffer2 
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        /// <param name="pageNumber">Flash location of the template, two bytes with high byte front and low byte behind.</param>
        public async Task LoadTemplateByPageId(byte bufferId, byte[] pageNumber)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.LoadTemplateByPageIdLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x06);
                packet.Write(InstructionCodes.LoadChar);
                packet.Write(bufferId);
                packet.Write(pageNumber);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.ReadError
                    ? "[Error]  error when reading template from library or the readout template is invalid"
                    : result.Read(9) == ReturnCode.AddressOver ? " addressing PageID is beyond the finger library" : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_LoadTemplateByPageId]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_LoadTemplateByPageId]", ex.InnerException);
            }
        }

        /// <summary>
        /// Eliminate a record of the fingers from the memory.
        /// </summary>
        /// <param name="pageId">Flash location of the template, two bytes with high byte front and low byte behind.</param>
        /// <param name="len">Number of templates to be deleted</param>
        /// <returns>Return true if the record remove successfuly</returns>
        public async Task<bool> DeleteTemplate(byte[] pageId, byte len)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.DeleteTemplateLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x07);
                packet.Write(InstructionCodes.DeleteChar);
                packet.Write(pageId);
                packet.Write(len);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return true;
                var error = result.Read(9) == ReturnCode.DeleteTempError
                    ? "[Error] faile to delete templates"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_DeleteTemplate]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_DeleteTemplate]", ex.InnerException);
            }
        }

        /// <summary>
        /// Remove all record of fingers from the memory.
        /// </summary>
        public async Task EmptyDataBase()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.EmptyDataBaseLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(InstructionCodes.EmptyDataBase);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.ClearTempError
                    ? "[Error] fail to clear finger library"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_EmptyDataBase]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_EmptyDataBase]", ex.InnerException);
            }
        }

        /// <summary>
        /// To search the whole finger library for the template that matches the one in CharBuffer1 or CharBuffer2. When found, PageID will be returned
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        /// <param name="startPageId">Start flash location</param>
        /// <param name="numberOfPages">Searching numbers</param>
        /// <returns>If the finger is found return pageId</returns>
        public async Task<int> SearchFinger(byte bufferId, byte[] startPageId, byte[] numberOfPages)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SearchFingerLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x08);
                packet.Write(InstructionCodes.SearchInDatabase);
                packet.Write(bufferId);
                packet.Write(startPageId);
                packet.Write(numberOfPages);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) // Return Page id Is 2 Byte
                {
                    var resultByte = new byte[] { result.Read(11), result.Read(10), 0, 0 };
                    return BitConverter.ToInt32(resultByte, 0);
                }
                var error = result.Read(9) == ReturnCode.NotSearched
                    ? "[Error] No matching in the library (both the PageID and matching score are 0);"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_SearchFinger]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SearchFinger]", ex.InnerException);
            }
        }
 /// <summary>
        /// To search the whole finger library for the template that matches the one in CharBuffer1 or CharBuffer2. When found, PageID will be returned
        /// </summary>
        /// <param name="bufferId">BufferId of charbuffer1 and charbuffer2 are 1 and 2 respectively.</param>
        /// <param name="startPageId">Start flash location</param>
        /// <param name="numberOfPages">Searching numbers</param>
        /// <returns>If the finger is found return pageId</returns>
        public async Task<int> HighSearchFinger(byte bufferId, byte[] startPageId, byte[] numberOfPages)
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.SearchFingerLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x08);
                packet.Write(InstructionCodes.HighSearchInDatabase);
                packet.Write(bufferId);
                packet.Write(startPageId);
                packet.Write(numberOfPages);
                packet.Write(packet.CalculateCheckSum());
                var result = await _packageManager.SendPacket(packet);
                 if (result.Read(9) == ReturnCode.Ok) // Return Page id Is 2 Byte
                {
                    var resultByte = new byte[] { result.Read(11), result.Read(10), 0, 0 };
                    return BitConverter.ToInt32(resultByte, 0);
                }
               var error = result.Read(9) == ReturnCode.NotSearched
                    ? "[Error] No matching in the library (both the PageID and matching score are 0);"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_SearchFinger]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_SearchFinger]", ex.InnerException);
            }
        }
        /// <summary>
        /// To reset the buffers
        /// </summary>
        public async Task Reset()
        {
            try
            {
                var packet = new Packet(_deviceAddress, PacketSizes.EmptyDataBaseLen);
                packet.Write(0x01);
                packet.Write(0x00);
                packet.Write(0x03);
                packet.Write(0x16);
                packet.Write(packet.CalculateCheckSum());

                var result = await _packageManager.SendPacket(packet);

                if (result.Read(9) == ReturnCode.Ok) return;
                var error = result.Read(9) == ReturnCode.ClearTempError
                    ? "[Error] fail to clear finger library"
                    : "[Error] when receiving package";
                throw new Exception($"{error} -->[FingerPrint_EmptyDataBase]");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "-->[FingerPrint_EmptyDataBase]", ex.InnerException);
            }
        }

        public void Dispose()
        {
            _packageManager.Dispose();
        }
    }
}
