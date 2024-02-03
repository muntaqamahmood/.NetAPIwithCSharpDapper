using Microsoft.AspNetCore.Authorization;
using DotNetAPI.Data;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("AllPosts/", Name = "AllPosts")]
        public IEnumerable<Posts> GetAllPosts()
        {
            IEnumerable<Posts> posts;
            string sql = @"SELECT [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated]
            FROM TutorialAppSchema.Posts";
            posts = _dapper.LoadData<Posts>(sql);
            return posts;
        }

        [HttpGet("SinglePost/{PostId}", Name = "SinglePost")]
        public Posts GetSinglePost(int PostId)
        {
            Posts posts;
            string sql = @"SELECT [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated]
            FROM TutorialAppSchema.Posts
            WHERE PostId = " + PostId.ToString();
            posts = _dapper.LoadDataSingle<Posts>(sql);
            return posts;
        }

        [HttpGet("{UserId}/AllUserPosts/", Name = "AllUserPosts")]
        public IEnumerable<Posts> GetAllUserPosts(int UserId)
        {
            IEnumerable<Posts> posts;
            string sql = @"SELECT [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated]
            FROM TutorialAppSchema.Posts
            WHERE UserId = " + UserId.ToString();
            posts = _dapper.LoadData<Posts>(sql);
            return posts;
        }

        [HttpGet("MyPosts/", Name = "MyPosts")]
        public IEnumerable<Posts> GetMyPosts()
        {

            IEnumerable<Posts> posts;
            string sql = @"SELECT [PostId],
            [UserId],
            [PostTitle],
            [PostContent],
            [PostCreated],
            [PostUpdated]
            FROM TutorialAppSchema.Posts
            WHERE UserId = " + this.User.FindFirst("userId")?.Value;
            // gets userId of the user making API call
            // pulls info about Claims from the user's token
            posts = _dapper.LoadData<Posts>(sql);
            if (!posts.Any())
            {
                return Array.Empty<Posts>();
                // throw new Exception("Failed to Get MyPosts!");
            }
            return posts;
        }

        [HttpPost("CreatePost/", Name = "CreatePost")]
        public IActionResult CreatePost(PostToAddDTO postToAdd)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"
                INSERT INTO TutorialAppSchema.Posts(
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated]
                ) VALUES ('" + userId +
                "','" + postToAdd.PostTitle +
                "','" + postToAdd.PostContent +
                "', GETDATE(), GETDATE())";
            bool check = _dapper.ExecuteSqlWithRowCount(sql);
            if (check == true) return Ok();
            else throw new Exception("Failed to Add Post!");
        }

        [HttpPut("EditPost/", Name = "EditPost")]
        public IActionResult EditPost(PostToEditDTO postToEdit)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"
                UPDATE TutorialAppSchema.Posts
                    SET [PostTitle] = '" + postToEdit.PostTitle +
            "', [PostContent] = '" + postToEdit.PostContent +
            "', [PostUpdated] = GETDATE() WHERE PostId = " + postToEdit.PostId.ToString()
            + " AND UserId = " + userId; // or else 2 people with same PostId will get edited
            bool check = _dapper.ExecuteSqlWithRowCount(sql);
            if (check == true) return Ok();
            else throw new Exception("Failed to Edit Post!");
        }

        [HttpDelete("DeletePost/{PostId}", Name = "DeletePost")]
        public IActionResult DeletePost(int PostId)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"
                DELETE FROM TutorialAppSchema.Posts WHERE PostId = " + PostId.ToString()
                + " AND UserId = " + userId;
            bool check = _dapper.ExecuteSqlWithRowCount(sql);
            if (check == true) return Ok();
            else throw new Exception("Failed to Delete Post!");
        }

        [HttpGet("SearchPostsByString/{searchParam}", Name = "Search")]
        public IEnumerable<Posts> SearchPosts(string searchParam)
        {
            IEnumerable<Posts> posts;
            string sql = @"SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
                FROM TutorialAppSchema.Posts
                WHERE PostTitle LIKE '%" + searchParam + "%' OR PostContent LIKE '%" + searchParam + "%'";
            posts = _dapper.LoadData<Posts>(sql);
            return posts;
        }
    }
}