using Data.Entities;
using Domain.Models;

/*
Denna kod är helt framtagen med hjälp av chatgpt för att lösa problemet jag hade att få ut ClientName på projektkorten.
Den behövdes för att mitt tidigare sätt var att ladda med hjälp av Tselect och då fick jag inte med ClientName.
Detta orsakade NullReferenceException i vyn eftersom .Client blev null trots att ClientId fanns i databasen.
Denna lösning är ett resultat av breakpoints som du föreslog, då jag kunde förstå att jag faktiskt får in allt jag behöver men det kommer bara inte ut i gränssnittet, och ChatGPT som hjälp.
*/

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
                ? new Client { ClientName = "No client" }
                : new Client
                {
                    Id = entity.Client.Id,
                    ClientName = entity.Client.ClientName
                },

                Status = entity.Status == null
                ? new Status {  }
                : new Status
                {
                    Id = entity.Status.Id,
                    StatusName = entity.Status.StatusName
                },

                    User = entity.User == null
                ? new User { }
                : new User
                {
                    Id = entity.User.Id,
                    FirstName = entity.User.FirstName,
                    LastName = entity.User.LastName,
                }

            };

            return project;
        }

        public static IEnumerable<Project> ToDomain(this IEnumerable<ProjectEntity> entities)
            => entities.Select(e => e.ToDomain());

        public static ClientEntity ToEntity(this Client client)
        {
            return new ClientEntity
            {
                Id = client.Id,
                ClientName = client.ClientName
            };
        }
    }


}