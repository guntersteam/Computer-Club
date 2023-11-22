using BLL.Interfaces;
using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;
using System.Security.Claims;

namespace ComputerClub.Controllers;

public class OrderController : Controller
{
    private readonly OrderService _orderService;
    private readonly ComputerService _computerService;
    private readonly UserService _userService;
    private readonly PaymentService _paymentService;
    public OrderController(OrderService orderService, ComputerService computerService, UserService userService, PaymentService paymentService)
    {
        _orderService = orderService;
        _computerService = computerService;
        _userService = userService;
        _paymentService = paymentService;
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
            StartDate = orderViewModel.StartTime,
            EndTime = orderViewModel.EndTime,
            State = orderViewModel.State,
            ComputerId = orderViewModel.ComputerId,
            UserId = orderViewModel.UserId,
        };
        
        _orderService.Insert(NewOrder);
        _orderService.Commit();

        var newPayment = new Payment
        {
            PaymentType = orderViewModel.PaymentType,
            Amount = orderViewModel.Amount,
            OrderId = NewOrder.OrderId,
            PaymentDate = DateTime.Now
        };

        _paymentService.Insert(newPayment);
        _paymentService.Commit();
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

        var payment = _paymentService.GetByPredicate(filter: payment => payment.OrderId == order.OrderId).FirstOrDefault();

        _orderService.Delete(id);
        _orderService.Commit();

        if(payment != null)
        {
            _paymentService.Delete(payment.PaymentId);
        }


        return RedirectToAction("Index", "Order");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var order = _orderService.GetById(id);
        var orderViewModel = new OrderViewModel
        {
            OrderId = id,
            StartTime = order.StartDate,
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
        

        orderViewModel.ComputerId = _computerService.FindByModelName(orderViewModel.ModelName);
        orderViewModel.UserId = _userService.FindByEmail(orderViewModel.Email);

        var updatedOrder = new Order
        {
            OrderId = orderViewModel.OrderId,
            StartDate = orderViewModel.StartTime,
            EndTime = orderViewModel.EndTime,
            State = orderViewModel.State,
            UserId = orderViewModel.UserId,
            ComputerId = orderViewModel.ComputerId
        };

        _orderService.Update(updatedOrder);

        return RedirectToAction("Index", "Order");
    }

    [HttpPost]
    public IActionResult InsertOrder(ComputerOrdersViewModel computerOrderViewModel)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (userId == null)
            return NotFound();

        var user = _userService.GetById(userId);

        if(computerOrderViewModel.Order.StartDate == DateTime.MinValue || computerOrderViewModel.Order.EndTime == DateTime.MinValue)
        {
            return View("DateError");
        }


        if (computerOrderViewModel.Order.StartDate.Date > computerOrderViewModel.Order.EndTime.Date)
        {
            ModelState.AddModelError("", "Invalid Date Range");
        }

        foreach (var order in _orderService.GetAll())
        {
            if ((computerOrderViewModel.Order.StartDate.Date <= order.EndTime.Date) && (order.StartDate.Date <= computerOrderViewModel.Order.EndTime.Date))
            {
                ModelState.AddModelError("", "Date Range Overlaps");
            }
        }

        var amount = _computerService.GetById(computerOrderViewModel.Computer.ComputerId).PriceForHour
            * (computerOrderViewModel.Order.EndTime - computerOrderViewModel.Order.StartDate).Hours;

        var newOrder = new Order
        {
            StartDate = computerOrderViewModel.Order.StartDate,
            EndTime = computerOrderViewModel.Order.EndTime,
            ComputerId = computerOrderViewModel.Computer.ComputerId,
            UserId = userId
        };

        _orderService.Insert(newOrder);
        newOrder = _orderService.GetById(newOrder.OrderId);

        var newPayment = new Payment
        {
            Amount = amount,
            PaymentType = computerOrderViewModel.Payment.PaymentType,
            PaymentDate = DateTime.Now,
            OrderId = newOrder.OrderId,
    };
        _paymentService.Insert(newPayment);

        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

        var computer = _computerService.GetById(computerOrderViewModel.Computer.ComputerId);

        smtpClient.Credentials = new System.Net.NetworkCredential("artem.protsenko2@gmail.com", "awlq lqdn gecy kbxq");
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true;

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("artem.protsenko2@gmail.com");
        mail.To.Add(new MailAddress($"{user.Email}"));
        mail.Subject = "Creating new order";
        mail.Body = $"You're ordere computer {computer.ModelName} " +
            $"from {newOrder.StartDate} to {newOrder.EndTime} in our club \nPayment inforamtion: Payment type:{newPayment.PaymentDate} Type: {newPayment.PaymentType} Price {newPayment.Amount}";

        smtpClient.Send(mail);

        return RedirectToAction("Index","Home");
    }

}
