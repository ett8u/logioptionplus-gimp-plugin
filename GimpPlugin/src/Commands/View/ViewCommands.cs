namespace Loupedeck.GimpPlugin.Commands.View
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class ViewZoomInCommand : PluginDynamicCommand
    {
        public ViewZoomInCommand() : base("Zoom In", "Zoom in on image", "View") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-view-zoom-in");
        }
    }

    public class ViewZoomOutCommand : PluginDynamicCommand
    {
        public ViewZoomOutCommand() : base("Zoom Out", "Zoom out on image", "View") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-view-zoom-out");
        }
    }

    public class ViewZoomFitCommand : PluginDynamicCommand
    {
        public ViewZoomFitCommand() : base("Fit in Window", "Fit image in window", "View") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-view-zoom-fit-in");
        }
    }

    public class ViewZoom100Command : PluginDynamicCommand
    {
        public ViewZoom100Command() : base("Zoom 100%", "Set zoom to 100%", "View") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-view-zoom-1-1");
        }
    }
}
