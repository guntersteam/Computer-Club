using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using DL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using QuestPDF.Infrastructure;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Previewer;

namespace ComputerClub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly OrderService _orderService;
        private readonly UserService _userService;
        private readonly ComputerService _computerService;
        private readonly PaymentService _paymentService;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager, OrderService orderService, UserService userService,
            ComputerService computerService, PaymentService paymentService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _orderService = orderService;
            _userService = userService;
            _computerService = computerService;
            _paymentService = paymentService;
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

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _userService.GetById(id);

            var accountViewModel = new AccountViewModel
            {
                UserId = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
            return View(accountViewModel);
        }

        [HttpPost]
        public IActionResult Edit(AccountViewModel accountViewModel)
        {
            var user = _userService.GetById(accountViewModel.UserId);

            var updatedUser = new AppUser
            {
                Id = accountViewModel.UserId,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
            };

           var result = _userManager.UpdateAsync(updatedUser);
            _userService.Commit();

            return RedirectToAction("Info", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult OrderHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var orders = _orderService.GetByPredicate(filter: order => order.UserId == userId);
            if (userId == null)
                return NotFound();

            var orderHistoryViewModel = new OrderHistoryViewModel
            {
                Orders = orders
            };

            return View(orderHistoryViewModel);

        }

        public IActionResult TakeAnFile(int id)
        {
            var order = _orderService.GetById(id);

            var user = _userService.GetByPredicate(user => user.Id == order.UserId).FirstOrDefault();

            var computer = _computerService.GetByPredicate(computer => computer.ComputerId == order.ComputerId).FirstOrDefault();

            var payment = _paymentService.GetByPredicate(payment => payment.OrderId == order.OrderId).FirstOrDefault();

            var doc = Document.Create(container => container.Page(page =>
            {

                page.Margin(50);
                page.Size(PageSizes.A4);
                //Table(table =>
                // {
                //     table.ColumnsDefinition(columns =>
                //     {
                //         columns.ConstantColumn(50);
                //         columns.ConstantColumn(100);
                //         columns.RelativeColumn(2);
                //         columns.RelativeColumn(3);
                //     });

                //     table.Cell().ColumnSpan(4).LabelCell("User information");
                //     table.Cell().ValueCell("First Name: " + user.FirstName);
                //     table.Cell().ValueCell("Last Name: " + user.LastName);
                //     table.Cell().ValueCell("Phone Number: " + user.PhoneNumber);
                //     table.Cell().ValueCell("Role: " + user.Role);
                // });

                page.Content().Padding(50).Column(column =>
                {
                    column.Item().Text("Order information").FontSize(30).Bold();
                    column.Item().Text("Date:" + DateTime.Now);
                    column.Item().Text("User Information").FontSize(14).Bold();
                    column.Item().Text("First Name: " + user.FirstName);
                    column.Item().Text("Last Name: " + user.LastName);
                    column.Item().Text("Phone Number: " + user.PhoneNumber);
                    column.Item().Text("Role: " + user.Role);


                    column.Item().Text("Order Information").FontSize(14).Bold();
                    column.Item().Text("Order ID: " + order.OrderId);
                    column.Item().Text("Start Date: " + order.StartDate);
                    column.Item().Text("End Time: " + order.EndTime);
                    column.Item().Text("State: " + order.State);


                    column.Item().Text("Payment information").FontSize(14).Bold();
                    column.Item().Text("Payment Id: " + payment.PaymentId);
                    column.Item().Text("Payment amount: " + payment.Amount);
                    column.Item().Text("Phone Number: " + payment.PaymentDate);
                    column.Item().Text("Payment type" + payment.PaymentType);

                    column.Spacing(20);

                    column.Item().Text("Computer information").FontSize(14).Bold();
                    column.Item().Text("Computer id : " + computer.ComputerId);
                    column.Item().Text("Computer model name : " + computer.ModelName);
                    column.Item().Text("Computer renting price: " + computer.PriceForHour);
                });
            }));

            MemoryStream stream = new MemoryStream();
            doc.GeneratePdf(stream);

            stream.Position = 0;

            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = "Sample.pdf";
            return fileStreamResult;

        }

        public IActionResult GetAdmins()
        {
            var users = _userService.GetByPredicate(user => user.Role == UserRole.Admin);

            var adminsNumber = users.Count();

            var doc = Document.Create(container => container.Page(page =>
            {

                page.Margin(50);
                page.Size(PageSizes.A4);

                page.Content().Padding(50).Column(column =>
                {
                    column.Item().Text("Administrators information").FontSize(30).Bold(); 
                    column.Item().Text("Date:" + DateTime.Now);
                    column.Item().Text("Total admin number " + adminsNumber);
                    foreach (var user in users)
                    {
                        column.Item().Text("Admin information").FontSize(14).Bold();
                        column.Item().Text("First Name: " + user.FirstName);
                        column.Item().Text("Last Name: " + user.LastName);
                        column.Item().Text("Phone Number: " + user.PhoneNumber);
                        column.Item().Text("Role: " + user.Role);
                    }
                });
            }));

            MemoryStream stream = new MemoryStream();
            doc.GeneratePdf(stream);

            stream.Position = 0;

            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            fileStreamResult.FileDownloadName = "Sample.pdf";
            return fileStreamResult;
        }
    }
}
