using DotNetAPI.Models;

namespace DotNetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;

        public UserRepository(IConfiguration configuration)
        {
            _entityFramework = new DataContextEF(configuration);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null) _entityFramework.Add(entityToAdd);
        }
        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove != null) _entityFramework.Remove(entityToRemove);
        }

        public IEnumerable<User> GetAllUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }
        public User GetSingleUser(int UserId)
        {
            User? user = _entityFramework.Users.Where(u => u.UserId == UserId).FirstOrDefault<User>();
            return user ?? throw new Exception($"User with UserId: {UserId} not found!");
        }
        public IEnumerable<UserSalary> GetAllUserSalary()
        {
            IEnumerable<UserSalary> userSalaries = _entityFramework.UserSalary.ToList<UserSalary>();
            return userSalaries;
        }
        public UserSalary GetSingleUserSalary(int UserId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == UserId).FirstOrDefault<UserSalary>();
            return userSalary ?? throw new Exception($"UserSalary with UserId: {UserId} not found!");
        }
        public IEnumerable<UserJobInfo> GetAllUserJobInfo()
        {
            IEnumerable<UserJobInfo> userJobInfos = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
            return userJobInfos;
        }
        public UserJobInfo GetSingleUserJobInfo(int UserId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo.Where(u => u.UserId == UserId).FirstOrDefault<UserJobInfo>();
            return userJobInfo ?? throw new Exception($"UserJobInfo with UserId: {UserId} not found!");
        }
        public int MaxIndex<T>(T entity)
        {
            return entity switch
            {
                UserJobInfo => _entityFramework.UserJobInfo.Max(u => u.UserId),
                UserSalary => _entityFramework.UserSalary.Max(u => u.UserId),
                _ => _entityFramework.Users.Max(u => u.UserId),
            };
        }
    }
}