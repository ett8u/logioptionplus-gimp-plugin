# Design Document: GIMP Logitech Plugin

## Design Changelog

### Version 1.4 (2024-02-23) - Profile System & Marketplace Distribution

**Added:**
- Profile vs plugin distinction section explaining the critical difference between plugins (code + available actions) and profiles (device-specific button/dial mappings)
- Profile creation workflow documentation showing how users customize default profiles in Options+ UI
- Recommended profile layouts for MX Creative Keypad, Dialpad, and Actions Ring
- Marketplace distribution guide with submission checklist and package requirements
- 3 new correctness properties for profile validation:
  - Property 19: Profile installation verification
  - Property 20: Profile activation on GIMP focus
  - Property 21: Default profile preservation on updates

**Changed:**
- Updated package structure to include `profiles/` directory as REQUIRED (not optional)
- Updated plugin installation flow to show default profile auto-installation
- Updated architecture diagram to show profiles as separate from plugin code
- Clarified that profiles are created in Options+ UI, NOT hand-coded

**Rationale:**
Profiles are the critical missing piece for marketplace distribution. Users don't hand-code .lp5 files - they customize the default profiles we provide using the Options+ UI. The plugin package MUST include default profiles for all supported devices, which auto-install when the plugin is installed. This is how users get a working configuration out-of-the-box. Without understanding this distinction, developers might think they need to document how to manually create profiles, which is not the user workflow.

---

### Version 1.3 (2024-02-23) - Sidecar Architecture Clarification

**Added:**
- "The Sidecar Execution Model" section explaining separate process architecture
- Detailed explanation of gRPC communication between Plugin Service and plugin process
- Fault isolation benefits (plugin crashes don't affect Options+ or Plugin Service)
- Architecture components table showing process boundaries

**Changed:**
- Updated architecture diagram to show plugin as separate process (not DLL in Plugin Service)
- Updated all communication flows to explicitly show gRPC protocol
- Updated component descriptions to clarify process isolation
- Updated threading model section to explain SDK-managed threading across process boundary

**Removed:**
- Misleading references to plugin being "loaded" into Plugin Service memory space

**Rationale:**
The original design incorrectly implied the plugin runs as a DLL loaded into the Plugin Service process. In reality, Logitech adopted the Loupedeck architecture where plugins run as separate processes (sidecar model) and communicate via gRPC. This is critical for fault isolation - if the plugin crashes, it doesn't take down Options+ or the Plugin Service. The SDK abstracts the gRPC communication, making it feel like simple method calls, but understanding the process boundary is important for debugging and error handling.

---

### Version 1.2 (2024-02-23) - Configuration Management Simplification

**Removed:**
- ConfigurationManager component and all related code examples
- config.json file format section
- Configuration persistence logic
- Custom configuration storage implementation

**Changed:**
- Simplified component model from 6 components to 4 core components
- Updated "What Plugin Does NOT Implement" section to explicitly list removed functionality
- Updated communication flow to show Options+ handling all configuration persistence
- Clarified that plugin receives configuration via SDK callbacks (over gRPC), not by reading files

**Rationale:**
The original design over-engineered configuration management by creating a custom ConfigurationManager class and config.json persistence layer. This violates the SDK pattern - Logi Options+ handles ALL configuration storage and persistence. The plugin simply receives configuration updates via SDK callbacks (which happen over gRPC in the sidecar model). Adding custom configuration management creates unnecessary complexity and diverges from how the SDK is designed to work. The LibreHardwareMonitor reference plugin confirms this - no custom config managers needed.

---

### Version 1.1 (2024-02-23) - GIMP API Integration Update

**Changed:**
- Replaced GIMP_PDB (Python-Fu Database) references with GObjects introspection
- Updated GimpInterop class to use GObject.Introspection NuGet package
- Updated all code examples to show GObjects API calls instead of PDB calls
- Updated package structure to include GObject.Introspection.dll

**Rationale:**
GIMP 3 uses GObjects as its primary API, not the legacy PDB (Procedure Database). GObjects provides better .NET interop through the GObject.Introspection library, which uses GObject introspection to dynamically discover GIMP's API at runtime. This is more maintainable than hardcoding PDB procedure names and provides better type safety. The introspection approach also means the plugin can adapt to different GIMP versions without recompilation.

---

### Version 1.0 (2024-02-23) - Initial Design

**Added:**
- Initial architecture with Plugin and Application classes
- 18 correctness properties covering core functionality
- Component model with GimpPlugin, GimpApplication, Action Commands, and GimpInterop
- Communication flows for plugin loading, action execution, and configuration
- Testing strategy with property-based testing requirements
- Data models for plugin package structure
- Error handling strategy with SDK logging

**Initial Design Decisions:**
- Chose C# and Logitech Actions SDK for implementation
- Chose GObjects API for GIMP integration
- Defined sidecar architecture with gRPC communication
- Established 50-100ms latency target for action execution
- Set resource limits: <50MB memory, <5% CPU idle

## Overview

The GIMP Logitech Plugin is a C# plugin built using the Logitech Actions SDK that bridges GIMP 3 functionality with Logitech MX devices through the Logi Options+ app. The plugin enables users to trigger GIMP operations via the Action Ring overlay and programmable device buttons, streamlining image editing workflows.

### Plugin vs Profile Distinction

Understanding the difference between plugins and profiles is critical for marketplace distribution:

**PLUGIN** = Package containing:
- List of AVAILABLE ACTIONS (the operations GIMP can perform)
- DEFAULT PROFILE(S) (pre-configured button/dial mappings for supported devices)
- Plugin code that executes the actions

**PROFILE** = Device-specific configuration file (.lp5) containing:
- Mapping of actions to physical buttons/dials for a SPECIFIC DEVICE
- Action settings and parameters
- Custom icons and sounds
- Workspace/page layouts
- Action Ring configuration (8 action slots)
- Application binding (auto-activates when GIMP is in focus)

**Key Workflow:**
1. User installs plugin from marketplace.logi.com
2. Plugin package includes default profiles for all supported devices
3. When installed, default profile automatically installs for user's device
4. Profile automatically activates when GIMP comes into focus (adapt-to-app)
5. User can customize the default profile or create new profiles based on it
6. Plugin updates provide new default profiles as templates (don't overwrite user profiles)

### The "Sidecar" Execution Model

The plugin uses a **sidecar architecture** - it runs as a separate process, NOT as a DLL loaded into the Logi Plugin Service memory space:

**Separate Process Execution:**
- Your C# plugin code is compiled into an executable (or a DLL with a runner)
- The Logi Plugin Service spawns the plugin as a separate process
- The plugin runs in its own isolated memory space

**gRPC Communication:**
"Logitech didn't invent a new way for apps to talk to each other. They adopted the Loupedeck architecture, which was built specifically to allow developers to write plugins in any language. To make that work across different processes safely, they used gRPC. This is why we can see the Grpc.Net.Client libraries in the plugin folder and see the processes 'talking' to each other over local ports rather than just sharing memory."

- Plugin communicates with Logi Plugin Service via gRPC (high-performance Remote Procedure Call framework)
- The C# Actions SDK wraps gRPC protocol, making it feel like simple interface implementation
- All action triggers, configuration updates, and lifecycle events flow through gRPC

**Fault Isolation Benefit:**
- If the plugin crashes, only the plugin process dies
- Logi Options+ and Plugin Service continue running normally
- User's mouse/keyboard functionality remains unaffected
- Plugin Service can restart the plugin process automatically

**Architecture Components:**

| Component | Role |
|-----------|------|
| Logi Options+ | UI where users configure buttons and dials |
| Plugin Service | Broker that routes hardware signals to plugins via gRPC |
| GIMP Plugin | Standalone process that listens for action events via gRPC |

**Why C# SDK?**
The Actions SDK provides a C# wrapper around the gRPC protocol definitions. This abstraction makes plugin development feel like implementing standard interfaces in a library, while the SDK handles all the complex networking, serialization, and process communication under the hood. You write simple C# classes, and the SDK transparently manages the gRPC communication with the Plugin Service.

### Plugin Architecture

The plugin follows the standard Logitech Actions SDK architecture with two core classes:
1. **GimpPlugin**: Main plugin class (inherits from `Plugin` abstract class) - handles plugin lifecycle and action registration
2. **GimpApplication**: Application class (inherits from `ClientApplication` abstract class) - links to GIMP process for foreground detection

Key design principles:
- Plugin runs as a separate process, communicating via gRPC with Plugin Service
- SDK abstracts gRPC communication, making it feel like simple method calls
- Process isolation ensures plugin crashes don't affect Options+ or Plugin Service
- Logi Plugin Service and Options+ handle ALL configuration persistence
- Follow SDK patterns exactly - no over-engineering
- Low latency action execution (50-100ms response time)
- Simple error handling with SDK logging
- Minimal resource footprint (<50MB memory, <5% CPU idle)
- Direct GIMP GObjects API integration via .NET interop

**Reference Implementation:**
The LibreHardwareMonitor plugin is a good reference for this architecture:
- Monitors hardware state (similar to monitoring GIMP state)
- Displays values in Action Ring (similar to displaying GIMP actions)
- Uses simple Plugin + Application class pattern
- No complex managers or over-engineered components

## Architecture

### High-Level Architecture

```mermaid
graph TB
    subgraph "Logitech Ecosystem"
        LO[Logi Options+ App]
        LPS[Logi Plugin Service]
        AR[Action Ring Overlay]
        DEV[MX Device]
        PROF[User Profiles .lp5<br/>Button/Action Ring Mappings]
    end
    
    subgraph "Plugin Process (Separate Process)"
        GP[GimpPlugin Class<br/>inherits Plugin]
        GA[GimpApplication Class<br/>inherits ClientApplication]
        AC[Action Command Classes<br/>inherit PluginDynamicCommand]
        GI[GimpInterop Class]
    end
    
    subgraph "GIMP"
        GIMP[GIMP 3 Process<br/>gimp-3.0.exe]
        GOBJ[GObjects API]
    end
    
    subgraph "Plugin Package"
        PKG[Plugin Package<br/>from marketplace.logi.com]
        DEFPROF[Default Profiles<br/>DefaultProfile70/71/72.lp5]
    end
    
    DEV -->|Button Press| LO
    LO -->|User Config| LO
    LO <-->|Manages| LPS
    AR -->|Action Selection| LO
    PROF -->|Defines Mappings| LO
    
    PKG -->|Install| LO
    DEFPROF -->|Auto-Install| PROF
    
    LPS <-->|gRPC Protocol| GP
    LPS -->|Spawns Process| GP
    GP -->|Registers| AC
    GP -->|Creates| GA
    GA -->|GetProcessName<br/>returns gimp-3.0| GIMP
    AC -->|Invokes| GI
    GI -->|.NET Interop| GOBJ
    GOBJ -->|Controls| GIMP
    
    LO -.->|Persists Config| LO
    LPS -.->|Provides Config<br/>via gRPC| GP
    PROF -.->|Activates when<br/>GIMP in focus| LO
```

**Key Architecture Points:**
- Plugin runs as a **separate process**, NOT a DLL loaded into Plugin Service
- Logi Plugin Service spawns the plugin process at startup
- Plugin communicates with Plugin Service via **gRPC protocol**
- C# SDK wraps gRPC, making it feel like simple interface implementation
- Process isolation: plugin crashes don't affect Options+ or Plugin Service
- Logi Options+ manages all configuration persistence (plugin does NOT persist config)
- **Profiles (.lp5 files) define button/Action Ring mappings** - created in Options+ UI
- **Default profiles auto-install** when plugin is installed from marketplace
- **Profiles automatically activate** when GIMP comes into focus (adapt-to-app)
- GimpApplication links to GIMP process via `GetProcessName()` returning "gimp-3.0"
- Plugin activates when GIMP comes to foreground (if HasNoApplication = false)

### Component Responsibilities

**GimpPlugin Class** (inherits from `Plugin` abstract class)
- Plugin lifecycle management (Initialize, Shutdown)
- Register all action command instances with SDK
- Provide plugin metadata (name, version, description)
- Configure plugin properties:
  - `HasNoApplication = false` (requires GIMP in foreground)
  - `UsesApplicationApiOnly = true` (uses GIMP API, not keyboard shortcuts)
- Receive configuration from Logi Plugin Service via gRPC (does NOT persist config)
- Runs in separate process, communicates via gRPC (SDK abstracts this)
- **NOTE**: Plugin code does NOT handle button mappings - that's done by profiles (.lp5 files)

**GimpApplication Class** (inherits from `ClientApplication` abstract class)
- Link to GIMP process via `GetProcessName()` returning "gimp-3.0"
- Enable "Adapt to App" feature (actions available when GIMP is foreground)
- Provide application metadata (name, version)
- Plugin activates automatically when GIMP comes to foreground
- **NOTE**: Profile activation is handled by Options+ based on this process link

**Action Command Classes** (inherit from `PluginDynamicCommand` base class)
- One class per GIMP action or workflow
- Implement `RunCommand()` to execute the action
- Provide metadata: DisplayName, GroupName, icon path
- Use GimpInterop to invoke GIMP operations
- Handle errors with SDK logging
- Can be Command actions (button press) or Adjustment actions (dial/slider)
- Executed in plugin process, triggered via gRPC from Plugin Service
- **NOTE**: These define AVAILABLE actions - profiles map them to buttons/Action Ring

**GimpInterop Class**
- Establish connection to GIMP via GObjects introspection
- Invoke GIMP operations with parameters via .NET interop
- Marshal .NET types to GObject types
- Check GIMP state (has active image, has selection, etc.)
- Return operation results or errors
- Runs in plugin process, separate from Plugin Service

**Profiles (.lp5 files)** - NOT part of plugin code
- Created in Options+ UI, exported as .lp5 files
- Define button/dial mappings to plugin actions
- Configure Action Ring layout (8 action slots)
- Set action parameters and custom icons
- Bind to GIMP application (adapt-to-app)
- Stored and managed by Options+, NOT by plugin code
- Default profiles included in plugin package, auto-install on plugin installation

### Communication Flow

**Plugin Loading Flow:**
1. User starts Logi Options+ application
2. Logi Options+ launches Logi Plugin Service
3. Logi Plugin Service discovers plugin in plugins directory
4. Plugin Service spawns plugin as a separate process (not loaded as DLL)
5. gRPC connection established between Plugin Service and plugin process
6. Plugin Service calls `GimpPlugin.Initialize()` via gRPC
7. GimpPlugin creates GimpApplication instance
8. GimpPlugin creates and registers all action command instances
9. Plugin Service receives action catalog via gRPC
10. Options+ displays GIMP actions in UI for user configuration

**Profile Installation Flow (First Time):**
1. User installs plugin from marketplace.logi.com
2. Plugin package includes default profiles (DefaultProfile70/71/72.lp5)
3. Options+ detects user's device (e.g., MX Creative Keypad = device ID 70)
4. Options+ automatically installs DefaultProfile70.lp5 for user's device
5. Profile is bound to GIMP application (adapt-to-app)
6. Profile contains button mappings and Action Ring configuration
7. User can now see GIMP profile in Options+ UI

**Action Execution Flow:**
1. User triggers action (via Action Ring or device button press)
2. Options+ looks up action from active profile's button/Action Ring mapping
3. Logi Options+ sends action trigger to Logi Plugin Service
4. Plugin Service routes trigger to plugin process via gRPC
5. Plugin Service calls `RunCommand()` on appropriate action command via gRPC
6. Action command uses GimpInterop to invoke GIMP operation via GObjects API
7. GimpInterop marshals .NET types to GObject types
8. GIMP executes operation and returns result
9. Result/error logged via SDK logging framework
10. Plugin Service receives completion notification via gRPC
11. Options+ receives completion status

**Configuration Flow:**
1. User modifies configuration in Logi Options+ UI (enables/disables actions, updates bindings)
2. Options+ persists configuration to its own storage (plugin does NOT handle persistence)
3. Options+ sends configuration updates to Plugin Service
4. Plugin Service provides configuration to plugin via gRPC callbacks
5. Plugin responds to action triggers based on current configuration state
6. NO custom ConfigurationManager needed - SDK handles everything via gRPC

**Foreground Detection Flow:**
1. GimpApplication.GetProcessName() returns "gimp-3.0"
2. Logi Plugin Service monitors active window process name
3. When GIMP comes to foreground, Plugin Service activates plugin via gRPC
4. Options+ activates GIMP profile (loads button/Action Ring mappings)
5. GIMP actions become available in Action Ring and device buttons
6. When GIMP loses focus, Plugin Service deactivates plugin via gRPC
7. Options+ deactivates GIMP profile (restores previous profile)
8. GIMP actions become unavailable until GIMP regains focus

### Threading Model

The SDK handles threading automatically via gRPC - plugin does NOT implement custom threading:
- Logi Plugin Service manages all gRPC communication with plugin on its own threads
- Action commands execute on SDK-managed threads in the plugin process
- Plugin Service ensures thread-safe action execution across process boundary
- GimpInterop may use async/await for responsiveness, but threading is managed by SDK
- No locks, mutexes, or synchronization primitives needed in plugin code
- Process isolation provides natural thread safety between Plugin Service and plugin

## Components and Interfaces

### Simplified Component Model

The plugin architecture is intentionally simple, following SDK patterns:

**Four Core Components:**
1. **GimpPlugin** - Plugin lifecycle and action registration
2. **GimpApplication** - Process linking for foreground detection
3. **Action Command Classes** - Individual GIMP operations (one class per action)
4. **GimpInterop** - GIMP GObjects API bridge

**No Additional Managers Needed:**
- NO ConfigurationManager (SDK handles config)
- NO ThreadManager (SDK handles threading)
- NO CommunicationManager (SDK handles communication)
- NO StateManager (actions check state as needed)
- NO EventManager (SDK handles events)

This keeps the plugin simple, maintainable, and aligned with SDK expectations.

### GimpPlugin Class

The main plugin class inheriting from the SDK's `Plugin` abstract class. This is the entry point for the plugin.

**Note:** This class runs in a separate process (sidecar model). The SDK handles all gRPC communication with the Plugin Service transparently, making it feel like simple method calls.

```csharp
public class GimpPlugin : Plugin
{
    private GimpInterop _gimpInterop;
    private GimpApplication _gimpApplication;
    private List<PluginDynamicCommand> _commands;
    
    // Plugin metadata - displayed in Options+ UI
    public override string Name => "GIMP Integration";
    public override string Version => "1.0.0";
    public override string Description => "Integrate GIMP 3 with Logitech devices";
    
    // Plugin configuration
    public override bool HasNoApplication => false;  // Requires GIMP in foreground
    public override bool UsesApplicationApiOnly => true;  // Uses GIMP API, not keyboard shortcuts
    
    // Called by Logi Plugin Service via gRPC when plugin process is spawned
    public override void Initialize()
    {
        // Initialize GIMP interop layer
        _gimpInterop = new GimpInterop();
        
        // Create application instance for process linking
        _gimpApplication = new GimpApplication();
        
        // Create and register all action commands with SDK
        RegisterAllCommands();
        
        // Log initialization via SDK logging
        Logger.Info("GIMP Plugin initialized");
    }
    
    // Called by Logi Plugin Service via gRPC when plugin process is terminating
    public override void Shutdown()
    {
        // Cleanup resources
        _gimpInterop?.Disconnect();
        _commands?.Clear();
        Logger.Info("GIMP Plugin shutdown");
    }
    
    private void RegisterAllCommands()
    {
        _commands = new List<PluginDynamicCommand>
        {
            // File operations
            new FileNewCommand(_gimpInterop),
            new FileOpenCommand(_gimpInterop),
            new FileSaveCommand(_gimpInterop),
            new FileSaveAsCommand(_gimpInterop),
            new FileExportCommand(_gimpInterop),
            
            // Edit operations
            new EditUndoCommand(_gimpInterop),
            new EditRedoCommand(_gimpInterop),
            new EditCutCommand(_gimpInterop),
            new EditCopyCommand(_gimpInterop),
            new EditPasteCommand(_gimpInterop),
            
            // Layer operations
            new LayerNewCommand(_gimpInterop),
            new LayerDuplicateCommand(_gimpInterop),
            new LayerMergeDownCommand(_gimpInterop),
            new LayerFlattenCommand(_gimpInterop),
            
            // Selection operations
            new SelectionAllCommand(_gimpInterop),
            new SelectionNoneCommand(_gimpInterop),
            new SelectionInvertCommand(_gimpInterop),
            new SelectionFeatherCommand(_gimpInterop),
            
            // Tool operations
            new ToolBrushCommand(_gimpInterop),
            new ToolEraserCommand(_gimpInterop),
            new ToolCloneCommand(_gimpInterop),
            new ToolSmudgeCommand(_gimpInterop),
            new ToolBlurCommand(_gimpInterop),
            new ToolTextCommand(_gimpInterop),
            new ToolMoveCommand(_gimpInterop),
            new ToolCropCommand(_gimpInterop),
            
            // View operations
            new ViewZoomInCommand(_gimpInterop),
            new ViewZoomOutCommand(_gimpInterop),
            new ViewZoomFitCommand(_gimpInterop),
            new ViewZoom100Command(_gimpInterop),
            
            // Filter operations
            new FilterGaussianBlurCommand(_gimpInterop),
            new FilterSharpenCommand(_gimpInterop),
            new FilterColorBalanceCommand(_gimpInterop),
            new FilterBrightnessContrastCommand(_gimpInterop),
            
            // Workflow operations
            new QuickExportPngWorkflow(_gimpInterop),
            new PrepareForWebWorkflow(_gimpInterop),
            new CreateLayerCopyWithEffectsWorkflow(_gimpInterop)
        };
        
        // Register each command with SDK
        foreach (var command in _commands)
        {
            RegisterCommand(command);
        }
    }
}
```

### GimpApplication Class

The application class inheriting from the SDK's `ClientApplication` abstract class. This links the plugin to the GIMP process.

```csharp
public class GimpApplication : ClientApplication
{
    // Return GIMP process name for Windows - used by SDK for foreground detection
    public override string GetProcessName()
    {
        // SDK monitors active window and activates plugin when this process is foreground
        return "gimp-3.0";  // or "gimp-2.99" for development builds
    }
    
    // Application metadata - displayed in Options+ UI
    public override string ApplicationName => "GIMP";
    public override string ApplicationVersion => "3.0+";
}
```

**How Foreground Detection Works:**
1. SDK calls `GetProcessName()` to get "gimp-3.0"
2. SDK monitors active window process name continuously
3. When GIMP window becomes active, SDK activates plugin
4. Plugin actions become available in Action Ring and device buttons
5. When GIMP loses focus, SDK deactivates plugin
6. This happens automatically - no code needed in plugin

### Action Command Classes

Each GIMP action is implemented as a separate class inheriting from `PluginDynamicCommand` base class. Actions can be:
- **Command actions**: Triggered by button press (most GIMP actions)
- **Adjustment actions**: Controlled by dial/slider (could be used for brush size, opacity, etc.)

**File Operation Example:**

```csharp
public class FileNewCommand : PluginDynamicCommand
{
    private readonly GimpInterop _gimpInterop;
    
    // Metadata displayed in Options+ UI
    public override string DisplayName => "New Image";
    public override string GroupName => "File";  // Category for organization
    
    public FileNewCommand(GimpInterop gimpInterop)
    {
        _gimpInterop = gimpInterop;
    }
    
    // Called by SDK via gRPC when action is triggered by user
    public override void RunCommand()
    {
        try
        {
            if (!_gimpInterop.IsConnected())
            {
                Logger.Error("GIMP is not running");
                return;
            }
            
            // Create new 1920x1080 RGB image via GObjects API
            _gimpInterop.InvokeOperation("gimp-image-new", 1920, 1080, 0);
            Logger.Info("Created new image");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to create new image: {ex.Message}");
        }
    }
}
```

**Edit Operation Example:**

```csharp
public class EditUndoCommand : PluginDynamicCommand
{
    private readonly GimpInterop _gimpInterop;
    
    public override string DisplayName => "Undo";
    public override string GroupName => "Edit";
    
    public EditUndoCommand(GimpInterop gimpInterop)
    {
        _gimpInterop = gimpInterop;
    }
    
    public override void RunCommand()
    {
        try
        {
            if (!_gimpInterop.IsConnected() || !_gimpInterop.HasActiveImage())
            {
                Logger.Error("No active GIMP image");
                return;
            }
            
            var imageId = _gimpInterop.GetActiveImageId();
            _gimpInterop.InvokeOperation("gimp-image-undo", imageId);
            Logger.Info("Undo executed");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to undo: {ex.Message}");
        }
    }
}
```

**Workflow Example:**

```csharp
public class QuickExportPngWorkflow : PluginDynamicCommand
{
    private readonly GimpInterop _gimpInterop;
    
    public override string DisplayName => "Quick Export PNG";
    public override string GroupName => "Workflows";
    
    public QuickExportPngWorkflow(GimpInterop gimpInterop)
    {
        _gimpInterop = gimpInterop;
    }
    
    public override void RunCommand()
    {
        try
        {
            if (!_gimpInterop.IsConnected() || !_gimpInterop.HasActiveImage())
            {
                Logger.Error("No active GIMP image");
                return;
            }
            
            var imageId = _gimpInterop.GetActiveImageId();
            
            // Step 1: Flatten image
            _gimpInterop.InvokeOperation("gimp-image-flatten", imageId);
            Logger.Info("Image flattened");
            
            // Step 2: Export as PNG
            var exportPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                $"export_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            );
            _gimpInterop.InvokeOperation("file-png-save", imageId, exportPath);
            Logger.Info($"Exported to {exportPath}");
            
            // Step 3: Close without saving
            _gimpInterop.InvokeOperation("gimp-image-delete", imageId);
            Logger.Info("Image closed");
        }
        catch (Exception ex)
        {
            Logger.Error($"Workflow failed: {ex.Message}");
        }
    }
}
```

### GimpInterop Class

Provides .NET interop with GIMP's GObjects API. This is the bridge between C# and GIMP's C-based GObjects API.

```csharp
public class GimpInterop
{
    private dynamic _gimp;
    private bool _isConnected;
    
    // Establish connection to GIMP via GObject introspection
    public bool Connect()
    {
        try
        {
            // Use GObject.Introspection NuGet package to connect to GIMP
            // This uses GObject introspection to dynamically discover GIMP's API
            _gimp = GObjectIntrospection.Connect("Gimp", "3.0");
            _isConnected = true;
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to connect to GIMP: {ex.Message}");
            _isConnected = false;
            return false;
        }
    }
    
    public void Disconnect()
    {
        _isConnected = false;
        _gimp = null;
    }
    
    public bool IsConnected()
    {
        return _isConnected;
    }
    
    // Check if GIMP has an active image open
    public bool HasActiveImage()
    {
        if (!_isConnected) return false;
        
        try
        {
            var images = _gimp.image_list();
            return images != null && images.Length > 0;
        }
        catch
        {
            return false;
        }
    }
    
    // Get the ID of the currently active image
    public int GetActiveImageId()
    {
        if (!_isConnected) 
            throw new InvalidOperationException("Not connected to GIMP");
        
        var images = _gimp.image_list();
        if (images == null || images.Length == 0)
            throw new InvalidOperationException("No active image");
            
        return images[0];
    }
    
    // Check if the active image has a selection
    public bool HasActiveSelection()
    {
        if (!_isConnected || !HasActiveImage()) return false;
        
        try
        {
            var imageId = GetActiveImageId();
            var hasSelection = _gimp.selection_is_empty(imageId);
            return !hasSelection;
        }
        catch
        {
            return false;
        }
    }
    
    // Invoke a GIMP operation via GObjects API
    // Marshals .NET types to GObject types automatically
    public object InvokeOperation(string operationName, params object[] args)
    {
        if (!_isConnected)
            throw new InvalidOperationException("Not connected to GIMP");
        
        try
        {
            // Use reflection to invoke GIMP operation
            // GObject introspection converts hyphens to underscores
            var method = _gimp.GetType().GetMethod(operationName.Replace("-", "_"));
            if (method == null)
                throw new InvalidOperationException($"Operation not found: {operationName}");
            
            // SDK handles type marshaling between .NET and GObject types
            return method.Invoke(_gimp, args);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to invoke {operationName}: {ex.Message}", ex);
        }
    }
}
```

## Data Models

### What Plugin Does NOT Implement

Based on the SDK sidecar architecture, the plugin does NOT need to implement:

**Configuration Persistence:**
- NO custom ConfigurationManager class needed
- NO config.json files to read/write
- Logi Options+ handles ALL configuration storage
- Plugin receives configuration via SDK callbacks (over gRPC)

**Threading Management:**
- NO custom thread pools or task schedulers
- NO locks, mutexes, or synchronization primitives
- SDK manages all threading automatically in plugin process

**Communication Protocol:**
- NO direct communication with Logi Options+ app
- NO manual gRPC implementation or network sockets
- SDK handles all gRPC communication with Plugin Service transparently
- Plugin code looks like simple method calls, SDK handles serialization/networking

**UI Components:**
- NO custom configuration UI
- Options+ provides the configuration interface
- Plugin only provides metadata (DisplayName, GroupName, icons)

**Process Management:**
- NO code to launch or monitor GIMP process
- NO code to manage plugin process lifecycle (Plugin Service handles this)
- SDK handles foreground detection via GetProcessName()
- Plugin only needs to check if GIMP API is available

### Plugin Package Structure

Following Logitech marketplace requirements (LoupedeckPackage.yaml format):

```
GimpPlugin/
├── metadata/
│   ├── LoupedeckPackage.yaml    # Plugin configuration (REQUIRED)
│   └── Icon256x256.png          # Plugin icon 256x256 (REQUIRED)
├── win/                         # Windows binaries (REQUIRED)
│   ├── GimpPlugin.exe           # Plugin executable (sidecar process)
│   ├── GimpPlugin.dll           # Plugin assembly (if using DLL + runner)
│   ├── GimpPlugin.deps.json     # .NET dependencies
│   ├── GimpPlugin.runtimeconfig.json  # .NET runtime configuration
│   └── GObject.Introspection.dll # GObjects interop library
├── actionicons/                 # Action icon images (REQUIRED)
│   ├── file-new.png             # PNG or SVG, displayed in Action Ring
│   ├── file-open.png
│   ├── edit-undo.png
│   └── ...
├── actionsymbols/               # Small icons for action picker UI (OPTIONAL)
│   ├── file-new.svg             # Small SVG icons
│   ├── file-open.svg
│   └── ...
├── profiles/                    # Default profiles for supported devices (REQUIRED for marketplace)
│   ├── DefaultProfile70.lp5     # Logitech MX Creative Keypad
│   ├── DefaultProfile71.lp5     # Logitech MX Creative Dialpad
│   ├── DefaultProfile72.lp5     # Logitech Actions Ring (Action Ring overlay)
│   ├── DefaultProfile20.lp5     # Loupedeck CT (optional - if supporting)
│   ├── DefaultProfile30.lp5     # Loupedeck Live (optional - if supporting)
│   ├── DefaultProfile40.lp5     # Razer Stream Controller (optional - if supporting)
│   ├── DefaultProfile50.lp5     # Loupedeck Live S (optional - if supporting)
│   └── DefaultProfile60.lp5     # Razer Stream Controller X (optional - if supporting)
├── localization/                # Translation files (OPTIONAL)
│   ├── en-US.xliff
│   └── de-DE.xliff
└── events/                      # Event definitions (OPTIONAL)
    └── events.json
```

**Key Points:**
- Plugin runs as a **separate process** (sidecar model), spawned by Plugin Service
- Can be packaged as standalone .exe OR as DLL with SDK-provided runner
- Plugin Service spawns the process and communicates via gRPC
- metadata/ and win/ folders are REQUIRED
- actionicons/ folder is REQUIRED for Action Ring display
- profiles/ folder is REQUIRED for marketplace submission (must include default profiles)
- localization/, events/ are OPTIONAL
- icontemplates/ folder can be used for dynamic icon generation (OPTIONAL)

**Profile Naming Convention:**
- DefaultProfile{DeviceID}.lp5 - Standard device profile
- DefaultProfile{DeviceID}win.lp5 - Windows-specific variant
- DefaultProfile{DeviceID}mac.lp5 - macOS-specific variant
- Device IDs: 20=Loupedeck CT, 30=Loupedeck Live, 40=Razer Stream Controller, 50=Loupedeck Live S, 60=Razer Stream Controller X, 70=MX Creative Keypad, 71=MX Creative Dialpad, 72=Actions Ring

**Profile Contents:**
Each .lp5 file contains:
- Button/dial mappings to plugin actions
- Action Ring configuration (8 action slots for DefaultProfile72.lp5)
- Action settings and parameters
- Custom icons and sounds
- Workspace/page layouts
- Application binding (auto-activates when GIMP is in focus)

### Plugin Metadata (LoupedeckPackage.yaml)

Configuration file that describes the plugin to Logi Plugin Service and Options+:

```yaml
name: GIMP Integration
version: 1.0.0
author: Your Name
description: Integrate GIMP 3 with Logitech MX devices
icon: Icon256x256.png
minOptionsVersion: 1.60.0
supportedPlatforms:
  - windows
hasNoApplication: false          # Plugin requires GIMP in foreground
usesApplicationApiOnly: true     # Uses GIMP API, not keyboard shortcuts
requirements:
  gimp: "3.0+"
  windows: "10+"
```

**Key Configuration Properties:**
- `hasNoApplication: false` - Plugin activates only when GIMP is foreground
- `usesApplicationApiOnly: true` - Plugin uses GIMP API, not simulated keyboard shortcuts
- `minOptionsVersion` - Minimum Logi Options+ version required
- `supportedPlatforms` - Currently only Windows (GIMP 3 Windows build)

### Action Catalog

Actions are organized by GroupName (category):

**File Operations (GroupName: "File")**
- New Image, Open, Save, Save As, Export

**Edit Operations (GroupName: "Edit")**
- Undo, Redo, Cut, Copy, Paste

**Layer Operations (GroupName: "Layer")**
- New Layer, Duplicate Layer, Merge Down, Flatten Image

**Selection Operations (GroupName: "Selection")**
- Select All, Select None, Invert Selection, Feather Selection

**Tool Operations (GroupName: "Tools")**
- Brush, Eraser, Clone, Smudge, Blur, Text, Move, Crop

**View Operations (GroupName: "View")**
- Zoom In, Zoom Out, Fit in Window, 100% Zoom

**Filter Operations (GroupName: "Filters")**
- Gaussian Blur, Sharpen, Color Balance, Brightness-Contrast

**Workflow Operations (GroupName: "Workflows")**
- Quick Export PNG, Prepare for Web, Create Layer Copy with Effects

### Predefined Workflows

**Quick Export PNG**
1. Flatten image
2. Export as PNG to My Pictures folder
3. Close without saving

**Prepare for Web**
1. Flatten image
2. Scale to 1920px width (maintain aspect ratio)
3. Apply slight sharpen
4. Export as optimized JPEG (quality 85)

**Create Layer Copy with Effects**
1. Duplicate active layer
2. Apply Gaussian blur (radius 5)
3. Set layer opacity to 50%
4. Set blend mode to overlay

### Log File Format

Logs are written via SDK logging framework (plugin does NOT implement custom logging):

**Log Location:**
`%APPDATA%\Logitech\LogiOptionsPlus\Plugins\GimpPlugin\logs\gimp-plugin.log`

**Log Format:**
`[TIMESTAMP] [LEVEL] Message`

**Log Levels:**
- INFO: Normal operations (plugin initialized, action executed)
- WARNING: Non-critical issues (GIMP slow to respond)
- ERROR: Failures (GIMP not running, operation failed)

**Example Log Entries:**
```
[2024-01-15 14:32:01.123] [INFO] GIMP Plugin initialized
[2024-01-15 14:32:02.456] [INFO] Connected to GIMP 3.0.0
[2024-01-15 14:32:15.789] [INFO] Executed action: New Image
[2024-01-15 14:32:20.123] [ERROR] Failed to create new image: GIMP is not running
[2024-01-15 14:32:25.456] [ERROR] Operation timed out: gimp-image-flatten
```

**Logging Best Practices:**
- Use SDK's Logger class (Logger.Info, Logger.Warning, Logger.Error)
- Include operation/action name in log messages
- Include exception messages for errors
- Do NOT log sensitive user data

## Profile Design and Creation

### Profile Creation Workflow

Profiles are NOT hand-coded - they are created using the Options+ UI and exported as .lp5 files:

**Step 1: Create Profile in Options+ UI**
1. Open Logi Options+ application
2. Navigate to device settings (e.g., MX Creative Keypad)
3. Create new profile for GIMP application
4. Map plugin actions to physical buttons/dials
5. Configure Action Ring with 8 most common actions
6. Set action parameters and custom icons
7. Test profile with GIMP running

**Step 2: Export Profile**
1. In Options+ UI, select the profile
2. Export profile as .lp5 file
3. Rename to appropriate DefaultProfile{DeviceID}.lp5
4. Place in plugin's profiles/ directory

**Step 3: Include in Plugin Package**
1. Add all device-specific .lp5 files to profiles/ directory
2. Package plugin for marketplace distribution
3. When user installs plugin, default profiles auto-install
4. Profile automatically activates when GIMP comes into focus

### Recommended Default Profile Layout

**Action Ring Configuration (DefaultProfile72.lp5)**

The Action Ring has 8 slots for the most frequently used actions. Recommended layout:

| Slot | Action | Rationale |
|------|--------|-----------|
| 1 | Undo | Most frequently used action |
| 2 | Redo | Complement to Undo |
| 3 | New Layer | Common workflow action |
| 4 | Save | Frequent operation |
| 5 | Brush Tool | Primary editing tool |
| 6 | Eraser Tool | Common editing tool |
| 7 | Zoom Fit | View adjustment |
| 8 | Quick Export PNG | Workflow shortcut |

**MX Creative Keypad Button Mappings (DefaultProfile70.lp5)**

The MX Creative Keypad has multiple programmable buttons. Recommended mappings:

| Button | Action | Category |
|--------|--------|----------|
| Button 1 | File New | File |
| Button 2 | File Open | File |
| Button 3 | File Save | File |
| Button 4 | Edit Undo | Edit |
| Button 5 | Edit Redo | Edit |
| Button 6 | Edit Copy | Edit |
| Button 7 | Edit Paste | Edit |
| Button 8 | Layer New | Layer |
| Button 9 | Layer Duplicate | Layer |
| Button 10 | Selection All | Selection |
| Button 11 | Selection None | Selection |
| Button 12 | Zoom In | View |

**MX Creative Dialpad Mappings (DefaultProfile71.lp5)**

The MX Creative Dialpad has dials and buttons. Recommended mappings:

| Control | Action | Notes |
|---------|--------|-------|
| Dial 1 | Brush Size Adjustment | If supported as adjustment action |
| Dial 2 | Opacity Adjustment | If supported as adjustment action |
| Dial 3 | Zoom Level | If supported as adjustment action |
| Button 1 | Brush Tool | Tool switching |
| Button 2 | Eraser Tool | Tool switching |
| Button 3 | Clone Tool | Tool switching |
| Button 4 | Undo | Quick access |
| Button 5 | Redo | Quick access |

### Profile Lifecycle

**First Installation:**
1. User installs plugin from marketplace
2. Plugin package includes default profiles for all supported devices
3. Options+ automatically installs default profile for user's device
4. Profile is bound to GIMP application (adapt-to-app)
5. When GIMP comes into focus, profile automatically activates
6. User sees pre-configured Action Ring and button mappings

**User Customization:**
1. User opens Options+ and navigates to GIMP profile
2. User modifies button mappings, Action Ring layout, or action parameters
3. Options+ saves customized profile (separate from default)
4. User's custom profile persists across sessions
5. User can create multiple profiles for different GIMP workflows

**Plugin Updates:**
1. Developer releases plugin update with improved default profiles
2. User installs plugin update from marketplace
3. Updated default profiles become available as templates
4. User's existing custom profiles are NOT overwritten
5. User can choose to reset to new default profile or keep customizations

### Profile Design Best Practices

**Action Selection:**
- Prioritize most frequently used actions for Action Ring (8 slots)
- Group related actions on adjacent buttons (e.g., Undo/Redo)
- Include at least one workflow action for common tasks
- Balance between file, edit, layer, and tool operations

**Icon Design:**
- Use clear, recognizable icons for each action
- Maintain consistent visual style across all action icons
- Ensure icons are readable at Action Ring size (typically 64x64px)
- Use GIMP's standard icons where possible for familiarity

**Action Ring Organization:**
- Place most critical actions in top positions (1-4)
- Group complementary actions together (Undo/Redo, Brush/Eraser)
- Include at least one view action (Zoom Fit, Zoom 100%)
- Include at least one save/export action

**Button Layout:**
- Map frequently used actions to easily accessible buttons
- Consider ergonomics - place common actions on dominant hand buttons
- Group related actions by category (file operations together, edit operations together)
- Leave some buttons unmapped for user customization

**Testing:**
- Test profile with real GIMP workflows
- Verify all mapped actions execute correctly
- Ensure Action Ring displays properly with GIMP in focus
- Test adapt-to-app activation (profile activates when GIMP gains focus)
- Verify profile deactivates when switching to other applications

## Marketplace Distribution

### Marketplace Submission Requirements

The plugin must be packaged correctly for submission to marketplace.logi.com:

**Required Components:**
1. Plugin code (compiled executable or DLL)
2. Plugin metadata (LoupedeckPackage.yaml)
3. Plugin icon (Icon256x256.png)
4. Action icons (actionicons/ directory)
5. Default profiles for supported devices (profiles/ directory)
6. README with installation instructions
7. License file

**Critical for Marketplace Approval:**
- Plugin MUST include default profiles for all supported devices
- Default profiles MUST be well-designed and tested
- Action icons MUST be clear and professional
- Plugin metadata MUST be complete and accurate
- README MUST include GIMP 3 installation prerequisites

### Package Validation Checklist

Before marketplace submission, verify:

**Metadata Validation:**
- [ ] LoupedeckPackage.yaml includes all required fields
- [ ] Plugin name, version, author, description are accurate
- [ ] Icon256x256.png is high quality and represents GIMP
- [ ] minOptionsVersion is set correctly
- [ ] supportedPlatforms includes "windows"
- [ ] hasNoApplication is set to false (requires GIMP in foreground)
- [ ] usesApplicationApiOnly is set to true (uses GIMP API)

**Profile Validation:**
- [ ] profiles/ directory exists and contains .lp5 files
- [ ] DefaultProfile70.lp5 exists (MX Creative Keypad)
- [ ] DefaultProfile71.lp5 exists (MX Creative Dialpad)
- [ ] DefaultProfile72.lp5 exists (Actions Ring)
- [ ] Each profile has been tested with corresponding device
- [ ] Action Ring configuration includes 8 well-chosen actions
- [ ] Button mappings are logical and ergonomic
- [ ] Profiles are bound to GIMP application (adapt-to-app)

**Action Validation:**
- [ ] All actions have DisplayName and GroupName
- [ ] All actions have corresponding icons in actionicons/
- [ ] Action icons are clear and recognizable
- [ ] Actions are organized into logical categories
- [ ] At least 3 workflow actions are included

**Code Validation:**
- [ ] Plugin compiles without errors
- [ ] All dependencies are included in win/ directory
- [ ] GObject.Introspection.dll is included
- [ ] Plugin executable runs without crashing
- [ ] Plugin successfully registers with Plugin Service

**Documentation Validation:**
- [ ] README includes GIMP 3 installation instructions
- [ ] README lists supported devices
- [ ] README includes minimum system requirements
- [ ] README explains how to use Action Ring
- [ ] README includes troubleshooting section
- [ ] License file is included

### Installation Flow for End Users

Understanding the end-user installation flow helps ensure proper packaging:

**Step 1: Prerequisites**
1. User has Windows 10 or later
2. User has Logi Options+ installed
3. User has GIMP 3.0 or later installed
4. User has supported Logitech device (MX Creative Keypad/Dialpad)

**Step 2: Plugin Installation**
1. User visits marketplace.logi.com
2. User searches for "GIMP Integration" plugin
3. User clicks "Install" button
4. Marketplace downloads plugin package
5. Options+ installs plugin to plugins directory
6. Plugin Service spawns plugin process
7. Plugin registers with Plugin Service

**Step 3: Profile Installation**
1. Options+ detects user's device (e.g., MX Creative Keypad)
2. Options+ automatically installs corresponding default profile (DefaultProfile70.lp5)
3. Profile is bound to GIMP application
4. Profile appears in Options+ UI under GIMP application

**Step 4: First Use**
1. User launches GIMP 3
2. GIMP window comes into focus
3. Options+ detects GIMP process (gimp-3.0.exe)
4. Options+ activates GIMP profile automatically
5. User can now trigger actions via Action Ring or device buttons
6. User can customize profile in Options+ UI

### Marketplace Listing Content

**Plugin Title:**
"GIMP Integration for Logitech MX Devices"

**Short Description (160 characters):**
"Control GIMP 3 with your Logitech MX device. Quick access to tools, layers, filters, and workflows via Action Ring and programmable buttons."

**Long Description:**
```
Streamline your GIMP 3 image editing workflow with seamless Logitech MX device integration. 
This plugin brings GIMP's powerful tools and operations to your fingertips through the 
Action Ring overlay and programmable device buttons.

Features:
• Quick access to 40+ GIMP actions via Action Ring
• Pre-configured button mappings for MX Creative Keypad and Dialpad
• File operations: New, Open, Save, Export
• Edit operations: Undo, Redo, Cut, Copy, Paste
• Layer management: New Layer, Duplicate, Merge, Flatten
• Tool switching: Brush, Eraser, Clone, Smudge, Blur, Text, Move, Crop
• View controls: Zoom In/Out, Fit, 100%
• Filters: Gaussian Blur, Sharpen, Color Balance, Brightness-Contrast
• Custom workflows: Quick Export PNG, Prepare for Web, Layer Copy with Effects
• Automatic profile activation when GIMP is in focus
• Fully customizable button and Action Ring layouts

Requirements:
• Windows 10 or later
• GIMP 3.0 or later
• Logi Options+ app
• Supported device: MX Creative Keypad, MX Creative Dialpad, or compatible device

Installation:
1. Install GIMP 3 from gimp.org
2. Install this plugin from marketplace
3. Launch GIMP - default profile activates automatically
4. Customize button mappings in Options+ if desired

Support:
For issues or questions, visit [support URL]
```

**Screenshots:**
1. Action Ring overlay showing GIMP actions
2. Options+ UI with GIMP profile configuration
3. MX Creative Keypad with GIMP button mappings
4. GIMP workflow demonstration with device integration

**Tags:**
gimp, image-editing, photo-editing, graphics, creative, productivity, workflow, tools

**Category:**
Creative & Designa (file paths are OK, file contents are NOT)

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Plugin Registration

*For any* Logi Options+ startup, the plugin SHALL successfully register itself with the Logi Plugin Service.

**Validates: Requirements 1.1**

### Property 2: GIMP Process Detection Timing

*For any* GIMP 3 process start event, the plugin SHALL detect the running instance within 2 seconds.

**Validates: Requirements 1.2**

### Property 3: Action Deregistration Timing

*For any* GIMP close event, the plugin SHALL deregister all active action bindings within 1 second.

**Validates: Requirements 1.3**

### Property 4: Connection State Consistency

*For any* plugin session while GIMP is running, the GimpInterop connection state SHALL remain valid and consistent throughout the session.

**Validates: Requirements 1.4**

### Property 5: Action Catalog Completeness

*For any* call to get registered actions, the returned list SHALL contain all required actions across all categories (File, Edit, Layer, Selection, Tools, View, Filters, Workflows).

**Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 3.1**

### Property 6: Action Execution Timing

*For any* action invocation (via Action Ring or device button), the action SHALL complete execution within 50ms.

**Validates: Requirements 3.3, 4.2**

### Property 7: Action Metadata Completeness

*For any* registered action command, the command SHALL have a non-empty DisplayName, a valid GroupName, and a valid icon path reference.

**Validates: Requirements 3.4, 8.3**

### Property 8: Foreground State Handling

*For any* application focus state where GIMP is not the active application, all Action Ring interactions for GIMP actions SHALL be disabled.

**Validates: Requirements 3.5**

### Property 9: Concurrent Action Execution Safety

*For any* set of concurrent action invocations, the plugin SHALL handle all executions without state corruption or race conditions.

**Validates: Requirements 4.4**

### Property 10: Operation Result Verification

*For any* GIMP operation invocation, the plugin SHALL verify the operation result status and handle success/failure appropriately.

**Validates: Requirements 5.2**

### Property 11: Error Logging Completeness

*For any* failed operation or precondition check, the plugin SHALL log an error entry containing the operation/action name and error details.

**Validates: Requirements 5.3, 7.1, 7.2**

### Property 12: Workflow Sequential Execution

*For any* workflow command execution, all GIMP operations SHALL execute in the defined sequence order.

**Validates: Requirements 6.3**

### Property 13: Workflow Error Handling

*For any* workflow where an operation fails, the workflow SHALL stop execution immediately and log which operation failed, without executing subsequent operations.

**Validates: Requirements 6.4**

### Property 14: Operation Timeout Detection

*For any* GIMP operation that exceeds 5 seconds, the plugin SHALL detect the timeout and log an error.

**Validates: Requirements 7.3**

### Property 15: Precondition Verification

*For any* action that requires preconditions (active image, active selection, etc.), the plugin SHALL verify all preconditions before attempting execution.

**Validates: Requirements 7.4, 7.5**

### Property 16: Resource Cleanup on Disconnect

*For any* GIMP disconnect event, the plugin SHALL release all associated resources (connections, handles, etc.).

**Validates: Requirements 10.4**

### Property 17: Memory Usage Constraint

*For any* normal operation period, the plugin SHALL consume less than 50MB of memory.

**Validates: Requirements 10.1**

### Property 18: CPU Usage Constraint

*For any* idle period, the plugin SHALL use less than 5% CPU.

**Validates: Requirements 10.2**

### Property 19: Default Profile Inclusion

*For any* plugin package prepared for marketplace distribution, the package SHALL include default profile files for all supported devices (DefaultProfile70.lp5, DefaultProfile71.lp5, DefaultProfile72.lp5).

**Validates: Requirements 9.1, 9.3**

### Property 20: Profile Action Ring Configuration

*For any* default profile file for Actions Ring (DefaultProfile72.lp5), the profile SHALL contain exactly 8 action mappings.

**Validates: Requirements 3.2**

### Property 21: Marketplace Metadata Completeness

*For any* plugin package prepared for marketplace submission, the LoupedeckPackage.yaml SHALL contain all required fields (name, version, author, description, icon, minOptionsVersion, supportedPlatforms).

**Validates: Requirements 9.1**

### Error Categories

**Connection Errors**
- GIMP not detected or not running
- GIMP process terminated unexpectedly
- GObjects API connection failure

**Operation Errors**
- GIMP operation invocation failure
- Operation timeout (>5 seconds)
- Invalid operation parameters
- Precondition not met (no active image, layer, or selection)

**Resource Errors**
- Log file write failure
- Memory allocation failure

### Error Handling Strategy

**Detection**
- Synchronous error detection for immediate operation failures
- Timeout detection for unresponsive GIMP operations (5 second threshold)
- Exception catching at all external API boundaries (GObjects API calls)
- SDK handles exceptions from plugin code gracefully

**Logging**
- All errors logged via SDK logging framework (Logger.Error, Logger.Warning)
- Plugin does NOT implement custom logging - use SDK's Logger class
- Log location managed by SDK: `%APPDATA%\Logitech\LogiOptionsPlus\Plugins\GimpPlugin\logs\`
- Log format: `[TIMESTAMP] [LEVEL] Message`
- Log levels: INFO, WARNING, ERROR
- Include exception messages and operation names in logs

**Recovery**
- Automatic reconnection attempts for GIMP connection (retry every 5 seconds, max 3 attempts)
- Graceful degradation: Actions fail gracefully when GIMP not available
- Operation retry: Single automatic retry for transient failures
- Process isolation: Plugin crashes don't affect Plugin Service or Options+ (sidecar model)
- Plugin Service can automatically restart crashed plugin process

**User Notifications**
- SDK may display error notifications in Options+ UI
- Plugin logs errors - SDK decides whether to show user notifications
- Focus on clear, actionable log messages

**Error Messages**

User-facing error messages (logged) should be:
- Descriptive: Clearly state what went wrong
- Actionable: Suggest what the user can do to resolve
- Non-technical: Avoid implementation details

Examples:
- "GIMP is not running. Please launch GIMP 3 to use this plugin."
- "Cannot create new layer: No image is currently open in GIMP."
- "Operation timed out. GIMP may be busy processing another task."

## Testing Strategy

### Dual Testing Approach

The plugin requires both unit testing and property-based testing for comprehensive coverage:

**Unit Tests**: Verify specific examples, edge cases, and error conditions
- Specific action execution examples
- Error handling for specific failure scenarios
- Integration points between components
- Predefined workflow execution

**Property Tests**: Verify universal properties across all inputs
- Timing constraints (detection, execution, cleanup)
- Error logging completeness
- Precondition validation
- Resource cleanup
- Concurrent execution safety

Both testing approaches are complementary and necessary. Unit tests catch concrete bugs in specific scenarios, while property tests verify general correctness across a wide range of inputs.

### Property-Based Testing Configuration

**Framework**: Use **FsCheck** for C# property-based testing (integrates with xUnit, NUnit, MSTest)

**Test Configuration**:
- Minimum 100 iterations per property test (due to randomization)
- Each property test must reference its design document property
- Tag format: `// Feature: gimp-logitech-plugin, Property {number}: {property_text}`

**Example Property Test Structure**:

```csharp
[Property]
public Property ActionMetadataCompleteness()
{
    // Feature: gimp-logitech-plugin, Property 7: Action Metadata Completeness
    return Prop.ForAll<PluginDynamicCommand>(command =>
    {
        return !string.IsNullOrEmpty(command.DisplayName) &&
               !string.IsNullOrEmpty(command.GroupName) &&
               IsValidIconPath(command.IconPath);
    }).QuickCheckThrowOnFailure();
}

[Property]
public Property ConcurrentActionExecutionSafety()
{
    // Feature: gimp-logitech-plugin, Property 9: Concurrent Action Execution Safety
    return Prop.ForAll<List<string>>(actionIds =>
    {
        var plugin = new GimpPlugin();
        plugin.Initialize();
        
        // Execute actions concurrently
        var tasks = actionIds.Select(id => 
            Task.Run(() => plugin.ExecuteAction(id))).ToArray();
        Task.WaitAll(tasks);
        
        // Verify no state corruption
        return plugin.IsStateValid();
    }).QuickCheckThrowOnFailure();
}
```

### Unit Testing Strategy

**Test Organization**:
- One test class per component (GimpPlugin, GimpApplication, action commands, GimpInterop)
- Group tests by functionality (execution, validation, error handling)
- Use descriptive test names: `ExecuteAction_WhenPreconditionNotMet_LogsError`

**Key Test Scenarios**:

1. **Plugin Lifecycle**
   - Successful initialization
   - Action command registration
   - Graceful shutdown and resource cleanup

2. **Action Execution**
   - Successful execution of each action category
   - Precondition validation (active image, layer, selection)
   - Timeout handling for long-running operations
   - Error propagation from GIMP

3. **Workflow Execution**
   - Sequential execution of predefined workflows
   - Failure handling at each step
   - Logging of failed operations

4. **GIMP Integration**
   - Connection establishment
   - Operation invocation
   - State queries (HasActiveImage, HasActiveSelection)
   - Error handling for GIMP not running

5. **Error Handling**
   - Logging for all error categories
   - Recovery from transient failures
   - Timeout detection

**Mocking Strategy**:
- Mock GimpInterop for isolated action command testing
- Use real GimpInterop for integration tests (requires GIMP installed)
- Mock SDK logging framework for unit tests

**Test Data**:
- Various GIMP operation results (success, failure, timeout)
- Different GIMP states (no image, has image, has selection)
- Edge cases (empty strings, null values, invalid parameters)

### Integration Testing

**GIMP Integration Tests**:
- Test against real GIMP 3 instance (requires GIMP installed)
- Verify GObjects API calls execute correctly
- Test operation timing and timeout handling
- Validate precondition checks against actual GIMP state

**Logi Plugin Service Integration Tests**:
- Test plugin registration and lifecycle
- Verify action command registration
- Test action execution via SDK

**Profile Integration Tests**:
- Verify default profiles are included in plugin package
- Test profile installation when plugin is installed
- Verify profile activates when GIMP comes into focus
- Test Action Ring displays correct actions from profile
- Verify button mappings execute correct actions
- Test profile deactivation when GIMP loses focus
- Verify user can customize profiles without breaking functionality

**Performance Tests**:
- Memory usage during normal operation (<50MB)
- CPU usage when idle (<5%)
- Action execution timing (50ms)
- GIMP detection timing (<2 seconds)
- Cleanup timing (<1 second)

### Marketplace Package Testing

**Package Structure Validation**:
- Verify all required directories exist (metadata/, win/, actionicons/, profiles/)
- Verify all required files exist (LoupedeckPackage.yaml, Icon256x256.png)
- Verify default profiles exist for all supported devices
- Verify all action icons are present and valid

**Profile Content Validation**:
- Verify DefaultProfile72.lp5 contains 8 Action Ring mappings
- Verify all profile action references match registered plugin actions
- Verify profiles are bound to GIMP application
- Verify profiles contain valid button/dial mappings

**Metadata Validation**:
- Verify LoupedeckPackage.yaml contains all required fields
- Verify hasNoApplication is set to false
- Verify usesApplicationApiOnly is set to true
- Verify supportedPlatforms includes "windows"

**End-to-End Installation Test**:
- Install plugin package in test Options+ environment
- Verify plugin registers successfully
- Verify default profile installs for test device
- Launch GIMP and verify profile activates
- Test Action Ring displays correct actions
- Test button mappings execute correct actions
- Verify profile deactivates when switching applications

### Test Coverage Goals

- Line coverage: >80%
- Branch coverage: >75%
- Critical paths: 100% (action execution, error handling)
- All correctness properties: 100% (each property must have a corresponding property test)

