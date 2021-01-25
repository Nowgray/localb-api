using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Localbanda.Models
{
    public class Posts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Post_Id { get; set; }
        public int User_Id { get; set; }
        public string Body { get; set; }
        public string Job_Location { get; set; }
        public int? Report { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? DTS { get; set; }
        [NotMapped]
        public string userName { get; set; }
        [NotMapped]
        public Users Users { get; set; }
    }

    public class PostListResponse : BaseAPIResponse
    {
        public List<Posts> posts { get; set; }
    }
    public class NewPostResponse : BaseAPIResponse
    {
        public int NewPostCount { get; set; }
    }

    public class PostResponse : BaseAPIResponse
    {
        public Posts Post { get; set; }
    }

    public class PostsRequest : BaseAPIRequest
    {
        public Posts Post { get; set; }
    }
}
