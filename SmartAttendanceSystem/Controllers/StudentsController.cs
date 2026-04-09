using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAttendanceSystem.Data;
using SmartAttendanceSystem.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SmartAttendanceSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var data = await _context.Students.ToListAsync();
            Console.WriteLine("Total Students: " + data.Count); // debug
            return View(data);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile ImageFile, string CapturedImage)
        {
            Console.WriteLine("----- CREATE START -----");
            Console.WriteLine("Name: " + student.Name);
            Console.WriteLine("Email: " + student.Email);
            Console.WriteLine("Captured Length: " + (CapturedImage?.Length ?? 0));
            Console.WriteLine("ImageFile: " + (ImageFile != null ? ImageFile.FileName : "NULL"));

            // 🔥 IMPORTANT FIX (ModelState block hata diya)
            ModelState.Remove("ImagePath");

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // ✅ PRIORITY 1: FILE UPLOAD
            if (ImageFile != null && ImageFile.Length > 0)
            {
                try
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    student.ImagePath = "/images/" + fileName;
                    Console.WriteLine("✔ File Upload Saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ File Upload Error: " + ex.Message);
                }
            }

            // ✅ PRIORITY 2: WEBCAM
            else if (!string.IsNullOrWhiteSpace(CapturedImage) && CapturedImage.Contains("base64"))
            {
                try
                {
                    string base64Data = CapturedImage.Split(',')[1];
                    byte[] imageBytes = Convert.FromBase64String(base64Data);

                    string fileName = Guid.NewGuid().ToString() + ".png";
                    string filePath = Path.Combine(uploadPath, fileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    student.ImagePath = "/images/" + fileName;
                    Console.WriteLine("✔ Webcam Image Saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Webcam Error: " + ex.Message);
                }
            }

            // ✅ DEFAULT IMAGE
            else
            {
                student.ImagePath = "/images/default.png";
                Console.WriteLine("⚠ Default Image Used");
            }

            // 🔍 PRINT MODEL ERRORS (for safety)
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState Invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("ERROR: " + error.ErrorMessage);
                }
            }

            // 🔥 FORCE SAVE (no blocking now)
            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ SAVED SUCCESSFULLY");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ DB SAVE ERROR: " + ex.Message);
            }

            Console.WriteLine("----- CREATE END -----");

            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            ModelState.Remove("ImagePath"); // 🔥 fix

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(e => e.Id == student.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}