import face_recognition
import os
import sys
from pathlib import Path

# 📁 FOLDER CONTAINING STUDENT PHOTOS
STUDENTS_FOLDER = "wwwroot/images"

def load_known_faces():
    """Load all student face encodings from wwwroot/images folder"""
    known_encodings = []
    known_names = []
    
    if not os.path.exists(STUDENTS_FOLDER):
        sys.stderr.write(f"Students folder not found: {STUDENTS_FOLDER}\n")
        return known_encodings, known_names
    
    # 🔍 SCAN ALL IMAGES IN FOLDER
    for filename in os.listdir(STUDENTS_FOLDER):
        if filename.endswith(('.png', '.jpg', '.jpeg')):
            filepath = os.path.join(STUDENTS_FOLDER, filename)
            
            try:
                # 📸 LOAD IMAGE
                image = face_recognition.load_image_file(filepath)
                
                # 🧠 ENCODE FACE
                face_encodings = face_recognition.face_encodings(image)
                
                if face_encodings:
                    # 💾 STORE ENCODING
                    known_encodings.append(face_encodings[0])
                    
                    # 🏷️ EXTRACT NAME FROM FILENAME
                    # Assumes filename format: "StudentName.png" or "StudentName_anything.png"
                    # Use the base filename with underscores replaced by spaces
                    name = os.path.splitext(filename)[0].replace('_', ' ')
                    known_names.append(name)
                    
            except Exception as e:
                print(f"Error processing {filename}: {e}")
    
    return known_encodings, known_names

def recognize_face(image_path):
    """Recognize face in captured image"""
    
    if not os.path.exists(image_path):
        return "Unknown"
    
    try:
        # 📥 LOAD TRAINED FACE ENCODINGS
        known_encodings, known_names = load_known_faces()
        
        # debug: report how many known faces loaded
        sys.stderr.write(f"Loaded {len(known_encodings)} known faces: {known_names}\n")

        if not known_encodings:
            sys.stderr.write("No known faces available for comparison.\n")
            return "Unknown"
        
        # 📸 LOAD CAPTURED IMAGE
        captured_image = face_recognition.load_image_file(image_path)
        
        # 🧠 ENCODE CAPTURED FACE
        captured_encodings = face_recognition.face_encodings(captured_image)
        
        if not captured_encodings:
            return "Unknown"
        
        # 🔍 COMPARE WITH KNOWN FACES
        captured_encoding = captured_encodings[0]
        
        # TOLERANCE: Lower = stricter matching (0.6 is default, 0.5 is stricter)
        matches = face_recognition.compare_faces(
            known_encodings, 
            captured_encoding, 
            tolerance=0.5
        )
        
        # 📊 CALCULATE FACE DISTANCES
        face_distances = face_recognition.face_distance(
            known_encodings, 
            captured_encoding
        )
        
        # debug: show match boolean list and distances
        sys.stderr.write(f"Matches: {matches}\n")
        sys.stderr.write(f"Distances: {face_distances}\n")

        # 🎯 FIND BEST MATCH
        if len(face_distances) > 0:
            best_match_index = face_distances.argmin()
            
            # ✅ IF MATCH FOUND AND CONFIDENT
            if matches[best_match_index] and face_distances[best_match_index] < 0.5:
                # return the matched name on stdout
                return known_names[best_match_index]
        
        return "Unknown"
        
    except Exception as e:
        print(f"Error in recognize_face: {e}")
        return "Unknown"

# 🚀 MAIN EXECUTION
if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Unknown")
        sys.exit(1)
    
    image_path = sys.argv[1]
    result = recognize_face(image_path)
    print(result)
