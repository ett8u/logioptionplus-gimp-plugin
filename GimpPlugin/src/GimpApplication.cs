namespace Loupedeck.GimpPlugin
{
    using System;

    /// <summary>
    /// Links the plugin to GIMP 3 application
    /// </summary>
    public class GimpApplication : ClientApplication
    {
        public GimpApplication()
        {
        }

        // Link to GIMP 3 Windows process
        protected override String GetProcessName() => "gimp-3";

        // Not used on Windows
        protected override String GetBundleName() => "";

        // Check if GIMP 3 is installed
        public override ClientApplicationStatus GetApplicationStatus()
        {
            // Check common installation paths for GIMP 3
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var gimpPath = System.IO.Path.Combine(programFiles, "GIMP 3", "bin", "gimp-3.exe");
            
            if (System.IO.File.Exists(gimpPath))
            {
                return ClientApplicationStatus.Installed;
            }
            
            return ClientApplicationStatus.NotInstalled;
        }
    }
}
