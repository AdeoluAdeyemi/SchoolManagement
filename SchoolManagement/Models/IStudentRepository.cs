using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public interface IStudentRepository
    {
        Student GetStudent(int Id);
        IEnumerable<Student> GetAllStudent();
        Student Add(Student student);
        Student Update(Student studentChanges);
        Student Delete(int Id); 
    }
}
