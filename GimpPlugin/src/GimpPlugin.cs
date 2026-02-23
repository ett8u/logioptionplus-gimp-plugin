namespace Loupedeck.GimpPlugin
{
    using System;
    using Loupedeck.GimpPlugin.Interop;

    /// <summary>
    /// GIMP Logitech Plugin - Integrates GIMP 3 with Logitech MX devices
    /// </summary>
    public class GimpPlugin : Plugin
    {
        // This is an application-specific plugin that uses the Application API
        public override Boolean UsesApplicationApiOnly => true;
        public override Boolean HasNoApplication => false;

        public GimpPlugin()
        {
            PluginLog.Init(this.Log);
            PluginResources.Init(this.Assembly);
        }

        public override void Load()
        {
            PluginLog.Info("GIMP Plugin loading...");
            
            // The SDK automatically discovers and loads commands from the assembly
            // No manual registration needed
            
            PluginLog.Info("GIMP Plugin loaded successfully");
        }

        public override void Unload()
        {
            PluginLog.Info("GIMP Plugin unloading...");
            GimpInterop.Instance.Disconnect();
        }
    }
}
