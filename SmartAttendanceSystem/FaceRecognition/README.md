# 🎥 Face Recognition Setup Guide

## 📋 Prerequisites

1. **Python 3.7+** installed on your system
2. **CMake** (required for face_recognition library)
3. **Visual C++ Build Tools** (on Windows)

## 🚀 Installation Steps

### Step 1: Install Python Dependencies

```bash
cd SmartAttendanceSystem\FaceRecognition
pip install -r requirements.txt
```

> **Note:** The first installation may take 5-10 minutes as it downloads pre-trained models.

### Step 2: Prepare Student Photos

1. Create a folder with student images in: `wwwroot/images/`
2. **Image naming format:** Use student's exact name as filename
   - Example: `John Doe.jpg` or `Jane_Smith.png`
3. Use clear, front-facing photos for best accuracy

### Step 3: Run Your Project

```bash
cd SmartAttendanceSystem
dotnet run
```

## 🎯 How It Works

1. When a student clicks **🎥 Live Attendance**:
   - Webcam feed starts
   - Student faces the camera
   
2. When **Capture** is clicked:
   - Image is sent to backend (C#)
   - `AutoMark()` method saves the image
   - Python script (`recognize.py`) is executed
   
3. Face Recognition Process:
   - Loads all student photos from `wwwroot/images/`
   - Encodes their faces
   - Compares with captured image
   - Returns matching student name
   
4. Attendance is automatically marked for the identified student

## ⚙️ Configuration

### Adjust Face Recognition Accuracy

In `recognize.py`, line ~69:

```python
# tolerance=0.5 (stricter, fewer false matches)
# tolerance=0.6 (default, balanced)
# tolerance=0.7 (looser, more matches)

matches = face_recognition.compare_faces(
    known_encodings, 
    captured_encoding, 
    tolerance=0.5  # ← Adjust this value
)
```

## ❌ Troubleshooting

### Python not found
```bash
python --version
# If not recognized, add Python to PATH
```

### Face recognition library fails to install
- Install Visual C++ Build Tools
- Or use pre-built wheels: `pip install face_recognition-0.3.0-cp310-cp310-win_amd64.whl`

### "Unknown" returned for known faces
- Check image quality (clear, well-lit)
- Ensure student name in filename matches database
- Adjust tolerance value in script (see above)

### Connection timeout
- Ensure `wwwroot/images/` folder exists
- Check Python path is correct

## 📸 Best Practices

✅ **Good Photos:**
- Clear face, looking at camera
- Good lighting
- No sunglasses/hats
- Face takes up 70%+ of image

❌ **Bad Photos:**
- Side profile
- Poor lighting
- Obstructed face
- Multiple faces in one image

## 🔄 Workflow Example

```
1. Student Registration
   └─ Upload photo via Students/Create page
   └─ Photo saved to wwwroot/images/
   └─ Filename: [Student_Name].png

2. Live Attendance
   └─ Click "🎥 Live Attendance"
   └─ Face camera and click "Capture"
   └─ Python recognizes face
   └─ Attendance marked automatically
   └─ Success message shows
```

## 📊 Performance Notes

- **First load:** ~3-5 seconds (loads all student encodings)
- **Recognition:** ~0.5-1 second per image
- **Database save:** ~0.2 seconds

## 🆘 Still Having Issues?

1. Check Console output for Python error messages
2. Verify `FaceRecognition/recognize.py` exists
3. Run: `python FaceRecognition/recognize.py "wwwroot/images/test.png"`
4. Ensure photos are in correct folder with correct naming
