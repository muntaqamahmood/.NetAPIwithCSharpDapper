using DotNetAPI.Data;
using DotNetAPI.DTOs;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// sub namespace so that .net doesnt load up this controller as soon as we run the project
// and save some resource + memory during runtime  
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;

    // Constructor for Controller Class
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUsers/", Name = "GetAllUsersEF")]
    public IEnumerable<User> GetAllUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetSingleUser/{UserId}", Name = "GetSingleUserEF")]
    public User GetSingleUser(int UserId)
    {
        User? user = _entityFramework.Users.Where(u => u.UserId == UserId).FirstOrDefault<User>();
        if (user != null) return user;
        else throw new Exception("Failed to GetSingleUser");
    }

    [HttpPut("EditUser", Name = "EditUserEF")]
    public IActionResult EditUser(User user)
    {
        // find user by Id
        User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();
        if (userDb != null)
        {
            // update User
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Gender = user.Gender;
            userDb.Email = user.Email;
            if (_entityFramework.SaveChanges() > 0) return Ok(); // save updated user
            else throw new Exception("Failed to Edit User!");
        }
        else throw new Exception("user from DB was null!");
    }

    [HttpPost("AddUser", Name = "AddUserEF")]
    public IActionResult AddUser(UserDTO user)
    {
        User userDb = new User();
        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Gender = user.Gender;
        userDb.Email = user.Email;
        _entityFramework.Users.Add(userDb); // dont forget the .Users
        if (_entityFramework.SaveChanges() > 0) return Ok();
        else throw new Exception("Failed to Add User with EF");
    }

    [HttpDelete("DeleteUser/{UserId}", Name = "DeleteUserEF")]
    public IActionResult DeleteUser(int UserId)
    {
        User? userDb = _entityFramework.Users.Where(u => u.UserId == UserId).FirstOrDefault();
        if (userDb != null)
        {
            _entityFramework.Users.Remove(userDb);

            if (_entityFramework.SaveChanges() > 0) return Ok();
            else throw new Exception("Failed to Delete User with EF");
        }
        else throw new Exception("User not found!");
    }
}
