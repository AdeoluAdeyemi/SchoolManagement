using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    [Authorize] //Checks if the user is logged in
    public class HomeController : Controller
    {
        private IStudentRepository _studentRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IStudentRepository studentRepository, 
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger) 
        {
            _studentRepository = studentRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }


        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _studentRepository.GetAllStudent();
            return View(model);
        }


        [AllowAnonymous]
        public ViewResult Details(int? Id)
        {

            // throw new Exception("Error in Details View");

            _logger.LogTrace("Trace Log");
            _logger.LogDebug("Debug Log");
            _logger.LogInformation("Information Log");
            _logger.LogWarning("Warning Log");
            _logger.LogError("Error Log");
            _logger.LogCritical("Critical Log");

            Student student = _studentRepository.GetStudent(Id.Value);

            if(student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", Id.Value);
            }


            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel {
                Student = student,
                PageTitle = "Details Page"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhotoPath = uniqueFileName
                };

                _studentRepository.Add(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }

            return View();
        }


        [HttpGet]
        public ViewResult Edit(int Id)
        {
            Student student = _studentRepository.GetStudent(Id);

            if(student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", Id);
            }

            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                ExistingPhotoPath = student.PhotoPath
            };
            return View(studentEditViewModel);
        }


        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = _studentRepository.GetStudent(model.Id);
                student.Name = model.Name;
                student.Email = model.Email;

                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null) {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                            
                _studentRepository.Update(student);
                return RedirectToAction("Details", new { id = student.Id });
            }

            return View();
        }

        private string ProcessUploadedFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos != null && model.Photos.Count > 1)
            {
                foreach (IFormFile photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                }

            }

            return uniqueFileName;
        }
    } 
}
