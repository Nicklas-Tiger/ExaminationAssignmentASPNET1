using Data.Entities;
using Domain.Models;

//Denna kod är helt framtagen av chatgpt för att lösa problemet jag hade att få ut ClientName på projektkorten.

namespace Business.Mapping
{
    public static class ProjectMapping
    {
        public static Project ToDomain(this ProjectEntity entity)
        {
            if (entity == null)
                return null!; 

            var project = new Project
            {
                Id = entity.Id,
                Image = entity.Image,
                ProjectName = entity.ProjectName,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Budget = entity.Budget,
 

                Client = entity.Client == null
                    ? null!
                    : new Client
                    {
                        Id = entity.Client.Id,
                        ClientName = entity.Client.ClientName
                    }
            };

            return project;
        }

        public static IEnumerable<Project> ToDomain(this IEnumerable<ProjectEntity> entities)
            => entities.Select(e => e.ToDomain());
    }
}