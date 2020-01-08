using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class SQLStudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SQLStudentRepository> _logger;

        public SQLStudentRepository(AppDbContext context, ILogger<SQLStudentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Student Add(Student student)
        {
            _context.Student.Add(student);
            _context.SaveChanges();
            return student;
        }

        public Student Delete(int Id)
        {
            Student student = _context.Student.Find(Id);
            if(student != null)
            {
                _context.Student.Remove(student);
                _context.SaveChanges();
            }
            return student;
        }
         
        public IEnumerable<Student> GetAllStudent()
        {
            return _context.Student;
        }

        public Student GetStudent(int Id)
        {
            _logger.LogTrace("Trace Log");
            _logger.LogDebug("Debug Log");
            _logger.LogInformation("Information Log");
            _logger.LogWarning("Warning Log");
            _logger.LogError("Error Log");
            _logger.LogCritical("Critical Log");

            return _context.Student.Find(Id);
        }

        public Student Update(Student studentChanges)
        {
            var student = _context.Student.Attach(studentChanges);
            student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return studentChanges;
        }
    }
}
