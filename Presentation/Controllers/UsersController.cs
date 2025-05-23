﻿using Business.Services;
using Data.Contexts;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using System;
using System.Threading.Tasks;

namespace Presentation.Controllers;

[Authorize]
public class UsersController(IUserService userService, DataContext context, INotificationService notificationService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly DataContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    [Route("admin/members")]
    public async Task<IActionResult> Index()
    {
        var userResult = await _userService.GetUsersAsync();

        if (!userResult.Succeeded)
        {
            TempData["EditError"] = userResult.Error;
            return View(new UsersViewModel { Users = new List<UserViewModel>() });
        }

        var usersViewModel = new UsersViewModel
        {

            Users = userResult.Result!.Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName!,
                LastName = u.LastName!,
                Email = u.Email,
                Image = u.Image!,
                JobTitle = u.JobTitle!,
                PhoneNumber = u.PhoneNumber!
            }).ToList()
        };

        return View(usersViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AddError"] = "Invalid input. Please check the form and try again.";
            return RedirectToAction("Index");
        }

        var formData = model.MapTo<AddUserFormData>();
        var result = await _userService.CreateUserAsync(formData);

        if (result.Succeeded)
        {
            var notificationFormData = new NotificationFormData
            {
                NotificationTypeId = 1,
                NotificationTargetId = 1,
                Message = $"{formData.FirstName} has been added as a member.",
                Image = result.Result
            };

            await _notificationService.AddNotificationAsync(notificationFormData);
            return RedirectToAction("Index");
        }


        TempData["AddError"] = result.Error!;
        return RedirectToAction("Index");
    }

    //[HttpGet]
    //public async Task<IActionResult> Edit(string id)
    //{
    //    if (string.IsNullOrEmpty(id))
    //        return NotFound();

    //    var userResult = await _userService.GetUserByIdAsync(id);
    //    if (userResult is null)
    //        return NotFound();


    //    var vm = new EditUserViewModel
    //    {
    //        Id = userResult.,
    //        FirstName = userResult.FirstName,
    //        LastName = userResult.LastName,
    //        Email = userResult.Email,
    //        // Lägg till fler egenskaper vid behov, t.ex. image, roll, etc.
    //    };

    //    return View(vm);
    //}

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var formData = new EditUserFormData
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,

        };

        var result = await _userService.UpdateUserAsync(formData);
        if (result.Succeeded)
            return RedirectToAction("Index");
        else
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }
    }


    [HttpGet]
    public async Task<JsonResult> SearchUsers(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var users = await _context.Users
            .Where(x => x.FirstName!.Contains(term) || x.LastName!.Contains(term) || x.Email!.Contains(term))
            .Select(x => new { x.Id, imageUrl = x.Image, FullName = $"{x.FirstName} {x.LastName}" })
            .ToListAsync();

        return Json(users);
    }

}
