# ZeepUtils

A utility library for Unity and Zeepkist modding, providing a powerful fluent API for TextMeshPro (TMP) rich text
generation, color manipulation, and game integration.

## Features

- **TMPRichTextBuilder**: A fluent builder for creating complex TMP rich text strings with ease. Supports nesting,
  gradients, alignment, margins, and more.
- **ColorUtils**: Advanced color parsing and manipulation. Includes a minimized hex converter specifically designed for
  TMP.
- **Zeepkist Integration**:
    - `MessageApi`: High-level wrapper for Zeepkist's message system (server messages, private chat, broadcast).
    - `Emojis`: Constants and helpers for Zeepkist-specific TMP sprite tags.

## Installation

This library is intended for use in C# projects targeting Unity (specifically Zeepkist).

```xml
<PackageReference Include="ZeepUtils" Version="1.1.2" />
```

## Usage

### TMPRichTextBuilder

Create styled text using a fluent interface:

```csharp
using ZeepUtils.Text;

string styledText = new TMPRichTextBuilder("Important Message")
    .Bold()
    .Color("red")
    .Build();
// Output: <#f00><b>Important Message</b></color>
```

Add multiple layers and gradients:

```csharp
string gradientText = new TMPRichTextBuilder()
    .AddLayer("Cool Gradient")
    .ColorGradient("red", "yellow", "green")
    .Build();
```

### Color Utilities

```csharp
using ZeepUtils.Text;
using UnityEngine;

// Parse hex string with fallback
Color myColor = ColorUtils.ParseOrDefault("#FF5733", Color.white);

// Convert to minimized hex string (e.g., #ff5733 -> #f53 if possible, or #rrggbbaa -> #rgba)
string tmpHex = myColor.ToMinimizedHex();
```

### Zeepkist Message API

```csharp
using ZeepUtils.Zeepkist;

// Set a server message with duration
MessageApi.SetServerMessage("Lobby challenge starting!", MessageColor.orange, 10);

// Use constants for emojis
string heart = Emojis.Heart;
```

### Toast Notifications

Send toasts with automatic logging:

```csharp
using ZeepUtils.Zeepkist;

// Initialize once (e.g., in Plugin Awake)
ToastNotification.Initialize(this.Logger, "MyPluginTag");

// Use anywhere
ToastNotification.Success("Level Completed!");
ToastNotification.Error("Failed to load data.");
```

## Requirements

- **Framework**: .NET Framework 4.7.2
- **Dependencies**:
    - UnityEngine.Modules (2021.3.33+)
    - ZeepSDK (2.2.0+)
    - Zeepkist.GameLibs

## Author

**Hi Im Yolo**


