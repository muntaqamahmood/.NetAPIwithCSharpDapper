namespace DotNetAPI.DTOs
{
    // partial so that we can add to our Models on the fly
    public partial class UserDTO
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";

        public bool Active { get; set; }

        // public User(int UserId, string JobTitle, string Department)
        // {
        //     this.UserId = UserId;
        //     this.JobTitle = JobTitle;
        //     this.Department = Department;
        // }
    }
}