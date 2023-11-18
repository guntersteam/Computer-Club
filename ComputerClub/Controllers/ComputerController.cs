using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace ComputerClub.Controllers;

public class ComputerController : Controller
{
    private readonly ComputerService _computerService;

    public ComputerController(ComputerService computerService)
    {
        _computerService = computerService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var computers = _computerService.GetByPredicate();
        if (computers == null)
        {
            return RedirectToAction("Privacy", "Home");
        }
        return View(computers);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        _computerService.Delete(id);
        _computerService.Commit();
        return RedirectToAction("Index", "Computer");
    }

    [HttpGet]
    public IActionResult AddForm()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> InsertInto(ComputerViewModel computerViewModel)
    {
        computerViewModel.IsReserved = false;

        var newComputer = new Computer
        {
            ModelName = computerViewModel.ModelName,
            PriceForHour = computerViewModel.PriceForHour,
            IsReserved = computerViewModel.IsReserved
        };

        _computerService.Insert(newComputer);
        _computerService.Commit();

        return RedirectToAction("Index", "Computer");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var computer = _computerService.GetById(id);
        var computerViewModel = new ComputerViewModel
        {
            ComputerId = id,
            ModelName = computer.ModelName,
            PriceForHour = computer.PriceForHour,
            IsReserved = computer.IsReserved

        };
        return View(computerViewModel);
    }

    [HttpPost]
    public IActionResult Edit(ComputerViewModel computerViewModel)
    {

        var updatedComputer = new Computer
        {
            ComputerId = computerViewModel.ComputerId,
            ModelName = computerViewModel.ModelName,
            PriceForHour = computerViewModel.PriceForHour,
            IsReserved = computerViewModel.IsReserved
        };

        _computerService.Update(updatedComputer);
        _computerService.Commit();

        return RedirectToAction("Index", "Computer");
    }

    [HttpGet]
    public IActionResult ComputersView(string userOption, int? MinNumber, int? MaxNumber, bool? State)
    {
        var computers = _computerService.GetAll();

        var computerParams = new ComputerParametresViewModel
        {
            Computers = computers,
        };

        if (!userOption.IsNullOrEmpty())
        {
            var sortedComputers = _computerService.GetByPredicate().Where(computer => computer.ModelName.Contains(userOption)).ToList();
            var updatedcomputerParams = new ComputerParametresViewModel
            {
                Computers = sortedComputers,
            };
            return View(updatedcomputerParams);
        }

        if (MinNumber != null && MaxNumber != null)
        {
            computers = _computerService.GetByPredicate().Where(computer => computer.PriceForHour >= MinNumber && computer.PriceForHour <= MaxNumber).ToList();
            var updatedcomputerParams = new ComputerParametresViewModel
            {
                Computers = computers,
            };
            return View(updatedcomputerParams);
        }

        if(State != null)
        {
            var sortedComputers = _computerService.GetByPredicate().Where(computer => computer.IsReserved == State).ToList();
            var updatedcomputerParams = new ComputerParametresViewModel
            {
                Computers = sortedComputers,
            };
            return View(updatedcomputerParams);
        }
        

        return View(computerParams);
    }

    //[HttpPost]
    //public IActionResult GiveFilteredComputers(string userOption)
    //{
    //    var computers = _computerService.GetAll();

    //    if (userOption != null)
    //    {
    //        var sortedComputers = _computerService.GetByPredicate().Where(computer => computer.ModelName == userOption).ToList();
    //        return View(sortedComputers);
    //    }
    //    return View(computers);
    //}
}
