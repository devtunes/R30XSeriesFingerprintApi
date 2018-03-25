using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Jamsaz.FingerPrintPortableAPI.PacketManager;

namespace Jamsaz.FingerPrintAPI.WinIotDemo.PackageManager
{
    public class WinIotPakcgeManager : IPackageManager
    {
        private SerialDevice _serialDevice;
        private DataWriter _dataWriteObject;
        private DataReader _dataReaderObject;
        private readonly CancellationTokenSource _readCancellationTokenSource;
        private byte[] _currentData;
        private readonly AutoResetEvent _packetRecived;
        private List<byte[]> _sequenceDataList;

        public WinIotPakcgeManager()
        {
            try
            {
                _sequenceDataList = new List<byte[]>();
                _packetRecived = new AutoResetEvent(false);
                var deviceSelector = SerialDevice.GetDeviceSelector();
               // var devices = DeviceInformation.FindAllAsync(deviceSelector).AsTask().Result;
                var devices = Task.Run(async () => await DeviceInformation.FindAllAsync(deviceSelector)).Result;
                if (devices.Count <= 0)
                {
                    throw new Exception("Device not found -->[WinIotPakcgeManager_Constructor]");
                }

                var entry = devices.Single(x => x.Name.Equals("CP2102 USB to UART Bridge Controller")); //CP2102 USB to UART Bridge Controller

                _serialDevice = SerialDevice.FromIdAsync(entry.Id).AsTask().Result;

                // Configure serial settings
                _serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(250);
                _serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(250);
                _serialDevice.BaudRate = 57600;
                _serialDevice.Parity = SerialParity.None;
                _serialDevice.StopBits = SerialStopBitCount.One;
                _serialDevice.DataBits = 8;

                // Create cancellation token object to close I/O operations when closing the device
                _readCancellationTokenSource = new CancellationTokenSource();

                ListenToSerialPort();
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} -->[WinIotPakcgeManager_Constructor]", ex.InnerException);
            }
        }

        public void Dispose()
        {
            _serialDevice = null;
        }

        public async Task<Packet> SendPacket(Packet inputPacket)
        {
            var packetTowrite = inputPacket.ReadAll();
            _dataWriteObject = new DataWriter(_serialDevice.OutputStream);
            _dataWriteObject.WriteBytes(packetTowrite);
            var storeAsyncTask = _dataWriteObject.StoreAsync().AsTask();
            var bytesWritten = await storeAsyncTask;
            if (bytesWritten <= 0) return await Task.Run(() => new Packet(0));
            return await Task.Run(() =>
            {
                _packetRecived.WaitOne();
                var retPacket = new Packet(_currentData);
                return retPacket;
            });
        }

        public async Task WritePacket(Packet inputPacket)
        {
            var packetTowrite = inputPacket.ReadAll();
            _dataWriteObject = new DataWriter(_serialDevice.OutputStream);
            _dataWriteObject.WriteBytes(packetTowrite);
            var storeAsyncTask = _dataWriteObject.StoreAsync().AsTask();
            await storeAsyncTask;
        }

        private async void ListenToSerialPort()
        {
            try
            {
                if (_serialDevice == null) return;
                _dataReaderObject = new DataReader(_serialDevice.InputStream);

                while (true)
                {
                    await ReadAsync(_readCancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                CloseSerialPortDevice();
            }
            finally
            {
                // Cleanup once complete
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.DetachStream();
                    _dataReaderObject = null;
                }
            }
        }

        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            const uint readBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            var loadAsyncTask = _dataReaderObject.LoadAsync(readBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            var bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                try
                {
                    var data = new byte[bytesRead];
                    _dataReaderObject.ReadBytes(data);
                    _currentData = data;
                    _packetRecived.Set();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message} -->[WinIotPakcgeManager_ReadingDevice]", ex.InnerException);
                }
                finally
                {
                    if (_dataWriteObject != null)
                    {
                        _dataWriteObject.DetachStream();
                        _dataWriteObject = null;
                    }
                }
            }
        }

        private void CloseSerialPortDevice()
        {
            try
            {
                _serialDevice?.Dispose();
                _serialDevice = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} -->[WinIotPakcgeManager_CloseingDevice]", ex.InnerException);
            }
        }

    }
}
