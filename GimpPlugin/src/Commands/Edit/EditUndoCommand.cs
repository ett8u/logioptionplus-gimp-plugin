namespace Loupedeck.GimpPlugin.Commands.Edit
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class EditUndoCommand : PluginDynamicCommand
    {
        public EditUndoCommand() : base()
        {
            this.DisplayName = "Undo";
            this.Description = "Undo the last action";
            this.GroupName = "Edit";
        }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage())
            {
                return;
            }

            if (gimp.InvokeOperation("gimp-edit-undo"))
            {
                PluginLog.Info("Undo executed");
            }
        }
    }
}
