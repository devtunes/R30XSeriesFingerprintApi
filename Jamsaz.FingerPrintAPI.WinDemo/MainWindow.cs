using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using Jamsaz.FingerPrintAPI.WinDemo.Dialogs;
using Jamsaz.FingerPrintAPI.WinDemo.PackageManager;
using Jamsaz.FingerPrintAPI.WinDemo.Properties;
using Jamsaz.FingerPrintPortableAPI;
using Jamsaz.FingerPrintPortableAPI.Common;

namespace Jamsaz.FingerPrintAPI.WinDemo
{
    public partial class MainWindow : Form
    {
        private IFingerPrint _fingerPrint;
        private bool _isVerified;

        public MainWindow()
        {
            InitializeComponent();
            GetComDevices();
            _isVerified = false;
        }

        private void OpenDevice()
        {
            try
            {
                if (_fingerPrint == null)
                    _fingerPrint = new FingerPrint(new byte[] { 0xff, 0xff, 0xff, 0xff }, new WinAppPackageManager(3));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Form1_OpenDevice_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetComDevices()
        {
            var devices = SerialPort.GetPortNames();
            ComDevicesDropDownList.Items.AddRange(devices);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _fingerPrint?.Dispose();
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenDevice();
                _isVerified = await _fingerPrint.VerifyPassword(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                if (_isVerified)
                {
                    StatusLabel.Text = @"FingerPrinte Device Ready To Work.";
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        private async void EnrollButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_isVerified) return;
                var pageIdDialog = new PageSelectorDialog();
                if (pageIdDialog.ShowDialog() != DialogResult.OK) return;
                var pageId = pageIdDialog.PageId;
                if (!ConfirmDialog(@"Please put your finger on scanner and then select 'OK' from the box")) return;
                await _fingerPrint.Reset();

                var firstImage = 0xff;
                while (firstImage != ReturnCode.Ok)
                {
                    firstImage = await _fingerPrint.GenerateImage();
                }

                await _fingerPrint.GenerateImageToTz(1);

                Thread.Sleep(2000);

                var secoundImage = await _fingerPrint.GenerateImage();
                while (secoundImage != ReturnCode.Ok)
                {
                    secoundImage = await _fingerPrint.GenerateImage();
                }

                await _fingerPrint.GenerateImageToTz(2);

                await _fingerPrint.GenerateTemplate();

                await _fingerPrint.StoreTemplate(1, ConvertIntToByteArray(pageId));
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
        }

        private void OnError(string message)
        {
            MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _fingerPrint = null;
        }

        private byte[] ConvertIntToByteArray(int intValue)
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        private bool ConfirmDialog(string message)
        {
            return MessageBox.Show(message, @"Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) ==
                   DialogResult.OK;
        }
    }
}
