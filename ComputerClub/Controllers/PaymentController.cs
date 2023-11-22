using BLL.Interfaces;
using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensibility;

namespace ComputerClub.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;
        private readonly ComputerService _computerService;
        private readonly OrderService _orderService;
        public PaymentController(PaymentService paymentService,ComputerService computerService, OrderService orderService)
        {
            _paymentService = paymentService;
            _computerService = computerService;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            var paymentService = _paymentService.GetByPredicate();
            return View(paymentService);
        }

        public IActionResult Delete(int id)
        {
            var payment = _paymentService.GetById(id);
            if (payment == null)
            {
                return NotFound();
            }

            var order = _orderService.GetById(payment.OrderId);

            _paymentService.Delete(id);
            _paymentService.Commit();
            _orderService.Delete(order.OrderId);

            return RedirectToAction("Index", "Payment");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var payment = _paymentService.GetById(id);

            var order = _orderService.GetByPredicate(order => order.OrderId == payment.OrderId).FirstOrDefault();

            var paymentViewModel = new PaymentViewModel
            {
                PaymentId = payment.PaymentId,
                OrderId = order.OrderId,
                PaymentMethod = payment.PaymentType,
                Amount = payment.Amount,
            };

            return View(paymentViewModel);

        }


        public IActionResult Edit(PaymentViewModel paymentViewModel)
        {
            var payment = _paymentService.GetById(paymentViewModel.PaymentId);

            var order = _orderService.GetByPredicate(order => order.OrderId == payment.OrderId).FirstOrDefault();
            
            var updatedPayment = new Payment
            {
                PaymentId = paymentViewModel.PaymentId,
                PaymentDate = DateTime.Now, // validation
                PaymentType = paymentViewModel.PaymentMethod,
                Amount = paymentViewModel.Amount,
                OrderId= order.OrderId
            };

            _paymentService.Update(updatedPayment);

            return RedirectToAction("Index", "Payment");
        }
    }
}
