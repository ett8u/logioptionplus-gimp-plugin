namespace Loupedeck.GimpPlugin.Commands.Tool
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    public class ToolBrushCommand : PluginDynamicCommand
    {
        public ToolBrushCommand() : base("Brush Tool", "Activate brush tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-paintbrush-tool");
        }
    }

    public class ToolEraserCommand : PluginDynamicCommand
    {
        public ToolEraserCommand() : base("Eraser Tool", "Activate eraser tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-eraser-tool");
        }
    }

    public class ToolCloneCommand : PluginDynamicCommand
    {
        public ToolCloneCommand() : base("Clone Tool", "Activate clone tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-clone-tool");
        }
    }

    public class ToolSmudgeCommand : PluginDynamicCommand
    {
        public ToolSmudgeCommand() : base("Smudge Tool", "Activate smudge tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-smudge-tool");
        }
    }

    public class ToolBlurCommand : PluginDynamicCommand
    {
        public ToolBlurCommand() : base("Blur Tool", "Activate blur tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-blur-tool");
        }
    }

    public class ToolTextCommand : PluginDynamicCommand
    {
        public ToolTextCommand() : base("Text Tool", "Activate text tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-text-tool");
        }
    }

    public class ToolMoveCommand : PluginDynamicCommand
    {
        public ToolMoveCommand() : base("Move Tool", "Activate move tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-move-tool");
        }
    }

    public class ToolCropCommand : PluginDynamicCommand
    {
        public ToolCropCommand() : base("Crop Tool", "Activate crop tool", "Tools") { }
        protected override void RunCommand(String actionParameter)
        {
            var gimp = GimpInterop.Instance;
            if (!gimp.IsConnected()) return;
            gimp.InvokeOperation("gimp-context-set-tool", "gimp-crop-tool");
        }
    }
}
