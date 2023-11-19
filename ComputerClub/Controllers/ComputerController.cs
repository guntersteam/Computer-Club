using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
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

    //[HttpGet]
    //public IActionResult ComputersView(Dictionary<string, string?> searchParams)
    //{

    //    var computers = _computerService.GetAll();
    //    var query = computers.AsQueryable();

    //    foreach (KeyValuePair<string, string> keyValuePair in searchParams)
    //    {
    //        if (string.IsNullOrEmpty(keyValuePair.Value))
    //        {
    //            continue;
    //        }

    //        switch (keyValuePair.Key)
    //        {
    //            case "ModelName":
    //                query = query.Where(computer => computer.ModelName.Contains(keyValuePair.Value));
    //                break;
    //            case "MinNumber":
    //                if (int.TryParse(keyValuePair.Value, out int price))
    //                    query = query.Where(computer => computer.PriceForHour >= price);
    //                break;
    //            case "MaxNumber":
    //                if (int.TryParse(keyValuePair.Value, out int prices))
    //                    query = query.Where(computer => computer.PriceForHour <= prices);
    //                break;
    //            case "State":
    //                if (bool.TryParse(keyValuePair.Value, out bool isReserved))
    //                    query = query.Where(computer => computer.IsReserved == isReserved);
    //                break;
    //        }
    //    }

    //    var computersFiltered = query.ToList();

    //    var computerParams = new ComputerParametresViewModel
    //    {
    //        Computers = computersFiltered,
    //    };

    //    TempData["SearchParams"] = searchParams.Values.ToString();

    //    return View(computerParams);
    //}

    [HttpGet]
    public IActionResult ComputersView(Dictionary<string, string?> parametres, string modelName, string minNumber, string maxNumber, string state)
    {

        var computers = _computerService.GetAll();
        var query = computers.AsQueryable();

        if (modelName != null)
            query = query.Where(computer => computer.ModelName.Contains(modelName));
        if (minNumber != null)
        {
            if (int.TryParse(minNumber, out int prices))
                query = query.Where(computer => computer.PriceForHour >= prices);
        }
        if (maxNumber != null)
        {
            if (int.TryParse(maxNumber, out int prices))
                query = query.Where(computer => computer.PriceForHour <= prices);
        }

        if (state != null)
        {
            if (bool.TryParse(state, out bool isReserved))
                query = query.Where(computer => computer.IsReserved == isReserved);
        }

        var savedQuery = query.ToList();

        foreach (KeyValuePair<string, string> pair in parametres)
        {
            if (pair.Key != "Asc" && pair.Key != "Desc")
                continue;
            switch (pair.Value)
            {
                case "ModelName":
                    if (pair.Key == "Asc")
                        query = _computerService.GetByPredicate().OrderBy(model => model.ModelName).Intersect(savedQuery).AsQueryable();
                    else
                       query = _computerService.GetByPredicate().OrderByDescending(model => model.ModelName).Intersect(savedQuery).AsQueryable();
                    break;
                case "PriceForHour":
                    if (pair.Key == "Asc")
                        query = _computerService.GetByPredicate().OrderBy(model => model.PriceForHour).Intersect(savedQuery).AsQueryable();
                    else
                        query = _computerService.GetByPredicate().OrderByDescending(model => model.PriceForHour).Intersect(savedQuery).AsQueryable();
                    break;

            }
        }

        var computersFiltered = query.ToList();

        var computerParams = new ComputerParametresViewModel
        {
            Computers = computersFiltered,
        };

        ViewBag.ModelName = modelName;
        ViewBag.MinNumber = minNumber;
        ViewBag.MaxNumber = maxNumber;
        ViewBag.State = state;

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
