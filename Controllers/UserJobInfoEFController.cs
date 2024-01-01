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
    DataContextEF _entityFramework;
    IUserRepository _userRepository;
    IMapper _mapper;

    // Constructor for Controller Class
    public UserJobInfoEFController(IConfiguration config, IUserRepository userRepository)
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
    [HttpGet("GetAllUserJobInfo/", Name = "GetAllUserJobInfoEF")]
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {
        IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
        return userJobInfos;
    }

    [HttpGet("GetSingleUserJobInfo/{UserId}", Name = "GetSingleUserJobInfoEF")]
    public UserJobInfo GetSingleUserJobInfo(int UserId)
    {
        UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == UserId).FirstOrDefault<UserJobInfo>();
        if (userJobInfo != null) return userJobInfo;
        else throw new Exception("Failed to Get Single User JobInfo");
    }

    [HttpPut("EditUserJobInfo", Name = "EditUserJobInfoEF")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        // find user by Id
        UserJobInfo? userJobInfoDb = _entityFramework.UserJobInfo.Where(u => u.UserId == userJobInfo.UserId).FirstOrDefault<UserJobInfo>();
        if (userJobInfoDb != null)
        {
            // update User
            userJobInfoDb.JobTitle = userJobInfo.JobTitle;
            userJobInfoDb.Department = userJobInfo.Department;
            if (_userRepository.SaveChanges()) return Ok(); // save updated user
            else throw new Exception("Failed to Edit User JobInfo!");
        }
        else throw new Exception("userJobInfo from DB was null!");
    }

    [HttpPost("AddUserJobInfo", Name = "AddUserJobInfoEF")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        // get max index so far
        int newIdx = _entityFramework.UserJobInfo.Max(u => u.UserId) + 1;
        Console.WriteLine(newIdx);
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
        UserJobInfo? userDb = _entityFramework.UserJobInfo.Where(u => u.UserId == UserId).FirstOrDefault<UserJobInfo>();
        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userDb);

            if (_userRepository.SaveChanges()) return Ok();
            else throw new Exception("Failed to Delete User JobInfo with EF");
        }
        else throw new Exception("User not found!");
    }

    [HttpDelete("DeleteUserJobInfoByTitle/{JobTitle}", Name = "DeleteUserJobInfoByTitleEF")]
    public IActionResult DeleteUserJobInfoByTitle(string JobTitle)
    {
        UserJobInfo? userDb = _entityFramework.UserJobInfo.Where(u => u.JobTitle == JobTitle).FirstOrDefault<UserJobInfo>();
        if (userDb != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(userDb);

            if (_userRepository.SaveChanges()) return Ok();
            else throw new Exception("Failed to Delete User JobInfo with EF");
        }
        else throw new Exception("User not found!");
    }
}
