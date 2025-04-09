using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Business.Mapping;
using System.Linq.Expressions;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
    Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {

        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var projectEntity = formData.MapTo<ProjectEntity>();

        if (formData.Image != null && formData.Image.Length > 0)
        {
        
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/projects");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(formData.Image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await formData.Image.CopyToAsync(fileStream);


            projectEntity.Image = uniqueFileName;
        }
        else
            projectEntity.Image = null; 





        var statusResult = await _statusService.GetStatusByIdAsync(1);
        var status = statusResult.Result;

        projectEntity.StatusId = status!.Id;

        var result = await _projectRepository.AddAsync(projectEntity);
        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };

    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllRawAsync(
            orderByDescending: true,
            sortBy: s => s.Created,
            where: null,
            includes: new Expression<Func<ProjectEntity, object>>[]
            {
            p => p.User,
            p => p.Status,
            p => p.Client
            }
        );

        if (!response.Succeeded)
        {
            return new ProjectResult<IEnumerable<Project>>
            {
                Succeeded = false,
                StatusCode = response.StatusCode,
                Result = null
            };
        }

        var projectsDomain = response.Result!.ToDomain(); // extension-metod du redan har

        return new ProjectResult<IEnumerable<Project>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = projectsDomain
        };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync(
            where: x => x.Id == id,
            include => include.User,
            include => include.Status,
            include => include.Client
        );

        return response.Succeeded
            ? new ProjectResult<Project>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = response.Result 
            }
            : new ProjectResult<Project>
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Project `{id}` was not found."
            };
    }


    public async Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData)
    {
        if (formData == null || string.IsNullOrEmpty(formData.Id))
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Invalid project data." };

        var projectEntityResponse = await _projectRepository.GetAllRawAsync(
            where: x => x.Id == formData.Id,
            includes: new Expression<Func<ProjectEntity, object>>[]
            {
            x => x.User,
            x => x.Status,
            x => x.Client
            }
        );

        var projectEntity = projectEntityResponse.Result?.FirstOrDefault();
        if (projectEntity == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = "Project not found." };

        // Uppdatera egenskaper
        projectEntity.ProjectName = formData.ProjectName;
        projectEntity.Description = formData.Description;
        projectEntity.StatusId = formData.StatusId;
        projectEntity.ClientId = formData.ClientId;
        projectEntity.UserId = formData.UserId;
        projectEntity.StartDate = formData.StartDate ?? default;
        projectEntity.EndDate = formData.EndDate ?? default;
        projectEntity.Budget = formData.Budget;

        var updateResult = await _projectRepository.UpdateAsync(projectEntity);

        return updateResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error };
    }


    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Invalid id." };

        var existingResponse = await _projectRepository.GetAsync(
            x => x.Id == id,
            x => x.User, x => x.Status, x => x.Client
        );

        if (!existingResponse.Succeeded || existingResponse.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = "Project not found." };

        var projectEntity = existingResponse.Result.MapTo<ProjectEntity>();


        var deleteResult = await _projectRepository.DeleteAsync(projectEntity);

        return deleteResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = deleteResult.StatusCode, Error = deleteResult.Error };
    }


}
