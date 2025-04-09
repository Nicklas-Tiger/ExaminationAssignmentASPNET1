using Data.Entities;
using Domain.Models;

namespace Business.Mapping
{
    public static class UserMapping
    {
        public static UserEntity ToEntity(this User user)
        {
            return new UserEntity
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                JobTitle = user.JobTitle,
                Image = user.Image,
                UserName = user.Email 
            };
        }
    }
}
