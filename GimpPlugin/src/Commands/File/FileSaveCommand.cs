namespace Loupedeck.GimpPlugin.Commands.File
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class FileSaveCommand : PluginDynamicCommand
    {
        public FileSaveCommand() : base()
        {
            this.DisplayName = "Save";
            this.Description = "Save the current image";
            this.GroupName = "File";
        }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage())
            {
                PluginLog.Warning("No active image to save");
                return;
            }

            if (gimp.InvokeOperation("gimp-file-save"))
            {
                PluginLog.Info("Image saved");
            }
        }
    }
}
