#!/bin/bash
# Nova Reel Video Generation Script
# Generates multiple 6-second segments for The Haptic Fox video

BUCKET_NAME="haptic-fox-videos-1772033027"
REGION="us-east-1"
MODEL_ID="amazon.nova-reel-v1:0"

# Video segments (each 6 seconds)
declare -a SEGMENTS=(
  "Opening: Vintage punch card morphing into modern Logitech MX Console with glowing blue buttons. Dark tech background, cinematic lighting, smooth transition."
  
  "GIMP interface: Image editor showing selection tool with cursor. Orange haptic pulse waves emanate from selection start point. Professional UI, modern design."
  
  "Haptic feedback: Close-up of MX Console button glowing in sync with orange pulse waves. Smooth animation, professional product shot, dark background."
  
  "Action Ring: Circular UI overlay with 8 GIMP tool icons rotating smoothly. Each icon lights up as it passes. Futuristic interface, glowing effects."
  
  "MQTT flow: Blue animated data packets flowing between GIMP logo and MX Console. Technical diagram style, smooth animation, dark background."
  
  "Wilber mascot: Orange fox character (GIMP mascot) in center with haptic pulse waves emanating outward. Text overlay 'The Haptic Fox'. Triumphant mood."
  
  "Workflow demo: Artist hands using MX Console with GIMP on screen. Layers being created, filters applied. Professional workspace, smooth editing."
  
  "Closing: Wilber fox with text 'Where every touch counts' and GitHub URL. Logitech logo appears. Professional end card, clean design."
)

# Function to generate video segment
generate_segment() {
  local index=$1
  local prompt=$2
  local seed=$((42 + index))
  
  echo "=== Generating Segment $index ==="
  echo "Prompt: $prompt"
  
  cat > /tmp/segment_${index}.json << EOF
{
  "taskType": "TEXT_VIDEO",
  "textToVideoParams": {
    "text": "$prompt"
  },
  "videoGenerationConfig": {
    "durationSeconds": 6,
    "fps": 24,
    "dimension": "1280x720",
    "seed": $seed
  }
}
EOF

  aws bedrock-runtime start-async-invoke \
    --model-id "$MODEL_ID" \
    --region "$REGION" \
    --model-input "file:///tmp/segment_${index}.json" \
    --output-data-config "{\"s3OutputDataConfig\":{\"s3Uri\":\"s3://${BUCKET_NAME}/segment-${index}/\"}}" \
    --output json | jq -r '.invocationArn' | tee /tmp/arn_${index}.txt
  
  echo ""
}

# Function to check status
check_status() {
  local arn=$1
  aws bedrock-runtime get-async-invoke \
    --invocation-arn "$arn" \
    --region "$REGION" \
    --output json | jq -r '.status'
}

# Generate all segments
echo "Starting video generation for ${#SEGMENTS[@]} segments..."
for i in "${!SEGMENTS[@]}"; do
  generate_segment $i "${SEGMENTS[$i]}"
  sleep 2  # Rate limiting
done

echo ""
echo "=== All segments submitted ==="
echo "Monitoring progress..."
echo ""

# Monitor progress
while true; do
  completed=0
  total=${#SEGMENTS[@]}
  
  for i in $(seq 0 $((total-1))); do
    if [ -f /tmp/arn_${i}.txt ]; then
      arn=$(cat /tmp/arn_${i}.txt)
      status=$(check_status "$arn")
      echo "Segment $i: $status"
      
      if [ "$status" == "Completed" ]; then
        ((completed++))
      fi
    fi
  done
  
  echo "Progress: $completed/$total completed"
  echo ""
  
  if [ $completed -eq $total ]; then
    echo "✅ All segments completed!"
    break
  fi
  
  sleep 30
done

# List output locations
echo ""
echo "=== Video Segments ==="
for i in $(seq 0 $((${#SEGMENTS[@]}-1))); do
  echo "Segment $i: s3://${BUCKET_NAME}/segment-${i}/output.mp4"
done

echo ""
echo "Download with:"
echo "aws s3 cp s3://${BUCKET_NAME}/segment-0/output.mp4 ./segment-0.mp4"
