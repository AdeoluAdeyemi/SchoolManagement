using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _studentList;
        public MockStudentRepository()
        {
            _studentList = new List<Student>()
            {
                new Student(){ Id = 1, Name = "Adeolu Adeyemi", Email = "adeolu@sholaadeyemi.com"/*, Department = Dept.Accounting*/},
                new Student(){ Id = 2, Name = "Bukola Adeyemi", Email = "olubukola@sholaadeyemi.com"/*, Department = Dept.Environmental_Sciences*/},
                new Student(){ Id = 3, Name = "Enitan Adeyemi", Email = "enitn@sholaadeyemi.com"/*, Department = Dept.Physical_Sciences*/}
            };
        }

        public Student Add(Student student)
        {
            student.Id = _studentList.Max<Student>(e => e.Id) + 1;
            _studentList.Add(student);
            return student;
        }

        public Student Delete(int Id)
        {
            Student student =_studentList.FirstOrDefault(e => e.Id == Id);
            if(student != null)
            {// We have found the student
                _studentList.Remove(student);
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudent()
        {
            return _studentList;
        }

        public Student GetStudent(int Id)
        {
            return _studentList.FirstOrDefault(e => e.Id == Id);
        }

        public Student Update(Student studentChanges)
        {
            Student student = _studentList.FirstOrDefault(e => e.Id == studentChanges.Id);
            if (student != null)
            {// We have found the student
                student.Name = studentChanges.Name;
                student.Email = studentChanges.Email;
                //student.Department = studentChanges.Department;
            }
            return student;
        }
    }
}
