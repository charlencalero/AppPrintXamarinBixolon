using AppPrintXamarinBixolon.Service;
using Plugin.BxlMpXamarinSDK;
using Plugin.BxlMpXamarinSDK.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppPrintXamarinBixolon
{
	public partial class MainPage : ContentPage
	{
        private MPosControllerPrinter _printer;
        private MposConnectionInformation _connectionInfo;
        private static SemaphoreSlim _printSemaphore = new SemaphoreSlim(1, 1);

        public MainPage()
		{
			InitializeComponent();
            conexion();
		}

        private void conexion()
        {
            var connectionInfo = new MposConnectionInformation();

            connectionInfo.BluetoohDeviceId = "SPP-R320";
                connectionInfo.MacAddress = TextAddress.Text;
            connectionInfo.IntefaceType = MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH;
 _connectionInfo = connectionInfo;

            //if (connectionInfo != null)
            //{
            //   

            //    switch (_connectionInfo.IntefaceType)
            //    {
            //        case MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH:
            //        case MPosInterfaceType.MPOS_INTERFACE_WIFI:
            //        case MPosInterfaceType.MPOS_INTERFACE_ETHERNET:
            //            TextAddress.Text = connectionInfo.Address;
            //            btnOpenService.IsEnabled = true;
            //            break;
            //        default:
            //            return;
            //    }
            //}
        }
        async Task<MPosControllerPrinter> OpenPrinterService(MposConnectionInformation connectionInfo)
        {
            if (connectionInfo == null)
                return null;

            if (_printer != null)
                return _printer;

            btnOpenService.IsEnabled = false;

            _printer = MPosDeviceFactory.Current.createDevice(MPosDeviceType.MPOS_DEVICE_PRINTER) as MPosControllerPrinter;

            switch (connectionInfo.IntefaceType)
            {
                case MPosInterfaceType.MPOS_INTERFACE_BLUETOOTH:
                case MPosInterfaceType.MPOS_INTERFACE_WIFI:
                case MPosInterfaceType.MPOS_INTERFACE_ETHERNET:
                    _printer.selectInterface((int)connectionInfo.IntefaceType, connectionInfo.Address);
                    _printer.selectCommandMode((int)MPosCommandMode.MPOS_COMMAND_MODE_BYPASS);
                    break;
                default:
                    await DisplayAlert("Connection Fail", "Not Supported Interface", "OK");
                    return null;
            }

            await _printSemaphore.WaitAsync();

            try
            {
                var result = await _printer.openService();
                if (result != (int)MPosResult.MPOS_SUCCESS)
                {
                    _printer = null;
                    await DisplayAlert("Connection Fail", "openService failed. (" + result.ToString() + ")", "OK");
                }
            }
            finally
            {
                _printSemaphore.Release();
            }

            return _printer;
        }

        async void OnDeviceOpenClicked(object sender, EventArgs e)
        {
            if (_printer == null)
            {
                // Prepares to communicate with the printer 
                _printer = await OpenPrinterService(_connectionInfo);

                if (_printer != null)
                {
                    btnPrintText.IsEnabled = true;
                
                    btnCloseService.IsEnabled = true;
                    btnOpenService.IsEnabled = false;
                }
                else
                {
                    btnPrintText.IsEnabled = false;
              
                    btnCloseService.IsEnabled = false;
                    btnOpenService.IsEnabled = true;
                }
            }
        }
        async void OnDeviceCloseClicked(object sender, EventArgs e)
        {
            await _printSemaphore.WaitAsync();

            if (_printer != null)
            {
                // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                await _printer.closeService();
                _printer = null;
                btnPrintText.IsEnabled = false;
         
                btnCloseService.IsEnabled = false;
                btnOpenService.IsEnabled = true;
            }

            _printSemaphore.Release();
        }

        async void OnPrintTextClicked(object sender, EventArgs e)
        {
            // Prepares to communicate with the printer 
            _printer = await OpenPrinterService(_connectionInfo);

            if (_printer == null)
                return;

            try
            {
                await _printSemaphore.WaitAsync();

                uint textCount = 0;
                string printText = "_Hola Charlen Xamarin\n";

                // note : Page mode and transaction mode cannot be used together between IN and OUT.
                // When "setTransaction" function called with "MPOS_PRINTER_TRANSACTION_IN", print data are stored in the buffer.
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_IN);
                // Printer Setting Initialize
                await _printer.directIO(new byte[] { 0x1b, 0x40 });

                // Code Pages for the contries in east Asia. Please note that the font data downloading is required to print characters for Korean, Japanese and Chinese.
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_KSC5601 });   // Korean
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_SHIFTJIS });  // Japanese
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_GB2312 });    // Simplifies Chinese
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_BIG5 });      // Traditional Chinese
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_FARSI });     // Persian 
                //await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosEastAsiaCodePage.MPOS_CODEPAGE_FARSI_II });  // Persian 

                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { CodePage = (int)MPosCodePage.MPOS_CODEPAGE_WPC1252 });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { FontType = MPosFontType.MPOS_FONT_TYPE_B });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { FontType = MPosFontType.MPOS_FONT_TYPE_C });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Alignment = MPosAlignment.MPOS_ALIGNMENT_CENTER });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Bold = true });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Reverse = true });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Underline = MPosFontUnderline.MPOS_FONT_UNDERLINE_2 });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Height = MPosFontSizeHeight.MPOS_FONT_SIZE_HEIGHT_1 });
                await _printer.printText((textCount++).ToString() + printText, new MPosFontAttribute() { Width = MPosFontSizeWidth.MPOS_FONT_SIZE_WIDTH_1 });

                // Feed to tear-off position (Manual Cutter Position)
                await _printer.directIO(new byte[] { 0x1b, 0x4a, 0xaf });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
            finally
            {
                // Printer starts printing by calling "setTransaction" function with "MPOS_PRINTER_TRANSACTION_OUT"
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_OUT);
                // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                _printSemaphore.Release();
            }
        }


        async void OnPrintImageClicked(object sender, EventArgs e)
        {
            // Prepares to communicate with the printer 
            _printer = await OpenPrinterService(_connectionInfo);

            if (_printer == null)
                return;

            try
            {
                await _printSemaphore.WaitAsync();

                // note : Page mode and transaction mode cannot be used together between IN and OUT.
                // When "setTransaction" function called with "MPOS_PRINTER_TRANSACTION_IN", print data are stored in the buffer.
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_IN);
                // Printer Setting Initialize
                await _printer.directIO(new byte[] { 0x1b, 0x40 });

                await _printer.printBitmap(await DependencyService.Get<IPlatformInfo>().GetImgResourceAsync(),
                            (int)MPosImageWidth.MPOS_IMAGE_WIDTH_ASIS,  // Image Width
                            (int)MPosAlignment.MPOS_ALIGNMENT_CENTER,   // Alignment
                            50,                                         // brightness
                            true,                                       // Image Dithering
                            true);                                      // Image Compress

                await _printer.printText("Bienvenido a Xamarin", new MPosFontAttribute() { Alignment = MPosAlignment.MPOS_ALIGNMENT_CENTER });
                // Feed to tear-off position (Manual Cutter Position)
                await _printer.directIO(new byte[] { 0x1b, 0x4a, 0xaf });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
            finally
            {
                // Printer starts printing by calling "setTransaction" function with "MPOS_PRINTER_TRANSACTION_OUT"
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_OUT);
                // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                _printSemaphore.Release();
            }
        }


        async void OnPrint2dCodeClicked(object sender, EventArgs e)
        {
            // Prepares to communicate with the printer 
            _printer = await OpenPrinterService(_connectionInfo);

            if (_printer == null)
                return;

            try
            {
                await _printSemaphore.WaitAsync();

                // note : Page mode and transaction mode cannot be used together between IN and OUT.
                // When "setTransaction" function called with "MPOS_PRINTER_TRANSACTION_IN", print data are stored in the buffer.
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_IN);
                // Printer Setting Initialize
                await _printer.directIO(new byte[] { 0x1b, 0x40 });
                await _printer.printQRCode("20542471256|01|f001|102.00|12.301|06|1043679394|fhfjhg98987uu",
                                            (int)MPos2dCodeType.MPOS_BARCODE_QRCODE_MODEL2,
                                            (int)MPosAlignment.MPOS_ALIGNMENT_CENTER,
                                            7, // Size
                                            (int)MPosQRCodeECCLevel.MPOS_QRCODE_ECC_LEVEL_Q);

                await _printer.printPDF417("20542471256|01|f001|102.00|12.301|06|1043679394|fhfjhg98987uu",
                                (int)MPos2dCodeType.MPOS_BARCODE_PDF417,
                                (int)MPosAlignment.MPOS_ALIGNMENT_LEFT,
                                0, // number of columns
                                0, // number of rows
                                3, // module Width
                                3, // module Height
                                (int)MPosPDF417ECCLevel.MPOS_PDF417_ECC_LEVEL_0);

                // Feed to tear-off position (Manual Cutter Position)
                await _printer.directIO(new byte[] { 0x1b, 0x4a, 0xaf });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exception", ex.ToString(), "OK");
            }
            finally
            {
                // Printer starts printing by calling "setTransaction" function with "MPOS_PRINTER_TRANSACTION_OUT"
                await _printer.setTransaction((int)MPosTransactionMode.MPOS_PRINTER_TRANSACTION_OUT);
                // If there's nothing to do with the printer, call "closeService" method to disconnect the communication between Host and Printer.
                _printSemaphore.Release();
            }
        }


    }

}
