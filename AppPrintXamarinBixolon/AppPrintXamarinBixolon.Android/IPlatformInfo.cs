using System;
using Android.OS;
using Xamarin.Forms;
using System.Threading.Tasks;
using AppPrintXamarinBixolon.Service;

[assembly: Dependency(typeof(AppPrintXamarinBixolon.Droid.PlatformInfo))]

namespace AppPrintXamarinBixolon.Droid
{
    public class PlatformInfo: IPlatformInfo
    {
        public object AndroidContext { get; set; }
        public string GetAbsolutePath()
        {
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            if(sdCard != null)
                return sdCard.AbsolutePath;
            return "";
        }
        public object GetImgResource()
        {
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeResource((Android.Content.Res.Resources)AndroidResource, Resource.Drawable.bixolon_logo);
            return bitmap;                      
        }
        public async Task<object> GetImgResourceAsync()
        {
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeResource((Android.Content.Res.Resources)AndroidResource, Resource.Drawable.bixolon_logo);
            return bitmap;
        }

        public object AndroidResource { get; set; }


    }
}
