using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Data;
using SmartAttendanceSystem.Models;
using System.Diagnostics;
using System.Linq;


namespace SmartAttendanceSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ STEP 1: SHOW ALL ATTENDANCE + FILTER BY DATE
        public async Task<IActionResult> Index(DateTime? date)
        {
            var query = _context.Attendances
                .Include(a => a.Student)
                .AsQueryable();

            // 🔥 FILTER (NEW)
            if (date.HasValue)
            {
                query = query.Where(a => a.Date.Date == date.Value.Date);
            }

            var attendanceList = await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendanceList);
        }

        // ✅ STEP 2: SHOW STUDENTS FOR MARKING
        public async Task<IActionResult> Mark()
        {
            var students = await _context.Students.ToListAsync();
            return View(students);
        }

        // ✅ STEP 3: SMART SAVE (UPDATE SAME DAY + NO DUPLICATE)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(int studentId, bool isPresent)
        {
            DateTime today = DateTime.Today;

            // 🔍 CHECK EXISTING
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a =>
                    a.StudentId == studentId &&
                    a.Date.Date == today);

            if (existingAttendance != null)
            {
                // 🔁 UPDATE
                existingAttendance.IsPresent = isPresent;
                existingAttendance.Date = DateTime.Now;

                _context.Attendances.Update(existingAttendance);

                TempData["Message"] = "🔁 Attendance Updated!";
                Console.WriteLine("🔁 Updated existing attendance");
            }
            else
            {
                // ➕ CREATE NEW
                var attendance = new Attendance
                {
                    StudentId = studentId,
                    IsPresent = isPresent,
                    Date = DateTime.Now
                };

                _context.Attendances.Add(attendance);

                TempData["Message"] = "✅ Attendance Marked!";
                Console.WriteLine("✅ New attendance added");
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✅ STEP 2: LIVE PAGE
        public IActionResult Live()
        {
            return View();
        }

        // ✅ STEP 3: AUTO MARK WITH FACE RECOGNITION
        [HttpPost]
        public async Task<IActionResult> AutoMark(string CapturedImage)
        {
            if (string.IsNullOrEmpty(CapturedImage))
                return Content("❌ No Image");

            string fileName = Guid.NewGuid() + ".png";
            // Save captured images to a separate temp folder so they are not
            // mixed with the known student images stored in wwwroot/images.
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "captured");

            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);

            string filePath = Path.Combine(tempFolder, fileName);

            var base64 = CapturedImage.Split(',')[1];
            byte[] bytes = Convert.FromBase64String(base64);

            await System.IO.File.WriteAllBytesAsync(filePath, bytes);

            // 🔥 FACE RECOGNITION CALL
            string name = RecognizeFace(filePath);

            // remove the temporary captured image
            try
            {
                System.IO.File.Delete(filePath);
            }
            catch { }

            if (name == "Unknown")
                return Content("❌ Face Not Recognized");

            // Try robust matching: exact (case-insensitive) then partial match
            var students = await _context.Students.ToListAsync();
            var student = students
                .FirstOrDefault(s => string.Equals(s.Name?.Trim(), name?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (student == null)
            {
                student = students
                    .FirstOrDefault(s => !string.IsNullOrEmpty(s.Name) &&
                        s.Name.IndexOf(name ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (student == null)
                return Content("❌ Student Not Found");

            // 🔁 Check today's attendance
            var today = DateTime.Today;

            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.Date.Date == today);

            if (existing != null)
            {
                existing.IsPresent = true;
                existing.Date = DateTime.Now;
            }
            else
            {
                _context.Attendances.Add(new Attendance
                {
                    StudentId = student.Id,
                    IsPresent = true,
                    Date = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return Content($"✅ Attendance Marked: {name}");
        }

        // ✅ STEP 4: FACE RECOGNITION
        public string RecognizeFace(string imagePath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"FaceRecognition/recognize.py \"{imagePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            if (process == null)
                return "Unknown";

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = string.Empty;

            try
            {
                // try to read stderr if available
                stderr = process.StandardError?.ReadToEnd() ?? string.Empty;
            }
            catch { }

            process.WaitForExit();

            if (!string.IsNullOrEmpty(stderr))
            {
                // Log stderr for debugging
                Console.WriteLine($"Python stderr: {stderr}");
            }

            return stdout.Trim();
        }
    }
}