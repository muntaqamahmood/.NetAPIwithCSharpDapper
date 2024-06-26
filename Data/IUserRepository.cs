using DotNetAPI.Models;

namespace DotNetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);
        public IEnumerable<User> GetAllUsers();
        public User GetSingleUser(int UserId);
        public IEnumerable<UserSalary> GetAllUserSalary();
        public UserSalary GetSingleUserSalary(int UserId);
        public IEnumerable<UserJobInfo> GetAllUserJobInfo();
        public UserJobInfo GetSingleUserJobInfo(int UserId);
        public int MaxIndex<T>(T entity);

    }
}