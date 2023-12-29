namespace DotNetAPI
{
    // partial so that we can add to our Models on the fly
    public partial class UserJobInfo
    {
        public int UserId { get; set; }
        public string JobTitle { get; set; } = "";
        public string Department { get; set; } = "";

        // public UserJobInfo(int UserId, string JobTitle, string Department)
        // {
        //     this.UserId = UserId;
        //     this.JobTitle = JobTitle;
        //     this.Department = Department;
        // }
    }
}