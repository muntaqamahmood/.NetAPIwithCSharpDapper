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
public class UserJobInfoEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;

    // Constructor for Controller Class
    public UserJobInfoEFController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        // https://docs.automapper.org/en/stable/Getting-started.html
        _mapper = new Mapper(new MapperConfiguration(config =>
        {
            config.CreateMap<UserDTO, User>();
        }));
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUserJobInfo/", Name = "GetAllUserJobInfoEF")]
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {
        return _userRepository.GetAllUserJobInfo();
    }

    [HttpGet("GetSingleUserJobInfo/{UserId}", Name = "GetSingleUserJobInfoEF")]
    public UserJobInfo GetSingleUserJobInfo(int UserId)
    {
        return _userRepository.GetSingleUserJobInfo(UserId);
    }

    [HttpPut("EditUserJobInfo", Name = "EditUserJobInfoEF")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        // find user by Id
        UserJobInfo? userJobInfoDb = _userRepository.GetSingleUserJobInfo(userJobInfo.UserId);
        // update User
        userJobInfoDb.JobTitle = userJobInfo.JobTitle;
        userJobInfoDb.Department = userJobInfo.Department;
        if (_userRepository.SaveChanges()) return Ok(); // save updated user
        else throw new Exception("Failed to Edit User JobInfo!");
    }

    [HttpPost("AddUserJobInfo", Name = "AddUserJobInfoEF")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        // get max index so far
        int newIdx = _userRepository.MaxIndex<UserJobInfo>(userJobInfo) + 1;
        UserJobInfo userJobInfoDb = new UserJobInfo
        {
            JobTitle = userJobInfo.JobTitle,
            Department = userJobInfo.Department,
            UserId = newIdx
        };

        _userRepository.AddEntity<UserJobInfo>(userJobInfoDb);
        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception("Failed to Add User JobInfo with EF");
    }

    [HttpDelete("DeleteUserJobInfoJobInfo/{UserId}", Name = "DeleteUserJobInfoEF")]
    public IActionResult DeleteUserJobInfo(int UserId)
    {
        UserJobInfo? userDb = _userRepository.GetSingleUserJobInfo(UserId);
        _userRepository.RemoveEntity<UserJobInfo>(userDb);

        if (_userRepository.SaveChanges()) return Ok();
        else throw new Exception("Failed to Delete User JobInfo with EF");
    }
}
