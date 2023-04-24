using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.ViewModels;

namespace UserManagementWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {

        /*private readonly UserManager<ApplicationUser> userManager;

        public RolesController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }*/

        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public  IActionResult Index()
        {
             
            var roles = roleManager.Roles.ToList();//without async becouse of EntityFrameworkCore not existed
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index" , roleManager.Roles.ToList());
            }
            //check if Role is existed in database or not
            var RoleIsExisted = await roleManager.RoleExistsAsync(model.Name);
            if (RoleIsExisted)
            {
                ModelState.AddModelError("Name", "Role is Existed");
                return View("Index", roleManager.Roles.ToList());
            }
            //create role
            await roleManager.CreateAsync(new IdentityRole(model.Name));
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            return RedirectToAction(nameof(Index));
        }
      }
}
