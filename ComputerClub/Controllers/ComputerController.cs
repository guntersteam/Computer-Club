﻿using BLL.Services;
using ComputerClub.ViewModels;
using DL.Entities;
using Microsoft.AspNetCore.Mvc;

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
            IsReserved =computer.IsReserved

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

}
