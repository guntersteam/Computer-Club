using BLL.Interfaces;
using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;

namespace ComputerClub.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ReviewService _reviewService;
        private readonly ComputerService _computerService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;
        public ReviewController(ReviewService reviewService, ComputerService computerService, OrderService orderService, UserService userService)
        {
            _reviewService = reviewService;
            _computerService = computerService;
            _orderService = orderService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var reviews = _reviewService.GetByPredicate();
            return View(reviews);
        }

        [HttpGet]
        public IActionResult AddForm(ReviewViewModel reviewViewModel)
        {
            var users = _userService.GetAll();
            reviewViewModel.Users = users.Select(u => new SelectListItem
            {
                Value = u.Email.ToString(),
                Text = u.FirstName + " " + u.LastName
            }).ToList();

            var computers = _computerService.GetAll();
            reviewViewModel.Computers = computers.Select(c => new SelectListItem
            {
                Value = c.ModelName.ToString(),
                Text = c.ModelName
            }).ToList();

            return View(reviewViewModel);
        }

        [HttpPost]
        public IActionResult AddFormp(ReviewViewModel reviewViewModel)
        {
            reviewViewModel.UserId = _userService.FindByEmail(reviewViewModel.Email);
            reviewViewModel.ComputerId = _computerService.FindByModelName(reviewViewModel.ModelName);

            var newReview = new Review
            {
                ReviewId = reviewViewModel.ReviewId,
                ReviewText = reviewViewModel.ReviewText,
                UserId = reviewViewModel.UserId,
                ComputerId = reviewViewModel.ComputerId,
                Rating = reviewViewModel.Rating
            };

            _reviewService.Insert(newReview);
            _reviewService.Commit();

            return RedirectToAction("Index", "Review");

        }
        [HttpGet]

        public IActionResult Edit(int id)
        {
            var review = _reviewService.GetById(id);

            var reviewViewModel = new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                UserId = review.UserId,
                UserName = _userService.GetById(review.UserId).FirstName + " " + _userService.GetById(review.UserId).LastName,
                ComputerId = review.ComputerId,
                Rating = review.Rating,
                ModelName = _computerService.GetById(review.ComputerId).ModelName
            };

            var users = _userService.GetAll();
            reviewViewModel.Users = users.Select(u => new SelectListItem
            {
                Value = u.Email.ToString(),
                Text = u.FirstName + " " + u.LastName
            }).ToList();

            (reviewViewModel.Users[0], reviewViewModel.Users[reviewViewModel.Users.
               FindIndex(u => u.Text == reviewViewModel.UserName)]) = (reviewViewModel.Users[reviewViewModel.Users.
               FindIndex(u => u.Text == reviewViewModel.UserName)], reviewViewModel.Users[0]);

            var computers = _computerService.GetAll();
            reviewViewModel.Computers = computers.Select(c => new SelectListItem
            {
                Value = c.ModelName.ToString(),
                Text = c.ModelName
            }).ToList();

            (reviewViewModel.Computers[0], reviewViewModel.Computers[reviewViewModel.Computers.
               FindIndex(u => u.Text == reviewViewModel.ModelName)]) = (reviewViewModel.Computers[reviewViewModel.Computers.
               FindIndex(u => u.Text == reviewViewModel.ModelName)], reviewViewModel.Computers[0]);

            return View(reviewViewModel);

        }

        [HttpPost]

        public IActionResult Edit(ReviewViewModel reviewViewModel)
        {
            reviewViewModel.UserId = _userService.FindByEmail(reviewViewModel.Email);
            reviewViewModel.ComputerId = _computerService.FindByModelName(reviewViewModel.ModelName);

            var updatedReview = new Review
            {
                ReviewId = reviewViewModel.ReviewId,
                ReviewText = reviewViewModel.ReviewText,
                UserId = reviewViewModel.UserId,
                ComputerId = reviewViewModel.ComputerId,
                Rating = reviewViewModel.Rating
            };

            _reviewService.Update(updatedReview);
            return RedirectToAction("Index", "Review");

        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var review = _reviewService.GetById(id);
            if (review == null)
            {
                return NotFound();
            }

            var computer = _computerService.GetById(review.ComputerId);

            if (computer != null)
            {
                computer.IsReserved = false;
                _computerService.Update(computer);
                _computerService.Commit();
            }

            _reviewService.Delete(id);
            _reviewService.Commit();
            return RedirectToAction("Index", "Review");
        }

        public IActionResult CreateReview(ComputerOrdersViewModel computerOrderViewModel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (userId == null)
                return NotFound();

            if(computerOrderViewModel.Review.ReviewText == null)
            {
                return BadRequest();
            }

            var newReview = new Review
            {
                ReviewText = computerOrderViewModel.Review.ReviewText,
                Rating = computerOrderViewModel.Review.Rating,
                UserId = userId,
                ComputerId = computerOrderViewModel.Computer.ComputerId
            };

            _reviewService.Insert(newReview);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ComputerOrders(SortParams sortParams)
        {
            var computer = _computerService.GetById(sortParams.Id);

            Expression<Func<IQueryable<Review>, IOrderedQueryable<Review>>>? orderBy = sortParams.SortBy switch
            {
                "Asc" => l => l.OrderBy(r => r.Rating),
                "Desc" => l => l.OrderByDescending(l=> l.Rating),
                _ => null
            } ;

            List<Review>? reviews = _reviewService.GetByPredicate(filter: review => review.ComputerId == computer.ComputerId,
                orderBy: orderBy);

            TempData["listReviews"] = JsonSerializer.Serialize(reviews);

            return RedirectToAction("ComputerOrders","Computer", new { id= computer.ComputerId});
        }

    }

}
