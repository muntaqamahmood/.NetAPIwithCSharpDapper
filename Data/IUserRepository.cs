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
    }
}