using System;
using Xamarin.Forms;
using UIKit;
using System.Threading.Tasks;
using AppPrintXamarinBixolon.Service;

[assembly: Dependency(typeof(AppPrintXamarinBixolon.iOS.PlatformInfo))]

namespace AppPrintXamarinBixolon.iOS
{
    public class PlatformInfo: IPlatformInfo
    {
        public object AndroidContext { get; set; }
        public object AndroidResource { get; set; }
        public string GetAbsolutePath() { return ""; }
        public object GetImgResource()
        {
            UIImage imgResource = UIImage.FromFile("bixolon_logo.bmp");
            if (imgResource == null) return null;
            return imgResource;
        }
        public async Task<object> GetImgResourceAsync()
        {
            UIImage imgResource = UIImage.FromFile("bixolon_logo.bmp");
            if (imgResource == null) return null;
            return imgResource;
        }
    }
}
