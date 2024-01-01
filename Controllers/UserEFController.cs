using AutoMapper;
using DotNetAPI.Data;
using DotNetAPI.DTOs;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
// sub namespace so that .net doesnt load up this controller as soon as we run the project
// and save some resource + memory during runtime  
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;

    // Constructor for Controller Class
    public UserEFController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        // https://docs.automapper.org/en/stable/Getting-started.html
        _mapper = new Mapper(new MapperConfiguration(config =>
        {
            config.CreateMap<UserDTO, User>();
        }));
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUsers/", Name = "GetAllUsersEF")]
    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    [HttpGet("GetSingleUser/{UserId}", Name = "GetSingleUserEF")]
    public User GetSingleUser(int UserId)
    {
        return _userRepository.GetSingleUser(UserId);
    }

    [HttpPut("EditUser", Name = "EditUserEF")]
    public IActionResult EditUser(User user)
    {
        // find user by Id
        User? userDb = _userRepository.GetSingleUser(user.UserId);

        // update User
        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Gender = user.Gender;
        userDb.Email = user.Email;
        if (_userRepository.SaveChanges()) return Ok(); // save updated user
        else throw new Exception($"Failed to Save PUT Changes for User with UserId: {userDb.UserId}");
    }

    [HttpPost("AddUser", Name = "AddUserEF")]
    public IActionResult AddUser(UserDTO user)
    {
        User userDb = _mapper.Map<User>(user);
        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception($"Failed to Save POST Changes for User with UserId: {userDb.UserId}");
    }

    [HttpDelete("DeleteUser/{UserId}", Name = "DeleteUserEF")]
    public IActionResult DeleteUser(int UserId)
    {
        User? userDb = _userRepository.GetSingleUser(UserId);
        _userRepository.RemoveEntity<User>(userDb);
        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception($"Failed to Save DELETE Changes for User with UserId: {UserId}");
    }
}
