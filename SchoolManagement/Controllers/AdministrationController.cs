using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    //[Authorize(Roles="Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);  
        }

        [HttpGet]
        public IActionResult ListRoles() {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);

            if(role == null) 
            {// role not found in the DB
                ViewBag.ErrorMessage = $"Role with Id = {Id} cannot be found";
                return View("NotFound");

            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users)
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }


        //[HttpPost]
        //public async Task<IActionResult> EditRole(EditRoleViewModel model)
        //{
        //    var role = await _roleManager.FindByIdAsync(model.Id);

        //    if (role == null)
        //    {// role not found in the DB
        //        ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
        //        return View("NotFound");

        //    }
        //    else
        //    {
        //        role.Name = model.RoleName;
        //        var result = await _roleManager.UpdateAsync(role);

        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("ListRoles", "Administration");
        //        }

        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }

        //        return View(model);
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (await _roleManager.RoleExistsAsync(model.RoleName))
                {
                    ViewBag.ErrorMessage = $"Role with Name={model.RoleName} already exist";
                    return View("NotFound");
                }
                else
                {
                    var role = await _roleManager.FindByIdAsync(model.Id);
                    role.Name = model.RoleName;
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles", "Administration");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach(var user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            { // role not found in the database. 
                ViewBag.ErrorMessage = $"row id = {roleId} not found";
                return RedirectToAction("notfound");
            }


            for (var i = 0; i <  model.Count(); i++) {

                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else {
                    continue;
                }

                if (result.Succeeded)
                {
                    if(i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var user = _userManager.Users; 
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Password = user.PasswordHash,
                ConfirmPassword = user.PasswordHash,
                City = user.City,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            //var user = _userManager.Users; 
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                //user.PasswordHash = model.Password; 

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} cannot be found";
                return View("NotFound");
            }
            else
            {

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListUsers");

            }

        } 

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            var roleExist = await _roleManager.RoleExistsAsync(role.Name);

            //if(role == null)
            if (!roleExist)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            try{
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogDebug($"Error deleting role {ex}");
                ViewBag.ErrorTitle = $"{role.Name} role is in use";
                ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. " +
                    $"If you want to delete this role, " +
                    $"please remove the users from the role and then try to delete ";
            }

            return View("ListRoles");
        }

        //[HttpPost]
        //public async Task<IActionResult> DeleteRole(string roleId)
        //{
        //    var role = await _roleManager.FindByIdAsync(roleId);
        //    var roleExist = await _roleManager.RoleExistsAsync(role.Name);

        //    //if(role == null)
        //    if (!roleExist)
        //    {
        //        ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
        //        return View("NotFound");
        //    }

        //    IList<ApplicationUser> usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);


        //    for (var i = 0; i < usersInRole.Count(); i++)
        //    {
        //        if (i < (usersInRole.Count() -1 )) {
        //            await _userManager.RemoveFromRoleAsync(usersInRole[i], role.Name);
        //        }
        //        else
        //        {
        //           var result = await _roleManager.DeleteAsync(role);

        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("ListRoles");
        //            }

        //            foreach(var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }

        //        }
        //    }
        //    return View("ListRoles");
        //}

            [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId){
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in _roleManager.Roles) {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View();
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { userId = userId });
        }
    }
}
