using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;

    // Constructor for Controller Class
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    [HttpGet("GetDate", Name = "GetUserDate")]
    public DateTime GetUserDate()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetAllUsers/", Name = "GetAllUsers")]
    public IEnumerable<User> GetAllUsers() // function param is url params
    {
        IEnumerable<User> users;
        string sql = @"SELECT [UserId],
        [FirstName],
        [LastName],
        [Email],
        [Gender],
        [Active] FROM DotNetCourseDatabase.TutorialAppSchema.Users";
        users = _dapper.LoadData<User>(sql);
        return users;
    }

    // [HttpGet("controller/url/{url_params}", Name = "Endpoint Name")]
    [HttpGet("GetSingleUser/{UserId}", Name = "GetSingleUser")]
    public User GetSingleUser(string UserId) // function param is url params
    {
        User user;
        string sql = @"SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
        FROM DotNetCourseDatabase.TutorialAppSchema.Users
        WHERE UserId = " + UserId;
        user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPut("EditUser", Name = "EditUser")]
    public IActionResult EditUser(User user)
    {
        int boolToInt = -1;
        if (user.Active == true) boolToInt = 1;
        else boolToInt = 0;
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
            "', [LastName] = '" + user.LastName +
            "', [Email] = '" + user.Email +
            "', [Gender] = '" + user.Gender +
            "', [Active] = " + boolToInt + "\n" + " WHERE UserId = " + user.UserId;
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Edit User!");
    }

    [HttpPost("AddUser", Name = "AddUser")]
    public IActionResult AddUser(User user)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.Users(
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
        ) VALUES ('" + user.FirstName +
        "','" + user.LastName +
        "','" + user.Email +
        "','" + user.Gender +
        "','" + user.Active +
        "')";
        bool check = _dapper.ExecuteSqlWithRowCount(sql);
        if (check == true) return Ok();
        else throw new Exception("Failed to Add User!");
    }
}
