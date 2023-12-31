using DotNetAPI.Data;
using DotNetAPI.DTOs;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
// sub namespace so that .net doesnt load up this controller as soon as we run the project
// and save some resource + memory during runtime  
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserSalaryController : ControllerBase
{
    DataContextDapper _dapper;

    // Constructor for Controller Class
    public UserSalaryController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUsersSalary", Name = "GetAllUsersSalary")]
    public IEnumerable<UserSalary> GetAllUsersSalary()
    {
        IEnumerable<UserSalary> userSalaries;
        string sql = @"SELECT [UserId],
            [Salary] FROM TutorialAppSchema.UserSalary";
        userSalaries = _dapper.LoadData<UserSalary>(sql);
        return userSalaries;
    }

    [HttpGet("GetSingleUserSalary/{UserId}", Name = "GetSingleUserSalary")]
    public UserSalary GetSingleUserSalary(int UserId)
    {
        UserSalary userSalary;
        string sql = @"SELECT [UserId],
            [Salary] FROM TutorialAppSchema.UserSalary
        WHERE UserId = " + UserId.ToString();
        userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
        return userSalary;
    }

    [HttpPut("EditUserSalary", Name = "EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        string sql = @"
        UPDATE TutorialAppSchema.UserSalary
            SET [Salary] = '" + userSalary.Salary + "'" + "\n" + " WHERE UserId = " + userSalary.UserId;
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Edit UserSalary!");
    }

    [HttpPost("AddUserSalary", Name = "AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.UserSalary(
            [Salary]
        ) VALUES ('" + userSalary.Salary + "')";
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Add UserSalary!");
    }

    [HttpDelete("DeleteUserSalary/{UserId}", Name = "DeleteUserSalary")]
    public IActionResult DeleteUserSalary(int UserId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = " + UserId.ToString();
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Delete UserSalary!");
    }

    [HttpDelete("DeleteUserSalaryBySalary/{Salary}", Name = "DeleteUserSalaryBySalary")]
    public IActionResult DeleteUserSalaryBySalary(int Salary)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.UserSalary WHERE Salary = " + Salary.ToString();
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Delete UserSalary by Salary!");
    }
}
