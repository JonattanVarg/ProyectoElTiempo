namespace API.Configurations.InitialUsers.Interfaces
{
    public interface IInitialUser
    {
        string Username { get; set; }
        string Email { get; set; }
        string FullName { get; set; }
        string Password { get; set;  }
        string Role { get; set;  }
    }
}