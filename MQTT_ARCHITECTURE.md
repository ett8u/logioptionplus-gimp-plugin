# MQTT Architecture

## Overview

The plugin uses MQTT pub/sub for bidirectional communication between the Logi Options+ plugin and GIMP, enabling real-time feedback and dynamic UI updates on MX devices.

## Architecture Diagram

```
┌─────────────────────┐
│  Logi Options+      │
│  Plugin (C#)        │
└──────────┬──────────┘
           │ publishes commands
           ↓
┌─────────────────────┐
│  MQTT Broker        │
│  (localhost:1883)   │
│  In-Memory          │
└──────────┬──────────┘
           │ subscribes
           ↓
┌─────────────────────┐
│  GIMP Python-fu     │
│  Bridge Plugin      │
└──────────┬──────────┘
           │ executes
           ↓
┌─────────────────────┐
│  GIMP PDB           │
│  (Procedure DB)     │
└──────────┬──────────┘
           │ publishes state
           ↓
┌─────────────────────┐
│  MX Device          │
│  (Haptic + Icons)   │
└─────────────────────┘
```

## MQTT Topics

### `gimp/action`
**Publisher**: Logi Options+ Plugin  
**Subscriber**: GIMP Python-fu Plugin  
**Purpose**: Send commands from MX device to GIMP

**Message Format**:
```json
{
  "operation": "edit-undo",
  "parameters": [],
  "timestamp": "2026-02-25T14:30:00Z"
}
```

### `gimp/status`
**Publisher**: GIMP Python-fu Plugin  
**Subscriber**: Logi Options+ Plugin  
**Purpose**: Report GIMP state changes

**Message Format**:
```json
{
  "status": "ready",
  "hasActiveImage": true,
  "hasSelection": false,
  "timestamp": "2026-02-25T14:30:00Z"
}
```

### `mx/haptic`
**Publisher**: GIMP Python-fu Plugin  
**Subscriber**: Logi Options+ Plugin  
**Purpose**: Trigger haptic feedback on MX device

**Message Format**:
```json
{
  "type": "pulse|success|error",
  "intensity": 50,
  "timestamp": "2026-02-25T14:30:00Z"
}
```

### `mx/icon`
**Publisher**: GIMP Python-fu Plugin  
**Subscriber**: Logi Options+ Plugin  
**Purpose**: Update button icons on MX Console

**Message Format**:
```json
{
  "action": "layer-new",
  "icon": "base64_encoded_png",
  "timestamp": "2026-02-25T14:30:00Z"
}
```

## Components

### 1. MqttBroker.cs
In-memory MQTT server using MQTTnet v4.3.7

**Key Features**:
- Runs on localhost:1883
- No external dependencies
- Automatic message routing
- Event logging

### 2. GimpInterop.cs
MQTT client for Logi Options+ plugin

**Key Methods**:
- `ConnectAsync()` - Start MQTT broker
- `InvokeOperationAsync(operation, params)` - Send command to GIMP
- `SendHapticFeedbackAsync(type, intensity)` - Trigger haptic
- `UpdateIconAsync(action, iconData)` - Update MX Console icon

### 3. logitech-mqtt-bridge.py
GIMP Python-fu plugin using paho-mqtt

**Key Features**:
- Auto-connects to localhost:1883
- Subscribes to `gimp/action`
- Executes GIMP PDB procedures
- Publishes state updates
- Sends haptic feedback

## Setup

### Prerequisites
```bash
# Install paho-mqtt for GIMP Python
pip install paho-mqtt
```

### Installation

1. **C# Plugin**: Build and deploy via dotnet
2. **Python Plugin**: Copy to `%APPDATA%\GIMP\3.0\plug-ins\logitech-mqtt-bridge\`
3. **Start GIMP**: Run "Start Logitech MQTT Bridge" from Filters > Development

## Operation Flow

### Command Execution
1. User presses MX device button
2. Logi Options+ plugin publishes to `gimp/action`
3. MQTT broker routes message
4. Python plugin receives and executes GIMP command
5. Python plugin publishes success to `gimp/status`
6. Python plugin sends haptic feedback to `mx/haptic`

### State Updates
1. GIMP state changes (new layer, selection, etc.)
2. Python plugin detects change
3. Publishes to `gimp/status`
4. C# plugin updates MX device UI

## Benefits

✅ **Bidirectional** - Real-time feedback from GIMP to device  
✅ **Extensible** - Easy to add new topics/features  
✅ **Web-Ready** - WebSocket support for web apps  
✅ **Decoupled** - Components communicate via messages  
✅ **Reliable** - QoS levels ensure delivery  

## Future Enhancements

- WebSocket endpoint for web app integration
- Persistent message storage
- Multi-client support
- Custom workflow automation
- AI-assisted action suggestions
