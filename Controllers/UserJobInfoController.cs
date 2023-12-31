using DotNetAPI.Data;
using DotNetAPI.DTOs;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
// sub namespace so that .net doesnt load up this controller as soon as we run the project
// and save some resource + memory during runtime  
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserJobInfoController : ControllerBase
{
    DataContextDapper _dapper;

    // Constructor for Controller Class
    public UserJobInfoController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUserJobInfo", Name = "GetAllUserJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {
        IEnumerable<UserJobInfo> userJobInfos;
        string sql = @"SELECT [UserId],
            [JobTitle],
            [Department] FROM TutorialAppSchema.UserJobInfo";
        userJobInfos = _dapper.LoadData<UserJobInfo>(sql);
        return userJobInfos;
    }

    [HttpGet("GetSingleUserJobInfo/{UserId}", Name = "GetSingleUserJobInfo")]
    public UserJobInfo GetSingleUserJobInfo(int UserId)
    {
        UserJobInfo userJobInfo;
        string sql = @"SELECT [UserId],
            [JobTitle],
            [Department] FROM TutorialAppSchema.UserJobInfo
        WHERE UserId = " + UserId.ToString();
        userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
        return userJobInfo;
    }

    [HttpPut("EditUserJobInfo", Name = "EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
        UPDATE TutorialAppSchema.UserJobInfo
            SET [JobTitle] = '" + userJobInfo.JobTitle + "', [Department] = '"
            + userJobInfo.Department + "'" + "\n" +
            " WHERE UserId = " + userJobInfo.UserId;
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Edit UserJobInfo!");
    }

    [HttpPost("AddUserJobInfo", Name = "AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.UserJobInfo(
            [JobTitle],
            [Department]
        ) VALUES ('" + userJobInfo.JobTitle
        + "','" + userJobInfo.Department
        + "')";
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Add UserJobInfo!");
    }

    [HttpDelete("DeleteUserJobInfo/{UserId}", Name = "DeleteUserJobInfo")]
    public IActionResult DeleteUserJobInfo(int UserId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = " + UserId.ToString();
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Delete UserJobInfo!");
    }

}
