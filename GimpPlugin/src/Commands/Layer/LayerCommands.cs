namespace Loupedeck.GimpPlugin.Commands.Layer
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class LayerNewCommand : PluginDynamicCommand
    {
        public LayerNewCommand() : base("New Layer", "Create a new layer", "Layer") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-layer-new");
        }
    }

    public class LayerDuplicateCommand : PluginDynamicCommand
    {
        public LayerDuplicateCommand() : base("Duplicate Layer", "Duplicate active layer", "Layer") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-layer-duplicate");
        }
    }

    public class LayerMergeDownCommand : PluginDynamicCommand
    {
        public LayerMergeDownCommand() : base("Merge Down", "Merge layer down", "Layer") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-image-merge-down");
        }
    }

    public class LayerFlattenCommand : PluginDynamicCommand
    {
        public LayerFlattenCommand() : base("Flatten Image", "Flatten all layers", "Layer") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-image-flatten");
        }
    }
}
