# Screenshot Capture Guide for Video

## Required Screenshots (5 total)

### Screenshot 1: GIMP with Plugin Menu
**What to capture:**
- GIMP 3 main window with an image open
- Filters > Logitech menu expanded showing actions
- Clean workspace, no clutter

**Steps:**
1. Open GIMP 3
2. File > Open > Load any image (or create new)
3. Click Filters > Logitech
4. Press `Windows + Shift + S` to capture
5. Save as: `gimp-plugin-menu.png`

---

### Screenshot 2: Selection Tool Demo
**What to capture:**
- GIMP with Free Select tool active
- Partially completed selection path
- Cursor visible near starting point

**Steps:**
1. In GIMP, select Free Select tool (F key)
2. Make a few selection points
3. Hover cursor near the first point (don't close yet)
4. Press `Windows + Shift + S`
5. Save as: `gimp-selection-demo.png`

---

### Screenshot 3: Action Ring Mockup
**What to capture:**
- GIMP interface with Action Ring overlay (we'll create this)

**Steps:**
1. Take screenshot of GIMP interface
2. We'll add Action Ring overlay in Canva
3. Save as: `gimp-action-ring.png`

---

### Screenshot 4: Terminal Build Success
**What to capture:**
- Terminal/PowerShell showing successful build

**Steps:**
1. Open PowerShell
2. Navigate to: `cd C:\Users\Administrator\logioptionplus-gimp-plugin\GimpPlugin`
3. Run: `dotnet build`
4. When complete, capture the success message
5. Save as: `build-success.png`

---

### Screenshot 5: GitHub Repository
**What to capture:**
- GitHub repository page showing green checkmarks

**Steps:**
1. Open browser to: https://github.com/ett8u/logioptionplus-gimp-plugin
2. Capture the main page showing:
   - Repository name
   - README preview
   - File structure
   - Green commit checkmarks
3. Save as: `github-repo.png`

---

## Quick Capture Commands (Windows)

### Using PowerShell to automate:
```powershell
# Create screenshots directory
mkdir C:\Users\Administrator\video-assets

# Screenshots will be saved to:
# C:\Users\Administrator\Pictures\Screenshots\
# Then move them to video-assets folder
```

---

## Screenshot Settings

**Resolution**: 1920x1080 (Full HD)  
**Format**: PNG (best quality)  
**Location**: Save all to `C:\Users\Administrator\video-assets\`

---

## Additional Assets Needed

### 1. MX Console Image
**Option A**: Use official Logitech product photo
- Search: "Logitech MX Creative Console product photo"
- Download from Logitech.com press kit

**Option B**: Use mockup/illustration
- Create simple illustration in Canva
- Show device with glowing buttons

### 2. Architecture Diagram
**Create in Canva or PowerPoint:**
```
[GIMP Logo] ←→ [MQTT Server] ←→ [MX Console]
     ↓              ↓                ↓
  Python-fu    WebSocket         Haptics
```

**Elements:**
- 3 boxes with icons
- Bidirectional arrows
- Labels: "MQTT Pub/Sub", "WebSocket", "2-Way Communication"
- Color: Dark background, blue arrows

---

## Canva Video Assets Checklist

Upload these to Canva:
- [ ] Wilber mascot (SVG or PNG)
- [ ] gimp-plugin-menu.png
- [ ] gimp-selection-demo.png
- [ ] build-success.png
- [ ] github-repo.png
- [ ] MX Console image
- [ ] Architecture diagram

---

## Quick Screenshot Script

Save this as `capture-screenshots.ps1`:

```powershell
# Screenshot capture helper
Write-Host "GIMP Video Screenshot Helper" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""
Write-Host "1. Open GIMP and load an image"
Write-Host "2. Go to Filters > Logitech"
Write-Host "3. Press Windows + Shift + S to capture"
Write-Host ""
Write-Host "Screenshots save to: $env:USERPROFILE\Pictures\Screenshots\"
Write-Host ""
Write-Host "Press any key when ready for next screenshot..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Open GIMP
Start-Process "C:\Users\Administrator\AppData\Local\Programs\GIMP 3\bin\gimp-3.exe"

Write-Host "GIMP launched. Capture your screenshots!"
```

---

## After Capturing

1. **Organize files:**
   ```
   video-assets/
   ├── wilber.png
   ├── gimp-plugin-menu.png
   ├── gimp-selection-demo.png
   ├── build-success.png
   ├── github-repo.png
   ├── mx-console.png
   └── architecture-diagram.png
   ```

2. **Upload to Canva**
3. **Follow AI_VIDEO_PROMPTS.md** to create video

---

## Need Help?

If you need me to:
- Create the architecture diagram
- Design the Action Ring overlay
- Generate any other graphics

Just let me know!
