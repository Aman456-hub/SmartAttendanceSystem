# 🎓 Smart Attendance System (Face Recognition Based)

An AI-powered attendance management system that automates student attendance using real-time face recognition.

---

## 📌 Overview

SmartAttendanceSystem is a web-based application developed using ASP.NET Core MVC integrated with Python-based face recognition.

The system captures live images using a webcam, detects and recognizes faces, and automatically marks attendance in the database.

This project eliminates manual attendance, reduces proxy attendance, and ensures accuracy and efficiency.

---

## 🚀 Features

* Student Registration with Image Upload / Webcam Capture
* Real-time Face Detection & Recognition
* Automatic Attendance Marking
* Prevents Duplicate Attendance (Same Day)
* Attendance History Tracking
* Clean and Simple UI (Razor Views)
* Integration between C# and Python

---

## 🛠️ Tech Stack

### Backend

* ASP.NET Core MVC (.NET 10)
* C#

### Frontend

* Razor Views
* HTML, CSS, JavaScript
* Webcam API (getUserMedia)

### Database

* Entity Framework Core
* SQL Server / SQLite

### AI / Face Recognition

* Python
* face_recognition (dlib)
* OpenCV

---

## 🧠 How It Works

Step-by-step flow:

1. User opens /Attendance/Live
2. Webcam captures image using browser
3. Image is sent to backend (C#) as Base64
4. Backend saves image temporarily in wwwroot/captured/
5. Python script is triggered:
   python FaceRecognition/recognize.py <imagePath>
6. Python:

   * Loads known student images from wwwroot/images/
   * Encodes faces
   * Compares captured face with known faces
   * Returns matched student name
7. ASP.NET:

   * Matches name with database
   * Marks attendance (Insert / Update)
   * Displays result

---

## 📂 Project Structure

SmartAttendanceSystem/

Controllers/

* StudentsController.cs
* AttendanceController.cs

Models/

* Student.cs
* Attendance.cs

Data/

* AppDbContext.cs

Views/

* Students/
* Attendance/
* Shared/

wwwroot/

* images/ (student photos)
* captured/ (temporary images)

FaceRecognition/

* recognize.py

Program.cs
appsettings.json
README.md

---

## ⚙️ Setup Instructions

### 1. Clone Repository

git clone [https://github.com/YOUR_USERNAME/SmartAttendanceSystem.git](https://github.com/YOUR_USERNAME/SmartAttendanceSystem.git)
cd SmartAttendanceSystem

---

### 2. Setup Backend (.NET)

Open in Visual Studio
Restore dependencies

Run migration:

dotnet ef database update

---

### 3. Setup Python

Install Python (recommended 3.8+)

Install dependencies:

pip install face_recognition opencv-python

If dlib error occurs:

* Install CMake
* Install Visual Studio Build Tools
* Retry installation

---

### 4. Run Project

dotnet run

Open in browser:
[https://localhost:7172](https://localhost:7172)

---

## 📸 Usage Guide

### Add Student

* Go to /Students/Create
* Enter Name & Email
* Upload image OR capture via webcam

---

### Mark Attendance

* Go to /Attendance/Live
* Allow camera access
* Click Capture
* System detects and marks attendance

---

### View Attendance

* Go to /Attendance/Index
* View attendance records

---

## 🧪 Troubleshooting

Face not recognized:

* Use clear frontal image
* Good lighting
* One face per image
* Filename matches student name (e.g., John_Doe.png)

Python not running:

* Ensure python is in PATH
* Or use full path in code

dlib installation error:

* Install CMake
* Install Visual Studio Build Tools
* Then reinstall

Student not found:

* Check image filename vs database name
* Ensure student exists in DB

---

## 🔒 Security & Limitations

* Temporary images stored locally (not production safe)
* Uses filename-based identity mapping
* Single image per student reduces accuracy
* Python execution is synchronous

---

## 🚀 Future Improvements

* Multiple images per student
* Use Student ID instead of names
* Deploy AI as microservice
* Add authentication system
* Improve UI/UX
* Real-time video recognition

---

## 📷 Screenshots

●	LANDING PAGE:
 <img width="975" height="427" alt="image" src="https://github.com/user-attachments/assets/38fc394b-7bc1-466e-a37a-f4cb0be664d4" />

●	STUDENT CREATION:
<img width="975" height="517" alt="image" src="https://github.com/user-attachments/assets/9ff91c15-3c88-4774-8fde-7c3221d43f95" />

●	STUDENT INDEX:
<img width="975" height="517" alt="image" src="https://github.com/user-attachments/assets/ac19cd9d-5325-4155-aa09-783908ae2383" />

●	ATTENDANCE PORTAL:
<img width="975" height="517" alt="image" src="https://github.com/user-attachments/assets/a8874bd2-4530-4903-98cd-00faa5f8c810" />

●	LIVE ATTENDANCE:
<img width="975" height="517" alt="image" src="https://github.com/user-attachments/assets/d7693b2a-2e31-4b1b-9e63-7dc530bc1890" />

●	DATABASE:
<img width="892" height="629" alt="image" src="https://github.com/user-attachments/assets/9f5d80ec-ec85-41dc-a756-572015360678" />

<img width="911" height="549" alt="image" src="https://github.com/user-attachments/assets/f5d363a1-17e8-41b3-996a-09b6ab667a14" />


---

## 👨‍💻 Author

Aman Chhimwal
Yashasvi Pandey
