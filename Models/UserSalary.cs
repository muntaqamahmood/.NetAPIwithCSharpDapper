namespace DotNetAPI.Models
{
    // partial so that we can add to our Models on the fly
    public partial class UserSalary
    {
        public int UserId { get; set; }
        public decimal Salary { get; set; }

        // public UserJobInfo(int UserId, string JobTitle, string Department)
        // {
        //     this.UserId = UserId;
        //     this.JobTitle = JobTitle;
        //     this.Department = Department;
        // }
    }
}