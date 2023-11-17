using ComputerClub.ViewModels;
using DL.Entities;
using DL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ComputerClub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData["Error"] = "Wrong password";
                return View(loginViewModel);
            }

            TempData["Error"] = "Wrong credentials";

            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            var user = await _userManager.FindByEmailAsync(registerViewModel.Email);

            // If we have a user
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";

                return View(registerViewModel);
            }

            var newUser = new AppUser
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PhoneNumber = registerViewModel.PhoneNumber,
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                Role = UserRole.Client
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (!await _roleManager.RoleExistsAsync(UserRole.Client.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Client.ToString()));
            }

            if (newUserResponse.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, UserRole.Client.ToString());

            if (roleResult.Succeeded)
            {
                return View("Error");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Info()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var userInfoViewModel = new UserInfoViewModel
            {
                UserId = userId
            };

            return View(userInfoViewModel);
        }


    }
}
