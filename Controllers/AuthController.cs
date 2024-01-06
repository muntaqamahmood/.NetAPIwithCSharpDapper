using System.Security.Cryptography;
using System.Text;
using DotNetAPI.Data;
using DotNetAPI.DTOs;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotNetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dataContextDapper = new DataContextDapper(configuration);
        }

        [HttpPost("Register", Name = "Register")]
        public IActionResult RegisterUser(UserForRegistrationDto userToRegister)
        {
            if (userToRegister.Password == userToRegister.PasswordConfirm)
            {
                string sqlCheckUserExist = @"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" + userToRegister.Email + "'";
                // if user already exists
                IEnumerable<string> existingUsers = _dataContextDapper.LoadData<string>(sqlCheckUserExist);
                if (existingUsers.Count() == 0)
                {
                    Console.WriteLine("here");
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }
                    string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value +
                        Convert.ToBase64String(passwordSalt);
                    Console.WriteLine(passwordSaltPlusString);
                    // hashing using above salt
                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: userToRegister.Password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                    );
                    string sqlToAdd = @"
                        INSERT INTO TutorialAppSchema.Auth(
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES ('" + userToRegister.Email +
                        "', @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;
                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);

                    if (_dataContextDapper.ExecuteSqlWithParameters(sqlToAdd, sqlParameters))
                    {
                        return Ok();
                    }
                    throw new Exception($"Failed to Register User with Email: {userToRegister.Email}");
                }
                throw new Exception("User already exists.");
            }
            else throw new Exception("Passwords do not Match! Try again.");

            // string sql = @"
            // INSERT INTO TutorialAppSchema.Users(
            //     [FirstName],
            //     [LastName],
            //     [Email],
            //     [Gender],
            //     [Active]
            // ) VALUES ('" + user.Email +
            // "','" + user.LastName +
            // "','" + user.Email +
            // "','" + user.Gender +
            // "','" + user.Active +
            // "')";
        }

        // [HttpPost("Login", Name = "Login")]
        // public IActionResult LoginUser(UserForLoginDto userToLogin)
        // {


        //     string sql = @"
        //     INSERT INTO TutorialAppSchema.Users(
        //         [FirstName],
        //         [LastName],
        //         [Email],
        //         [Gender],
        //         [Active]
        //     ) VALUES ('" + user.Email +
        //     "','" + user.LastName +
        //     "','" + user.Email +
        //     "','" + user.Gender +
        //     "','" + user.Active +
        //     "')";
        //     bool check = _dapper.ExecuteSqlWithRowCount(sql);
        //     if (check == true) return Ok();
        //     else throw new Exception("Failed to Add User!");
        // }


    }
}