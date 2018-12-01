using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexOf.Fetcher
{
    public class Config
    {
        public List<string> AllowExtensions { get; set; }
        public Config()
        {
            AllowExtensions = new List<string>()
            {
                ".mkv", ".mp3", ".mp4", ".mov", ".avi", ".webm"  ,"m2ts"   ,".m4v", ".m4p"  ,".m4v"    ,    "mts"
            };
        }
    }
}
