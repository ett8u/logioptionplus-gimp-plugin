# AWS Nova Reel Video Generation

## Status

**First Video**: ✅ In Progress  
**ARN**: `arn:aws:bedrock:us-east-1:742734949110:async-invoke/f5qat97z03qy`  
**Output**: `s3://haptic-fox-videos-1772033027/output/f5qat97z03qy/output.mp4`  
**Duration**: 6 seconds  
**Resolution**: 1280x720  
**FPS**: 24

## Limitations

- **Maximum duration**: 6 seconds per video
- **Solution**: Generate 8 segments of 6 seconds each = 48 seconds total
- **Combine**: Use ffmpeg to concatenate segments

## Video Segments Plan

1. **Segment 0** (0-6s): Evolution - Punch card → MX Console
2. **Segment 1** (6-12s): GIMP interface with selection tool
3. **Segment 2** (12-18s): Haptic feedback visualization
4. **Segment 3** (18-24s): Action Ring with tools
5. **Segment 4** (24-30s): MQTT message flow
6. **Segment 5** (30-36s): Wilber mascot with title
7. **Segment 6** (36-42s): Workflow demonstration
8. **Segment 7** (42-48s): Closing with CTA

## Usage

### Generate All Segments
```bash
cd /home/ubuntu/logitech-studio/logioptionplus-gimp-plugin/video-assets
./generate_nova_reel_video.sh
```

### Check Status
```bash
aws bedrock-runtime get-async-invoke \
  --invocation-arn "arn:aws:bedrock:us-east-1:742734949110:async-invoke/f5qat97z03qy" \
  --region us-east-1
```

### Download Segments
```bash
aws s3 cp s3://haptic-fox-videos-1772033027/segment-0/output.mp4 ./segment-0.mp4
aws s3 cp s3://haptic-fox-videos-1772033027/segment-1/output.mp4 ./segment-1.mp4
# ... etc
```

### Combine Segments
```bash
# Create file list
for i in {0..7}; do echo "file 'segment-$i.mp4'" >> segments.txt; done

# Concatenate
ffmpeg -f concat -safe 0 -i segments.txt -c copy haptic-fox-final.mp4
```

## Current Job

Monitoring first test video generation...
Expected completion: ~2-5 minutes

## Next Steps

1. Wait for first video to complete
2. Review quality and adjust prompts if needed
3. Generate all 8 segments
4. Download and combine
5. Add audio/voiceover
6. Upload to DevPost
