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

        /*
        Removes an entity of any type T
        */
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
    }
}