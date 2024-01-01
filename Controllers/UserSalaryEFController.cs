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
public class UserSalaryEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IUserRepository _userRepository;
    IMapper _mapper;

    // Constructor for Controller Class
    public UserSalaryEFController(IConfiguration config, IUserRepository userRepository)
    {
        _entityFramework = new DataContextEF(config);
        _userRepository = userRepository;
        // https://docs.automapper.org/en/stable/Getting-started.html
        _mapper = new Mapper(new MapperConfiguration(config =>
        {
            config.CreateMap<UserDTO, User>();
        }));
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUserSalary/", Name = "GetAllUserSalaryEF")]
    public IEnumerable<UserSalary> GetAllUserSalary()
    {
        IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList<UserSalary>();
        return userSalaries;
    }

    [HttpGet("GetSingleUserSalary/{UserId}", Name = "GetSingleUserSalaryEF")]
    public UserSalary GetSingleUserSalary(int UserId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == UserId).FirstOrDefault<UserSalary>();
        if (userSalary != null) return userSalary;
        else throw new Exception("Failed to Get Single User Salary");
    }

    [HttpPut("EditUserSalary", Name = "EditUserSalaryEF")]
    public IActionResult EditUser(UserSalary userSalary)
    {
        // find user by Id
        UserSalary? userSalaryDb = _entityFramework.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault<UserSalary>();
        if (userSalaryDb != null)
        {
            // update User
            userSalaryDb.Salary = userSalary.Salary;
            if (_userRepository.SaveChanges()) return Ok(); // save updated user
            else throw new Exception("Failed to Edit User Salary!");
        }
        else throw new Exception("userSalary from DB was null!");
    }

    [HttpPost("AddUserSalary", Name = "AddUserSalaryEF")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        // get max index so far
        int newIdx = _entityFramework.UserSalary.Max(u => u.UserId) + 1;
        UserSalary userSalaryDb = new UserSalary
        {
            Salary = userSalary.Salary,
            UserId = newIdx
        };
        // userSalaryDb.UserId = userSalary.
        _userRepository.AddEntity<UserSalary>(userSalaryDb); // dont forget the .Users
        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception("Failed to Add User Salary with EF");
    }

    [HttpDelete("DeleteUserSalary/{UserId}", Name = "DeleteUserSalaryEF")]
    public IActionResult DeleteUserSalary(int UserId)
    {
        UserSalary? userDb = _entityFramework.UserSalary.Where(u => u.UserId == UserId).FirstOrDefault<UserSalary>();
        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserSalary>(userDb);

            if (_userRepository.SaveChanges()) return Ok();
            else throw new Exception("Failed to Delete User Salary with EF");
        }
        else throw new Exception("User not found!");
    }
}
