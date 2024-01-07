using System.Security.Cryptography;
using System.Text;
using DotNetAPI.Data;
using DotNetAPI.DTOs;
using System.Data;
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
                // Count() is used where Any() could be used instead to improve performance (CA1827)
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }
                    byte[] passwordHash = GetPasswordHash(userToRegister.Password, passwordSalt);
                    string sqlToAdd = @"
                        INSERT INTO TutorialAppSchema.Auth(
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES ('" + userToRegister.Email +
                        "', @PasswordHash, @PasswordSalt)";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;
                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);
                    // Register User
                    if (_dataContextDapper.ExecuteSqlWithParameters(sqlToAdd, sqlParameters))
                    {
                        // Add user to Users Table
                        string sqlToAddUser = @"INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]
                        ) VALUES ('" + userToRegister.FirstName +
                        "','" + userToRegister.LastName +
                        "','" + userToRegister.Email +
                        "','" + userToRegister.Gender +
                        "', 1)";
                        if (_dataContextDapper.ExecuteSqlWithRowCount(sqlToAddUser)) return Ok();
                        else throw new Exception("Failed to Add User to Table");
                    }
                    else throw new Exception($"Failed to Register User with Email: {userToRegister.Email}");
                }
                else throw new Exception("User already exists.");
            }
            else throw new Exception("Passwords do not Match! Try again.");
        }

        [HttpPost("Login", Name = "Login")]
        public IActionResult LoginUser(UserForLoginDto userToLogin)
        {
            string sql = @"SELECT
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" + userToLogin.Email + "'";
            UserForLoginConfirmationDto userForConfirmation = _dataContextDapper.LoadDataSingle<UserForLoginConfirmationDto>(sql);
            // Console.WriteLine($"Password is {user.PasswordHash[0]}, {user.PasswordSalt[0]}");

            byte[] passwordHash = GetPasswordHash(userToLogin.Password, userForConfirmation.PasswordSalt);

            // if (passwordHash == userForConfirmation.PasswordHash) // won't work, this is comparing the memeory address where these byte[] are stored
            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }
            return Ok();
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // get our PasswordKey from Config
            string passwordSaltPlusString = _configuration.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);
            // hashing using above salt
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

    }
}