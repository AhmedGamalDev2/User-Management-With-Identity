﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.ViewModels;

namespace UserManagementWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Select(user => new UserViewModel //Select:: to convert ApplicationUser to UserViewModel
            {//map some fields of ApplicationUser in UserViewModel
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).Result//Result::instead of using await
            }).ToListAsync();
            return View(users);//send UserViewModel
        }
        //Get
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user =await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync(); //get all roles from database 
            var userRolesViewModel =  new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role=> new RoleViewModel 
                {
                    RoleId= role.Id,
                    RoleName= role.Name,
                    IsSelected=_userManager.IsInRoleAsync(user,role.Name).Result    //true
                }).ToList()
            };

            return View(userRolesViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel  model)
        {
            var user =await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            var userRoles =await  _userManager.GetRolesAsync(user);//Roles assigned in this user ..related to this user 
            foreach (var role in model.Roles)
            {//there are 4 ways or cases for Roles
                if (userRoles.Any(r => r == role.RoleName) && !(role.IsSelected))//not selected and  existed in user
                {
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
                if (!userRoles.Any(r => r == role.RoleName) &&  role.IsSelected)//selected and not existed in user
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
            }
            return RedirectToAction(nameof(Index));

        }
    } 
  }
