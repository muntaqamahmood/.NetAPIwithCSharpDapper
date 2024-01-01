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
    }
}