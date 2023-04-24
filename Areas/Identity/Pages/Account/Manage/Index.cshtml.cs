using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementWithIdentity.Models;

namespace UserManagementWithIdentity.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }


        }
        //like Edit or Details when they laod the data with them to get razor 
        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,//user.FirstName that loaded from Get Razor page ,FristName from database 
                LastName= user.LastName,
                PhoneNumber = phoneNumber,
                ProfilePicture=user.ProfilePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);//like retrieve data from database
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user); //call Load method in Get method to show data in get razor 
            return Page();//return View()
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);// recieve data from Get Razor 
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            #region New
            var firstName = user.FirstName;//user Not User ,, user load data from frontend Razor
            var lastName = user.LastName;
           // var profilePictrue = user.ProfilePicture;
            if (Input.FirstName != firstName) //mean there is a changing in FristName 
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);//update database
            }
            if (Input.LastName != lastName)//Input.FristName that is in database , user.FirstName that is loaded from Get razor page  
            {
                user.LastName = Input.LastName;//input.LastName ..data from database ,, user.LatName ..data that will shown for user in frontend
                await _userManager.UpdateAsync(user);//update data base 
            }
           /* if (Input.ProfilePicture != profilePictrue) //mean there is a changing in profilePicture 
            {
                user.ProfilePicture = Input.ProfilePicture; //error
                await _userManager.UpdateAsync(user);//update database
            }*/
            if (Request.Form.Files.Count > 0)//mean there is picture was uploaded 
            {
                var filePicture = Request.Form.Files.FirstOrDefault();
                //check file size and extension

                using (var dataStream = new MemoryStream()) //=using var dataStream = new MemoryStream(); 
                {
                    await filePicture.CopyToAsync(dataStream);//convert IFormFile Picture  to byte[] or Stream 
                    user.ProfilePicture = dataStream.ToArray();
                    //user.ProfilePicture = Input.ProfilePicture;//error....can't convert IFormFile to byte[]
                }
                await _userManager.UpdateAsync(user);
            }

            #endregion
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);//saveChanges
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            
            
            await _signInManager.RefreshSignInAsync(user); //like save changes
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
