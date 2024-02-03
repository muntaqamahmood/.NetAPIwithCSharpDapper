namespace DotNetAPI.Models
{
    public partial class Posts
    {
        public int PostId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
        public DateTime PostCreated { get; set; }
        public DateTime PostUpdated { get; set; }
    }
}