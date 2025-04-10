using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class UsersViewModel
{
    public IEnumerable<UserViewModel> Users { get; set; } = null!;
    public AddUserViewModel AddUser { get; set; } = new AddUserViewModel();
    public List<EditUserViewModel> TeamMembers { get; set; } = null!;
}
