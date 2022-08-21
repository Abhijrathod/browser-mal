using System.IO;

namespace BrowserMal.Util
{
    public class RemovableDisks
    {
        private static readonly string DRIVE_LABEL = "BashBunny";

        public static string FindBashBunny()
        {
            var driveList = DriveInfo.GetDrives();

            foreach (DriveInfo drive in driveList)
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    if (drive.VolumeLabel == DRIVE_LABEL)
                        return drive.RootDirectory.FullName;
                }
            }

            return string.Empty;
        }
    }
}
