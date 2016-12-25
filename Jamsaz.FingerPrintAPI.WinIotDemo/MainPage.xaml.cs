using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jamsaz.FingerPrintAPI.WinIotDemo.PackageManager;
using Jamsaz.FingerPrintPortableAPI;
using Jamsaz.FingerPrintPortableAPI.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Jamsaz.FingerPrintAPI.WinIotDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IFingerPrint _fingerPrint;
        private bool _isVerified;

        public MainPage()
        {
            InitializeComponent();
            _isVerified = false;
        }

        private void OpenDevice()
        {
            try
            {
                if (_fingerPrint == null)
                    _fingerPrint = new FingerPrint(new byte[] { 0xff, 0xff, 0xff, 0xff }, new WinIotPakcgeManager());
            }
            catch (Exception ex)
            {
                StatusText.Text = $"{ex.Message}";
            }
        }

        private static byte[] ConvertIntToByteArray(int intValue)
        {
            var intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenDevice();
                _isVerified = await _fingerPrint.VerifyPassword(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                if (_isVerified)
                {
                    StatusText.Text = @"FingerPrinte Device Ready To Work.";
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "System Error please try again";
            }
        }

        private async void EnrollButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Put Your Finger on scanner";
                if (!_isVerified) return;
                var pageId = 10;//int.Parse(PageIdTextBox.Text.Trim());
                //await _fingerPrint.Reset();

                var firstImage = await _fingerPrint.GenerateImage();
                while (firstImage == ReturnCode.NoFinger)
                {
                    firstImage = await _fingerPrint.GenerateImage();
                }

                if (firstImage == ReturnCode.Ok)
                {
                    StatusText.Text = "Put off your finger";
                    await _fingerPrint.GenerateImageToTz(1);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(1000));

                StatusText.Text = "Put your finger again";
                var secoundImage = await _fingerPrint.GenerateImage();
                while (secoundImage == ReturnCode.NoFinger)
                {
                    secoundImage = await _fingerPrint.GenerateImage();
                }

                if (secoundImage == ReturnCode.Ok)
                {
                    await _fingerPrint.GenerateImageToTz(2);
                    StatusText.Text = "Put off your finger";
                }

                await _fingerPrint.GenerateTemplate();

                await _fingerPrint.StoreTemplate(1, ConvertIntToByteArray(pageId));
                StatusText.Text = "Your finger saved";
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
            }
        }

        private async void GetTemplateNumberButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var count = await _fingerPrint.GetNumberOfFingers();
                StatusText.Text = count.ToString();
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
            }
        }

        private async void ReadSystemParameter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var param = await _fingerPrint.ReadSystemParameter();
                StatusText.Text = "Params Get";
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
            }
        }

        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int pageId;
                if (!string.IsNullOrEmpty(CharPageIdTextBox.Text) || !int.TryParse(CharPageIdTextBox.Text, out pageId))
                    return;
                await _fingerPrint.LoadTemplateByPageId(0x01, TypeConverter.ToByteArray(pageId));
                var data = await _fingerPrint.GetTemplateFromBuffer(0x01);
                var fingerData = new byte[data.Length - 12];
                for (int i = 0, j = 12; i < fingerData.Length; i++, j++)
                {
                    fingerData[i] = data[j];
                }
                //File.WriteAllBytes($"C:\\finger{TypeConverter.ToUnInt16(pageId)}.finger", data);
                StatusText.Text = "Data of finger was written on disk";
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
            }
        }
    }
}
