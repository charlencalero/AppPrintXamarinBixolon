using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppPrintXamarinBixolon.Service
{
   public interface IPlatformInfo
    {
        object AndroidContext { get; set; }
        object AndroidResource { get; set; }
        string GetAbsolutePath();   // For Android Only
        object GetImgResource();    // Image Resource Type return
        Task<object> GetImgResourceAsync();

    }
}
