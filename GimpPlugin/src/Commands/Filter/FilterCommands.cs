namespace Loupedeck.GimpPlugin.Commands.Filter
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class FilterGaussianBlurCommand : PluginDynamicCommand
    {
        public FilterGaussianBlurCommand() : base("Gaussian Blur", "Apply Gaussian blur", "Filters") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("plug-in-gauss", 5.0, 5.0);
        }
    }

    public class FilterSharpenCommand : PluginDynamicCommand
    {
        public FilterSharpenCommand() : base("Sharpen", "Sharpen image", "Filters") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("plug-in-sharpen", 50);
        }
    }

    public class FilterColorBalanceCommand : PluginDynamicCommand
    {
        public FilterColorBalanceCommand() : base("Color Balance", "Adjust color balance", "Filters") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-color-balance");
        }
    }

    public class FilterBrightnessContrastCommand : PluginDynamicCommand
    {
        public FilterBrightnessContrastCommand() : base("Brightness-Contrast", "Adjust brightness and contrast", "Filters") { }

        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected() || !gimp.HasActiveImage()) return;
            gimp.InvokeOperation("gimp-brightness-contrast");
        }
    }
}
