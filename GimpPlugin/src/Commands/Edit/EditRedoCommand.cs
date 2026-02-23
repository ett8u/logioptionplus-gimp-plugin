namespace Loupedeck.GimpPlugin.Commands.Edit
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class EditRedoCommand : PluginDynamicCommand
    {
        public EditRedoCommand() : base()
        {
            this.DisplayName = "Redo";
            this.Description = "Redo the last undone action";
            this.GroupName = "Edit";
        }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage())
            {
                return;
            }

            if (gimp.InvokeOperation("gimp-edit-redo"))
            {
                PluginLog.Info("Redo executed");
            }
        }
    }
}
