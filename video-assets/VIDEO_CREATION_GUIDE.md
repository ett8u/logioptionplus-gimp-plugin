# Video Creation Guide - The Haptic Fox

## Option 1: Canva Video (Fastest - 30 minutes)

### Step 1: Setup
1. Go to https://canva.com/video
2. Search template: "Tech Product Demo 60 seconds"
3. Select 1920x1080 format

### Step 2: Customize
1. Replace title with "THE HAPTIC FOX"
2. Add subtitle: "GIMP 3 + Logitech MX"
3. Upload Wilber image (download from gimp.org)
4. Add GIMP screenshots from video-assets/screenshots/

### Step 3: Add Script
1. Use text-to-speech or record voiceover
2. Paste script from FINAL_SCRIPT_45s.md
3. Sync text overlays with voiceover timing

### Step 4: Style
- Background: Dark (#1A1A1A)
- Primary color: Logitech Blue (#00B8FC)
- Accent: Haptic Orange (#FF6B35)
- Font: Montserrat Bold

### Step 5: Export
- Format: MP4
- Quality: 1080p
- Download and test

---

## Option 2: Pictory.ai (Recommended - 1 hour)

### Step 1: Create Project
1. Go to https://pictory.ai
2. Choose "Script to Video"
3. Paste FINAL_SCRIPT_45s.md content

### Step 2: Select Template
- Template: "Tech/Software Demo"
- Style: Modern, Dark
- Aspect Ratio: 16:9

### Step 3: Customize Scenes
1. Scene 1: Upload split-screen mockup
2. Scene 2: Add GIMP interface screenshot
3. Scene 3: Add Wilber image + haptic animation
4. Scene 4: Add workflow screenshots
5. Scene 5: Add GitHub URL + logo

### Step 4: Add Media
- Upload screenshots from video-assets/screenshots/
- Add haptic pulse animation (create in Canva or use stock)
- Select upbeat electronic music from library

### Step 5: Voiceover
- Use Pictory's AI voice (Professional Male/Female)
- Or upload recorded voiceover
- Adjust timing to match visuals

### Step 6: Export
- Resolution: 1080p
- Format: MP4
- Add captions (auto-generated)

---

## Option 3: Manual Creation (Full Control - 2-4 hours)

### Tools Needed
- OBS Studio (screen recording)
- DaVinci Resolve (editing)
- GIMP (for screenshots)
- Audacity (audio editing)

### Step 1: Record Footage
```bash
# Start Xvfb
Xvfb :99 -screen 0 1920x1080x24 &
export DISPLAY=:99

# Launch GIMP
~/Downloads/GIMP-3.0.8-1-x86_64.AppImage &

# Record with OBS or:
ffmpeg -f x11grab -video_size 1920x1080 -i :99 -codec:v libx264 -r 30 gimp_demo.mp4
```

### Step 2: Capture Screenshots
1. Open GIMP
2. Load sample image
3. Demonstrate selection tool
4. Show Filters > Logitech menu
5. Capture each step

### Step 3: Create Animations
- Haptic pulse: Use Canva or After Effects
- MQTT flow: Use draw.io or Figma
- Wilber animations: Use GIMP itself

### Step 4: Edit in DaVinci Resolve
1. Import all footage
2. Cut to 45 seconds
3. Add text overlays
4. Add transitions
5. Color grade (dark theme)
6. Add music and sound effects

### Step 5: Export
- Format: H.264
- Resolution: 1920x1080
- Frame rate: 30fps
- Bitrate: 10 Mbps

---

## Quick Assets Checklist

### Images Needed
- [ ] Wilber mascot (download from gimp.org)
- [ ] MX Console product photo
- [ ] GIMP interface screenshots
- [ ] GitHub repository screenshot
- [ ] Logitech DevStudio Challenge logo

### Text Overlays
- [ ] "PRECISION REDEFINED"
- [ ] "THE HAPTIC FOX"
- [ ] "FEEL EVERY STROKE"
- [ ] "40+ Actions • Real-Time Feedback"
- [ ] "github.com/ett8u/logioptionplus-gimp-plugin"

### Audio
- [ ] Voiceover recording (45 seconds)
- [ ] Background music (upbeat electronic)
- [ ] Sound effects (haptic pulse, click, snap)

---

## Recommended Workflow

**For DevPost Submission (Fastest):**
1. Use **Canva Video** with tech template
2. Add screenshots and Wilber image
3. Use text-to-speech for voiceover
4. Export and upload
5. **Total time: 30 minutes**

**For Best Quality:**
1. Use **Pictory.ai** with script-to-video
2. Upload custom screenshots
3. Use AI voiceover or record your own
4. Fine-tune timing and transitions
5. **Total time: 1 hour**

---

## Next Steps

1. Choose your tool (Canva recommended for speed)
2. Gather assets (screenshots, Wilber, logos)
3. Follow the guide for your chosen tool
4. Export video
5. Upload to DevPost

**Need help with any step? Let me know!**
