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
    IUserRepository _userRepository;
    IMapper _mapper;

    // Constructor for Controller Class
    public UserSalaryEFController(IUserRepository userRepository)
    {
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
        return _userRepository.GetAllUserSalary();
    }

    [HttpGet("GetSingleUserSalary/{UserId}", Name = "GetSingleUserSalaryEF")]
    public UserSalary GetSingleUserSalary(int UserId)
    {
        return _userRepository.GetSingleUserSalary(UserId);
    }

    [HttpPut("EditUserSalary", Name = "EditUserSalaryEF")]
    public IActionResult EditUser(UserSalary userSalary)
    {
        // find user by Id
        UserSalary? userSalaryDb = _userRepository.GetSingleUserSalary(userSalary.UserId);
        // update User
        userSalaryDb.Salary = userSalary.Salary;
        if (_userRepository.SaveChanges()) return Ok(); // save updated user
        else throw new Exception("Failed to Edit User Salary!");
    }

    [HttpPost("AddUserSalary", Name = "AddUserSalaryEF")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        // get max index so far
        int newIdx = _userRepository.MaxIndex<UserSalary>(userSalary) + 1;
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
        UserSalary? userDb = _userRepository.GetSingleUserSalary(UserId);
        _userRepository.RemoveEntity<UserSalary>(userDb);

        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception("Failed to Delete User Salary with EF");
    }
}
