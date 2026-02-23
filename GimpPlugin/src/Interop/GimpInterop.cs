namespace Loupedeck.GimpPlugin.Interop
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Handles communication with GIMP 3 via Script-Fu batch mode
    /// </summary>
    public class GimpInterop
    {
        private static GimpInterop _instance;
        private bool _isConnected = false;
        private string _gimpExecutable;

        public static GimpInterop Instance => _instance ??= new GimpInterop();

        private GimpInterop() 
        {
            FindGimpExecutable();
        }

        private void FindGimpExecutable()
        {
            // Check common GIMP 3 installation paths
            var paths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "GIMP 3", "bin", "gimp-3.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GIMP 3", "bin", "gimp-3.exe"),
                "C:\\Program Files\\GIMP 3\\bin\\gimp-3.exe"
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    _gimpExecutable = path;
                    PluginLog.Info($"Found GIMP at: {path}");
                    return;
                }
            }

            PluginLog.Warning("GIMP 3 executable not found");
        }

        public bool Connect()
        {
            try
            {
                // Check if GIMP is running
                var gimpProcesses = Process.GetProcessesByName("gimp-3");
                if (gimpProcesses.Length == 0)
                {
                    PluginLog.Warning("GIMP 3 process not found");
                    return false;
                }

                _isConnected = true;
                PluginLog.Info("Connected to GIMP 3");
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to connect to GIMP: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _isConnected = false;
                PluginLog.Info("Disconnected from GIMP 3");
            }
        }

        public bool IsConnected() => _isConnected;

        public bool HasActiveImage()
        {
            if (!_isConnected) return false;
            // Assume active image exists if GIMP is running
            return true;
        }

        public int GetActiveImageId()
        {
            if (!_isConnected) return -1;
            return 1;
        }

        public bool HasActiveSelection()
        {
            if (!_isConnected) return false;
            return false;
        }

        public bool InvokeOperation(string operationName, params object[] parameters)
        {
            if (!_isConnected)
            {
                PluginLog.Error($"Cannot invoke {operationName}: Not connected to GIMP");
                return false;
            }

            try
            {
                PluginLog.Info($"Invoking GIMP operation: {operationName}");
                
                // Send keyboard shortcut to GIMP window
                SendKeyboardShortcut(operationName);
                
                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to invoke {operationName}: {ex.Message}");
                return false;
            }
        }

        private void SendKeyboardShortcut(string operation)
        {
            // Map operations to GIMP keyboard shortcuts
            var shortcut = GetShortcutForOperation(operation);
            if (!string.IsNullOrEmpty(shortcut))
            {
                // Use Windows API to send keys to GIMP
                ActivateGimpWindow();
                System.Threading.Thread.Sleep(50);
                SendKeys(shortcut);
            }
        }

        private string GetShortcutForOperation(string operation)
        {
            // Map common operations to GIMP shortcuts
            return operation switch
            {
                "gimp-file-open" => "^o",           // Ctrl+O
                "gimp-file-save" => "^s",           // Ctrl+S
                "gimp-file-save-as" => "^+s",       // Ctrl+Shift+S
                "gimp-file-export" => "^+e",        // Ctrl+Shift+E
                "gimp-edit-undo" => "^z",           // Ctrl+Z
                "gimp-edit-redo" => "^y",           // Ctrl+Y
                "gimp-edit-cut" => "^x",            // Ctrl+X
                "gimp-edit-copy" => "^c",           // Ctrl+C
                "gimp-edit-paste" => "^v",          // Ctrl+V
                "gimp-selection-all" => "^a",       // Ctrl+A
                "gimp-selection-none" => "^+a",     // Ctrl+Shift+A
                "gimp-selection-invert" => "^i",    // Ctrl+I
                "gimp-view-zoom-in" => "+",         // +
                "gimp-view-zoom-out" => "-",        // -
                "gimp-view-zoom-fit-in" => "^+e",   // Ctrl+Shift+E
                "gimp-view-zoom-1-1" => "1",        // 1
                _ => null
            };
        }

        private void ActivateGimpWindow()
        {
            try
            {
                var gimpProcesses = Process.GetProcessesByName("gimp-3");
                if (gimpProcesses.Length > 0)
                {
                    var handle = gimpProcesses[0].MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        NativeMethods.SetForegroundWindow(handle);
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to activate GIMP window: {ex.Message}");
            }
        }

        private void SendKeys(string keys)
        {
            try
            {
                System.Windows.Forms.SendKeys.SendWait(keys);
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Failed to send keys: {ex.Message}");
            }
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
