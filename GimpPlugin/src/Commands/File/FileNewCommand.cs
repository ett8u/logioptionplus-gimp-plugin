namespace Loupedeck.GimpPlugin.Commands.File
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class FileNewCommand : PluginDynamicCommand
    {
        public FileNewCommand() : base()
        {
            this.DisplayName = "New Image";
            this.Description = "Create a new image in GIMP";
            this.GroupName = "File";
        }

        protected override Boolean OnLoad()
        {
            return base.OnLoad();
        }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected())
            {
                gimp.Connect();
            }

            if (gimp.InvokeOperation("gimp-image-new", 1920, 1080, 0))
            {
                PluginLog.Info("Created new image");
            }
        }
    }
}
