using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "Adeolu Adeyemi",
                    Email = "adeolu@sholaadeyemi.com"
                },
                new Student
                {
                    Id = 2,
                    Name = "Bukola Adeyemi",
                    Email = "olubukola@sholaadeyemi.com"
                }
                );
        }
    }
}
