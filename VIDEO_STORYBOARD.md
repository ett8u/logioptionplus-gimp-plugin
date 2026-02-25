# The Haptic Fox - Storyboard

## Frame-by-Frame Breakdown (60 seconds @ 30fps = 1800 frames)

---

### SCENE 1: Evolution (0-10s, Frames 1-300)

**Frame 1-100 (0-3.3s)**
- Black screen fade in
- Vintage punch card image appears
- Morph to VT100 terminal
- Morph to modern MX Console
- Text: "From Bits..."

**Frame 101-200 (3.3-6.6s)**
- MX Console glowing, rotating 3D view
- Haptic waves emanating
- Text: "...to Empathy"

**Frame 201-300 (6.6-10s)**
- Title card: "THE HAPTIC FOX"
- Fox silhouette with haptic pulse
- Subtitle: "GIMP 3 + MX Devices"

---

### SCENE 2: The Problem (10-18s, Frames 301-540)

**Frame 301-400 (10-13.3s)**
- Split screen appears
- Left: Traditional workflow (mouse, keyboard, frustration)
- Right: MX Console (smooth, contextual, empowered)
- Text: "Digital Twins Demand New Affordances"

**Frame 401-540 (13.3-18s)**
- GIMP interface zooms in
- Complex selection tool active
- User struggling with precision
- Text: "30 Years of Creative Power"
- Zoom to starting point of selection path

---

### SCENE 3: The Solution (18-40s, Frames 541-1200)

**Frame 541-640 (18-21.3s)**
- Architecture diagram appears
- GIMP ↔ MQTT ↔ MX Console
- Animated message packets flowing
- Text: "MQTT Pub/Sub Architecture"

**Frame 641-740 (21.3-24.6s)**
- Close-up: Cursor hovering near selection start point
- Haptic pulse animation intensifies
- MX Console button glows
- Text: "Haptic Feedback = Precision"

**Frame 741-840 (24.6-28s)**
- Action Ring appears on screen
- 8 GIMP tools arranged in circle
- Rotating through options
- Each button shows live preview icon
- Text: "40+ Actions, Real-Time Feedback"

**Frame 841-940 (28-31.3s)**
- Python-fu console overlay
- MQTT messages scrolling:
  ```
  PUBLISH gimp/selection/hover
  SUBSCRIBE mx/haptic/pulse
  PUBLISH gimp/tool/active
  ```
- Text: "2-Way Communication"

**Frame 941-1040 (31.3-34.6s)**
- Web browser appears
- JavaScript code connecting to MQTT
- Text: "WebSocket Ready"
- Multiple apps connecting to same MQTT broker

**Frame 1041-1200 (34.6-40s)**
- Rapid montage:
  - Layer creation with haptic confirm
  - Filter application with progress haptics
  - Selection completion with success pulse
  - Export with deployment haptic
- Each action shows MX Console responding

---

### SCENE 4: The Impact (40-52s, Frames 1201-1560)

**Frame 1201-1300 (40-43.3s)**
- Code editor showing implementation
- Terminal: `dotnet build` ✓
- GitHub: Green checkmarks
- Text: "Built with AWS Kiro"

**Frame 1301-1400 (43.3-46.6s)**
- GIMP Filters menu showing plugin
- Options+ showing GIMP actions
- MX Console with custom profile loaded
- Text: "Production Ready"

**Frame 1401-1500 (46.6-50s)**
- Graph: MX adoption curve rising
- GIMP logo + MX logo = ❤️
- Text: "Widening MX Adoption"

**Frame 1501-1560 (50-52s)**
- Quote overlay:
  "Hardware jail-breaking into Application layer"
  "Solidarity in the silo-breaking freedom struggle"

---

### SCENE 5: The Close (52-60s, Frames 1561-1800)

**Frame 1561-1660 (52-55.3s)**
- Fox icon center screen
- Haptic waves pulsing outward
- Text orbiting: "FOSS • Versatile • Empathetic"

**Frame 1661-1760 (55.3-58.6s)**
- Call to action card:
  ```
  THE HAPTIC FOX
  github.com/ett8u/logioptionplus-gimp-plugin
  
  GIMP 3 + MX Devices + MQTT + Haptics
  ```

**Frame 1761-1800 (58.6-60s)**
- Logitech DevStudio Challenge 2026 logo
- Fade to black
- Final frame: Fox silhouette with single haptic pulse

---

## Animation Notes

### Haptic Pulse Effect
```
- 3 concentric circles
- Orange (#FF6B35) with alpha fade
- Expand from 0 to 100px radius
- Duration: 0.5s
- Easing: ease-out
```

### MQTT Message Flow
```
- Small packet icons (16x16px)
- Travel along curved paths
- Color: Logitech Blue (#00B8FC)
- Speed: 200px/s
- Trail effect with fade
```

### Transition Style
```
- Quick cuts for energy
- Smooth fades for concepts
- Zoom transitions for emphasis
- No wipes or slides (too dated)
```

---

## Audio Cues

**0-10s**: Ambient electronic build  
**10-18s**: Add bass, tension building  
**18-40s**: Full beat, energetic (demo section)  
**40-52s**: Maintain energy, add melodic element  
**52-60s**: Triumphant resolution, fade out  

**Sound Effects**:
- Haptic pulse: Soft "thump" with reverb
- MQTT message: Subtle "ping"
- Action completion: Satisfying "click"
- Selection close: Gentle "snap"

---

## Text Overlay Timing

All text should:
- Fade in: 0.3s
- Display: 2-3s minimum
- Fade out: 0.3s
- Font: Modern sans-serif (Montserrat or similar)
- Size: Large enough for mobile viewing
- Color: White with dark shadow for readability

---

## Production Notes

**Resolution**: 1920x1080 (1080p)  
**Frame Rate**: 30fps  
**Format**: MP4 (H.264)  
**Aspect Ratio**: 16:9  
**File Size Target**: <100MB  

**Screen Recording**:
- GIMP at 1920x1080
- Clean workspace, no clutter
- Smooth mouse movements
- Pre-planned actions

**Device Footage**:
- MX Console well-lit
- Stable camera
- Close-ups of button presses
- Haptic feedback visible (if possible)

---

## Backup 45-Second Version

If 60s is too long, cut:
- Frames 301-400 (problem setup)
- Frames 1041-1200 (extended montage)
- Frames 1401-1500 (adoption graph)

Keep core: Evolution → Solution → Demo → Impact → CTA
