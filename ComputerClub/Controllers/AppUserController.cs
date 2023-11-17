using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ComputerClub.Controllers;

public class AppUserController : Controller
{
    private readonly UserService _userService;
    private readonly OrderService _orderService;

    public AppUserController(UserService userService, OrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var users = _userService.GetByPredicate();
        if (users == null)
        {
            return RedirectToAction("Privacy", "Home");
        }
        return View(users);
    }

    [HttpGet]
    public IActionResult AddForm()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> InsertInto(UserViewModel userViewModel)
    {


        //Validation
        var newUser = new AppUser
        {
            Email = userViewModel.Email,
            Password = userViewModel.Password,
            FirstName = userViewModel.FirstName,
            LastName = userViewModel.LastName,
            PhoneNumber = userViewModel.PhoneNumber,
            Login = userViewModel.Login
        };
        _userService.Insert(newUser);
        _userService.Commit();

        return RedirectToAction("Index", "AppUser");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        _userService.Commit();
        return RedirectToAction("Index", "AppUser");
    }

    [HttpGet]
    public IActionResult Edit(string id)
    {
        var user = _userService.GetById(id);
        var userViewModel = new UserViewModel
        {
            UserId = id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Login = user.Login,
            Password = user.Password,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
        return View(userViewModel);
    }

    [HttpPost]
    public IActionResult Edit(UserViewModel userViewModel)
    {
        if (IsCorrect(userViewModel))
        {
            var updatedUser = new AppUser
            {
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Login = userViewModel.Login,
                Password = userViewModel.Password,
                Email = userViewModel.Email,
                PhoneNumber = userViewModel.PhoneNumber
            };

             _userService.Update(updatedUser);
            _userService.Commit();

            return RedirectToAction("Index", "AppUser");
        }
        else
            return BadRequest("UserViewModel or its properties cannot be null or empty.");
    }

    public bool IsCorrect(UserViewModel userViewModel)
    {
        return userViewModel != null
            && !string.IsNullOrEmpty(userViewModel.FirstName)
            && !string.IsNullOrEmpty(userViewModel.LastName)
            && !string.IsNullOrEmpty(userViewModel.Login)
            && !string.IsNullOrEmpty(userViewModel.Password)
            && !string.IsNullOrEmpty(userViewModel.Email)
            && !string.IsNullOrEmpty(userViewModel.PhoneNumber);
    }

    public IActionResult UserView(string id)
    {
        var user = _userService.GetById(id);

        //var orders= _orderService.GetByPredicate(o => o.UserId == user.AppUserId);

        var userViewModel = new UserViewModel
        {
            UserId = id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Login = user.Login,
            Password = user.Password,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
          //  Orders = orders
        };


        return View(userViewModel);
    }

}