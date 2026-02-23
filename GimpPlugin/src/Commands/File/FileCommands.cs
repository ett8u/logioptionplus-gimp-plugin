namespace Loupedeck.GimpPlugin.Commands.File
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class FileOpenCommand : PluginDynamicCommand
    {
        public FileOpenCommand() : base("Open", "Open an image file", "File") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) gimp.Connect();
            gimp.InvokeOperation("gimp-file-open");
        }
    }

    public class FileSaveAsCommand : PluginDynamicCommand
    {
        public FileSaveAsCommand() : base("Save As", "Save image with a new name", "File") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-file-save-as");
        }
    }

    public class FileExportCommand : PluginDynamicCommand
    {
        public FileExportCommand() : base("Export", "Export image", "File") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-file-export");
        }
    }
}
