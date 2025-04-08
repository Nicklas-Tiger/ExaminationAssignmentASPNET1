using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

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

        var response = await _projectRepository.GetAllAsync
            (
                orderByDescending: true,
                sortBy: s => s.Created, where: null,
                include => include.User,
                include => include.Status,
                include => include.Client
            );
        return new ProjectResult<IEnumerable<Project>> { Succeeded = response.Succeeded, StatusCode = response.StatusCode, Result = response.Result };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {

        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
                include => include.Status,
                include => include.Client
            );
        return response.Succeeded
            ? new ProjectResult<Project> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<Project> { Succeeded = false, StatusCode = 404, Error = $"Project `{id}` was not found." };

    }


    public async Task<ProjectResult> UpdateProjectAsync(EditProjectFormData formData)
    {
        if (formData == null || string.IsNullOrEmpty(formData.Id))
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Invalid project data." };

        var existingResponse = await _projectRepository.GetAsync(
            x => x.Id == formData.Id,
            x => x.User, x => x.Status, x => x.Client
        );
        if (!existingResponse.Succeeded || existingResponse.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = "Project not found." };

        var projectEntity = existingResponse.Result.MapTo<ProjectEntity>();

        projectEntity.ProjectName = formData.ProjectName;
        projectEntity.Description = formData.Description;
        projectEntity.StatusId = formData.StatusId;
        projectEntity.StartDate = formData.StartDate.HasValue ? formData.StartDate.Value : default(DateTime); //ChatGPT hjälpte
        projectEntity.EndDate = formData.EndDate.HasValue ? formData.EndDate.Value : default(DateTime); //ChatGPT hjälpte
        projectEntity.Budget = formData.Budget;
        projectEntity.ClientId = formData.ClientId;

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
