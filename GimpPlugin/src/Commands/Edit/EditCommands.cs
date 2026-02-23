namespace Loupedeck.GimpPlugin.Commands.Edit
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class EditCutCommand : PluginDynamicCommand
    {
        public EditCutCommand() : base("Cut", "Cut selection", "Edit") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-edit-cut");
        }
    }

    public class EditCopyCommand : PluginDynamicCommand
    {
        public EditCopyCommand() : base("Copy", "Copy selection", "Edit") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-edit-copy");
        }
    }

    public class EditPasteCommand : PluginDynamicCommand
    {
        public EditPasteCommand() : base("Paste", "Paste clipboard", "Edit") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-edit-paste");
        }
    }
}
