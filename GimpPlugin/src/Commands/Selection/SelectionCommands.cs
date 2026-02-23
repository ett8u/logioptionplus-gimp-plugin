namespace Loupedeck.GimpPlugin.Commands.Selection
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class SelectionAllCommand : PluginDynamicCommand
    {
        public SelectionAllCommand() : base("Select All", "Select entire image", "Selection") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-selection-all");
        }
    }

    public class SelectionNoneCommand : PluginDynamicCommand
    {
        public SelectionNoneCommand() : base("Select None", "Deselect all", "Selection") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-selection-none");
        }
    }

    public class SelectionInvertCommand : PluginDynamicCommand
    {
        public SelectionInvertCommand() : base("Invert Selection", "Invert current selection", "Selection") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-selection-invert");
        }
    }

    public class SelectionFeatherCommand : PluginDynamicCommand
    {
        public SelectionFeatherCommand() : base("Feather Selection", "Feather selection edges", "Selection") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-selection-feather", 5.0);
        }
    }
}
