using System.ComponentModel.DataAnnotations;

namespace SmartAttendanceSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? ImagePath { get; set; }
    }
}