namespace DotNetAPI.Models
{
    public partial class PostToEditDTO
    {
        // to know which post to edit with PostId
        public int PostId { get; set; }
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
    }
}