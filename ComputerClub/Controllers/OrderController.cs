using BLL.Interfaces;
using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComputerClub.Controllers;

public class OrderController : Controller
{
    private readonly OrderService _orderService;
    private readonly ComputerService _computerService;
    private readonly UserService _userService;

    public OrderController(OrderService orderService, ComputerService computerService, UserService userService)
    {
        _orderService = orderService;
        _computerService = computerService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var orders = _orderService.GetByPredicate();
        if (orders == null)
        {
            return RedirectToAction("Privacy", "Home");
        }
        return View(orders);
    }

    [HttpGet]
    public IActionResult AddForm(OrderViewModel orderViewModel)
    {
        var users = _userService.GetAll();
        orderViewModel.Users = users.Select(u => new SelectListItem
        {
            Value = u.Email.ToString(),
            Text = u.FirstName + " " + u.LastName
        }).ToList();

        var computers = _computerService.GetAll();
        orderViewModel.Computers = computers.Select(c => new SelectListItem
        {
            Value = c.ModelName.ToString(),
            Text = c.ModelName
        }).ToList();

        return View(orderViewModel);
    }

    [HttpPost]
    public IActionResult AddFormp(OrderViewModel orderViewModel)
    {
        if (orderViewModel.EndTime < DateTime.Now)
            orderViewModel.State = false;
        else
            orderViewModel.State = true;

        if (orderViewModel.StartTime < DateTime.Now
            || orderViewModel.EndTime < orderViewModel.StartTime
            || orderViewModel.EndTime < DateTime.Now)
        {
            return BadRequest("Invalid start or end time");
        }

        orderViewModel.ComputerId = _computerService.FindByModelName(orderViewModel.ModelName);
        orderViewModel.UserId = _userService.FindByEmail(orderViewModel.Email);


        var NewOrder = new Order
        {
            DateTime = orderViewModel.StartTime,
            EndTime = orderViewModel.EndTime,
            State = orderViewModel.State,
            ComputerId = orderViewModel.ComputerId,
            UserId = orderViewModel.UserId,
        };
        
        _orderService.Insert(NewOrder);
        _orderService.Commit();

        return RedirectToAction("Index", "Order");
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var order = _orderService.GetById(id);
        if (order == null)
        {
            return NotFound();
        }

        var computer = _computerService.GetById(order.ComputerId);
        if (computer != null)
        {
            computer.IsReserved = false; 
            _computerService.Update(computer);
            _computerService.Commit();
        }

        _orderService.Delete(id);
        _orderService.Commit();
        return RedirectToAction("Index", "Order");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var order = _orderService.GetById(id);
        var orderViewModel = new OrderViewModel
        {
            OrderId = id,
            StartTime = order.DateTime,
            EndTime = order.EndTime,
            UserName = _userService.GetById(order.UserId).FirstName + " " + _userService.GetById(order.UserId).LastName,
            ModelName = _computerService.GetById(order.ComputerId).ModelName

        };
        
        var users = _userService.GetAll(); 
        orderViewModel.Users = users.Select(u => new SelectListItem
        {
            Value = u.Email.ToString(),
            Text = u.FirstName + " " + u.LastName
        }).ToList();
        (orderViewModel.Users[0], orderViewModel.Users[orderViewModel.Users.
            FindIndex(u => u.Text == orderViewModel.UserName)]) = (orderViewModel.Users[orderViewModel.Users.
            FindIndex(u => u.Text == orderViewModel.UserName)] , orderViewModel.Users[0]);

        var computers = _computerService.GetAll();
        orderViewModel.Computers = computers.Select(c => new SelectListItem
        {
            Value = c.ModelName.ToString(),
            Text = c.ModelName
        }).ToList();
        (orderViewModel.Computers[0], orderViewModel.Computers[orderViewModel.Computers.
    FindIndex(u => u.Text == orderViewModel.ModelName)]) = (orderViewModel.Computers[orderViewModel.Computers.
    FindIndex(u => u.Text == orderViewModel.ModelName)], orderViewModel.Computers[0]);

        return View(orderViewModel);
    }
    [HttpPost]
    public IActionResult Edit(OrderViewModel orderViewModel)
    {
        orderViewModel.State = orderViewModel.EndTime < DateTime.Now;

        //if (orderViewModel.StartTime < DateTime.Now
        //    || orderViewModel.EndTime < orderViewModel.StartTime
        //    || orderViewModel.EndTime > DateTime.Now)
        //{
        //   return View(orderViewModel);
        //}
        orderViewModel.ComputerId = _computerService.FindByModelName(orderViewModel.ModelName);
        orderViewModel.UserId = _userService.FindByEmail(orderViewModel.Email);

        var updatedOrder = new Order
        {
            OrderId = orderViewModel.OrderId,
            DateTime = orderViewModel.StartTime,
            EndTime = orderViewModel.EndTime,
            State = orderViewModel.State,
            UserId = orderViewModel.UserId,
            ComputerId = orderViewModel.ComputerId
        };

        _orderService.Update(updatedOrder);

        return RedirectToAction("Index", "Order");
    }
}
