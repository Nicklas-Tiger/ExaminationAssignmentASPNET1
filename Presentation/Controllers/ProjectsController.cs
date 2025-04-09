using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Models;
using System.Diagnostics;
using System.Security.Claims;
namespace Presentation.Controllers;

[Authorize]
public class ProjectsController(IStatusService statusService, IClientService clientService, IProjectService projectService, IUserService userService) : Controller
{
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;
    private readonly IUserService _userService = userService;

    #region List

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var clients = await GetClientsSelectListAsync();
        var statuses = await GetStatusesSelectListAsync();
        var projects = await GetProjectsAsync();
        var users = await GetUsersSelectListAsync();

        var editProjectViewModels = projects.Select(p => new EditProjectViewModel
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Budget = p.Budget,
            ImageUrl = p.Image,
            ClientId = p.Client.Id, 
            StatusId = p.Status != null ? p.Status.Id : 0, 
            UserId = p.User.Id,
            Clients = clients,
            Statuses = statuses,
            Users = users
        }).ToList();

        var vm = new ProjectsViewModel
        {
            Projects = projects,
            AddProjectViewModel = new AddProjectViewModel() { Clients = clients },
            EditProjectViewModel = editProjectViewModels
        };

        return View(vm);
    }

    #endregion

    #region Add

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var vm = new AddProjectViewModel
        {
            Clients = await GetClientsSelectListAsync(),
        };
        return PartialView("~/Views/Shared/Partials/Project/_AddProjectModal.cshtml", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index");
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var formData = model.MapTo<AddProjectFormData>();
        formData.UserId = currentUserId!;

        var result = await _projectService.CreateProjectAsync(formData);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Projects");
        }
        else
        {
            ViewBag.ErrorMessage = result.Error;
            return RedirectToAction("Index");
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var projectResult = await _projectService.GetProjectAsync(id);
        if (!projectResult.Succeeded || projectResult.Result == null)
            return NotFound();

        var project = projectResult.Result;

        var vm = new EditProjectViewModel
        {
            Id = project.Id,
            ImageUrl = project.Image,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ClientId = project.Client?.Id ?? "",
            StatusId = project.Status?.Id ?? 0,
            UserId = project.User?.Id ?? "",
            Clients = await GetClientsSelectListAsync(),
            Statuses = await GetStatusesSelectListAsync(),
            Users = await GetUsersSelectListAsync()
        };

        return PartialView("~/Views/Shared/Partials/Project/_EditProjectModal.cshtml", vm);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            Debug.WriteLine("❌ ModelState ogiltigt. Fel:");
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Debug.WriteLine($"⛔️ {state.Key}: {error.ErrorMessage}");
                }
            }

            TempData["EditError"] = "Formuläret innehåller fel. Kontrollera fälten.";
            return RedirectToAction("Index");
        }

        var formData = new EditProjectFormData
        {
            Id = model.Id,
            ProjectName = model.ProjectName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = model.ClientId,
            StatusId = model.StatusId,
            UserId = model.UserId,
            Image = model.Image
        };

        var result = await _projectService.UpdateProjectAsync(formData);

        if (!result.Succeeded)
        {
            TempData["EditError"] = result.ErrorMessage ?? "Kunde inte uppdatera projektet.";
            TempData["OpenModalId"] = model.Id;
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }



    #endregion

    #region Helpers

    private async Task<IEnumerable<SelectListItem>> GetClientsSelectListAsync()
    {
        var result = await _clientService.GetClientsAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.ClientName,
        });

        return statusList!;
    }

    private async Task<IEnumerable<SelectListItem>> GetUsersSelectListAsync()
    {
        var result = await _userService.GetUsersAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = $"{s.FirstName} {s.LastName}",
        });
        return statusList!;
    }

    private async Task<IEnumerable<SelectListItem>> GetStatusesSelectListAsync()
    {
        var result = await _statusService.GetStatusesAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.StatusName
        });

        return statusList!;
    }

    private async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        IEnumerable<Project> projects = [];
        try
        {
            var projectResult = await _projectService.GetProjectsAsync();
            if (projectResult.Succeeded && projectResult.Result != null)
                projects = projectResult.Result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            projects = [];
        }

        return projects;
    }

    #endregion


} 