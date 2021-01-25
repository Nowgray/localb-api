using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localbanda.Helpers;
using Localbanda.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Localbanda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class PostsController : Controller
    {
        private readonly LocalbandaDbContext _context;

        public PostsController(LocalbandaDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet("GetAllPostsForAdmin")]
        public async Task<PostListResponse> GetAllPostsForAdmin(string login_Key, bool isApproved, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (tk.ValidateToken(login_Key))
                {
                    if (isApproved)
                    {
                        objApiResponse.posts = (from post in _context.Posts
                                                join user in _context.Users on post.User_Id equals user.User_Id
                                                select new Posts
                                                {
                                                    Body = post.Body,
                                                    userName = user.Full_Name,
                                                    Post_Id = post.Post_Id,
                                                    DTS = post.DTS,
                                                    IsDeleted = post.IsDeleted,
                                                    IsApproved = post.IsApproved,
                                                    User_Id = post.User_Id,
                                                    Job_Location = post.Job_Location,
                                                    Users = user
                                                }).Where(x => x.IsApproved).OrderByDescending(x => x.Post_Id).Skip(ToSkip).Take(ToTake).ToList();
                    }
                    else
                    {
                        objApiResponse.posts = (from post in _context.Posts
                                                join user in _context.Users on post.User_Id equals user.User_Id
                                                select new Posts
                                                {
                                                    Body = post.Body,
                                                    userName = user.Full_Name,
                                                    Post_Id = post.Post_Id,
                                                    DTS = post.DTS,
                                                    IsDeleted = post.IsDeleted,
                                                    IsApproved = post.IsApproved,
                                                    User_Id = post.User_Id,
                                                    Job_Location = post.Job_Location,
                                                    Users = user
                                                }).Where(x => !x.IsApproved).OrderByDescending(x => x.Post_Id).Skip(ToSkip).Take(ToTake).ToList();
                    }

                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                    await _context.SaveChangesAsync();
                    return objApiResponse;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
        }

        // GET: api/Posts
        [HttpGet("GetAllPosts")]
        public async Task<PostListResponse> GetAllPosts(string login_Key, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    objApiResponse.posts = (from post in _context.Posts
                                            join user in _context.Users on post.User_Id equals user.User_Id
                                            where !post.IsDeleted && (post.IsApproved || post.User_Id == objToken.UserId)
                                            select new Posts
                                            {
                                                Body = post.Body,
                                                userName = user.Full_Name,
                                                Post_Id = post.Post_Id,
                                                DTS = post.DTS,
                                                IsDeleted = post.IsDeleted,
                                                IsApproved = post.IsApproved,
                                                User_Id = post.User_Id,
                                                Job_Location = post.Job_Location,
                                                Users = user
                                            }).OrderByDescending(x => x.Post_Id).Skip(ToSkip).Take(ToTake).ToList();

                    var NewPosts = _context.Posts
                                .Where(c => !_context.UsersPostSync.Where(x => x.User_Id == objToken.UserId).Select(b => b.Post_Id)
                                .Contains(c.Post_Id)
                                ).ToList();
                    if (NewPosts != null && NewPosts.Count > 0)
                    {
                        try
                        {
                            foreach (Posts syncedPost in NewPosts)
                            {
                                try
                                {
                                    //var post = _context.Posts.FirstOrDefault();
                                    //UsersPostSync userPostSync = _context.UsersPostSync.Where(x => x.Post_Id == syncedPost.Post_Id && x.User_Id == syncedPost.User_Id).FirstOrDefault();
                                    //if (userPostSync == null)
                                    //{
                                    UsersPostSync userPostSync = new UsersPostSync();
                                    userPostSync.User_Id = objToken.UserId;
                                    userPostSync.Post_Id = syncedPost.Post_Id;
                                    userPostSync.Is_Synced = true;

                                    _context.UsersPostSync.Add(userPostSync);
                                    _context.Entry(userPostSync).State = EntityState.Added;
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    GNF.SaveException(ex, _context);
                                }
                            }

                        }

                        catch (Exception ex)
                        {
                            objApiResponse.Message = "Exception: " + ex.Message;
                            objApiResponse.Status = Messages.APIStatusError;
                            GNF.SaveException(ex, _context);
                            return objApiResponse;
                        }
                    }

                    //objApiResponse.posts = lstPosts;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                    await _context.SaveChangesAsync();
                    return objApiResponse;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
        }

        [HttpGet("IsNewPosts")]
        public async Task<NewPostResponse> IsNewPosts(string login_Key)
        {

            NewPostResponse objApiResponse = new NewPostResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {


                    var NewPostsCount = _context.Posts
                                .Where(c => !_context.UsersPostSync.Where(x => x.User_Id == objToken.UserId).Select(b => b.Post_Id)
                                .Contains(c.Post_Id)
                                //&& !_context.UsersPostSync.Select(b => b.User_Id).Contains(objToken.UserId)
                                ).Count();



                    objApiResponse.NewPostCount = NewPostsCount;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                    await _context.SaveChangesAsync();
                    return objApiResponse;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
        }

        // GET: api/Posts/5
        [HttpGet("GetPostByUserId")]
        public async Task<PostListResponse> GetPostByUserId(string login_Key)
        {
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    List<Posts> lstPosts = new List<Posts>();
                    objApiResponse.posts = (from post in _context.Posts
                                            join user in _context.Users on post.User_Id equals user.User_Id
                                            where !post.IsDeleted && post.User_Id == objToken.UserId && post.IsApproved
                                            select new Posts
                                            {
                                                Body = post.Body,
                                                userName = user.Full_Name,
                                                Post_Id = post.Post_Id,
                                                DTS = post.DTS,
                                                IsDeleted = post.IsDeleted,
                                                User_Id = post.User_Id,
                                                IsApproved = post.IsApproved,
                                                Job_Location = post.Job_Location,
                                                Users = user
                                            }).OrderByDescending(x => x.Post_Id).ToList();

                    //lstPosts = await _context.Posts.Where(x => x.User_Id == objToken.UserId && x.IsDeleted == false).ToListAsync();
                    //objApiResponse.posts = lstPosts;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

        // GET: api/Posts/5
        [HttpGet("GetPostById")]
        public async Task<PostResponse> GetPostById(string login_Key, int Id)
        {
            PostResponse objApiResponse = new PostResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    objApiResponse.Post = await (from post in _context.Posts
                                                 join user in _context.Users on post.User_Id equals user.User_Id
                                                 where !post.IsDeleted && post.Post_Id == Id && post.IsApproved
                                                 select new Posts
                                                 {
                                                     Body = post.Body,
                                                     userName = user.Full_Name,
                                                     Post_Id = post.Post_Id,
                                                     DTS = post.DTS,
                                                     IsDeleted = post.IsDeleted,
                                                     IsApproved = post.IsApproved,
                                                     User_Id = post.User_Id,
                                                     Job_Location = post.Job_Location,
                                                     Users = user
                                                 }).FirstOrDefaultAsync();

                    //lstPosts = await _context.Posts.Where(x => x.User_Id == objToken.UserId && x.IsDeleted == false).ToListAsync();
                    //objApiResponse.posts = lstPosts;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

        [HttpPost("NewPost")]
        public async Task<PostListResponse> NewPost(PostsRequest objRequest)
        {
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                string ValidationMsg = ValidationHelper.GetErrorListFromModelState(ModelState);
                if (!string.IsNullOrEmpty(ValidationMsg))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = ValidationMsg;
                    objApiResponse.ValidationMessage = ValidationMsg;
                    return objApiResponse;
                }
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(objRequest.Login_Key);
                if (!tk.ValidateToken(objRequest.Login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }

                var objPost = _context.Posts.Find(objRequest.Post.Post_Id);

                if (objPost != null)
                {
                    objPost.User_Id = objToken.UserId;
                    objPost.Body = objRequest.Post.Body;
                    objPost.Job_Location = objRequest.Post.Job_Location;
                    objPost.IsDeleted = false;
                    objPost.IsApproved = false;
                    objPost.DTS = DateTime.Now;

                    _context.Posts.Add(objPost);
                    _context.Entry(objPost).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    objPost = new Posts();

                    objPost.User_Id = objToken.UserId;
                    objPost.Body = objRequest.Post.Body;
                    objPost.Job_Location = objRequest.Post.Job_Location;
                    objPost.IsDeleted = false;
                    objPost.IsApproved = false;
                    objPost.DTS = DateTime.Now;

                    _context.Posts.Add(objPost);
                    _context.Entry(objPost).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }

                #region Upload Images
                //try
                //{
                //    ImageBase64 image = objRequest.Photos;
                //    if (image != null && image.base64image != null && image.fileName != null && image.fileExtention != null)
                //    {
                //        try
                //        {
                //            string fileName = ImageHelper.UploadImage(image, @GlobalConfig.UserProfilePhotoUpload, objUser.User_Id, _context);
                //            objUser.Profile_URL = fileName;
                //        }
                //        catch (Exception ex)
                //        {
                //            GNF.SaveException(ex, _context);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    objApiResponse.Message = "Exception: " + ex.Message;
                //    objApiResponse.Status = Messages.APIStatusError;
                //    GNF.SaveException(ex, _context);
                //    return objApiResponse;
                //}
                #endregion
                await _context.SaveChangesAsync();
                //Sending Notification to Admin for new post
                await PushHelper.NotifyToAdminForNewPost(_context, objPost.Post_Id);

                objApiResponse.posts = (from post in _context.Posts
                                        join user in _context.Users on post.User_Id equals user.User_Id
                                        where !post.IsDeleted && post.Post_Id == objPost.Post_Id && post.IsApproved
                                        select new Posts
                                        {
                                            Body = post.Body,
                                            userName = user.Full_Name,
                                            Post_Id = post.Post_Id,
                                            DTS = post.DTS,
                                            IsDeleted = post.IsDeleted,
                                            IsApproved = post.IsApproved,
                                            User_Id = post.User_Id,
                                            Job_Location = post.Job_Location,
                                            Users = user
                                        }).OrderByDescending(x => x.Post_Id).ToList();
                objApiResponse.Message = Messages.SuccessMessage;
                objApiResponse.Status = Messages.APIStatusSuccess;

                var NewPosts = _context.Posts
                                .Where(c => !_context.UsersPostSync.Where(x => x.User_Id == objToken.UserId).Select(b => b.Post_Id)
                                .Contains(c.Post_Id)
                                ).ToList();

                if (NewPosts != null && NewPosts.Count > 0)
                {
                    try
                    {
                        foreach (Posts syncedPost in NewPosts)
                        {
                            try
                            {

                                UsersPostSync userPostSync = new UsersPostSync();
                                userPostSync.User_Id = objToken.UserId;
                                userPostSync.Post_Id = syncedPost.Post_Id;
                                userPostSync.Is_Synced = true;

                                _context.UsersPostSync.Add(userPostSync);
                                _context.Entry(userPostSync).State = EntityState.Added;

                            }
                            catch (Exception ex)
                            {
                                GNF.SaveException(ex, _context);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    catch (Exception ex)
                    {
                        objApiResponse.Message = "Exception: " + ex.Message;
                        objApiResponse.Status = Messages.APIStatusError;
                        GNF.SaveException(ex, _context);
                        return objApiResponse;
                    }
                }

            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

        [HttpPost("DeletePost")]
        public async Task<PostListResponse> DeletePost(PostsRequest objRequest)
        {
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                string ValidationMsg = ValidationHelper.GetErrorListFromModelState(ModelState);
                if (!string.IsNullOrEmpty(ValidationMsg))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = ValidationMsg;
                    objApiResponse.ValidationMessage = ValidationMsg;
                    return objApiResponse;
                }
                TokenHelper tk = new TokenHelper(_context);
                if (!tk.ValidateToken(objRequest.Login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }

                var objPost = _context.Posts.Find(objRequest.Post.Post_Id);
                if (objPost != null)
                {
                    objPost.IsDeleted = true;
                    objPost.DTS = DateTime.Now;

                    _context.Posts.Add(objPost);
                    _context.Entry(objPost).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    await _context.SaveChangesAsync();

                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false).ToListAsync();
                    objApiResponse.Message = Messages.PostDeleted;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                }
                else
                {
                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false).ToListAsync();
                    objApiResponse.Message = Messages.ErrorMessage;
                    objApiResponse.Status = Messages.APIStatusError;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

        [HttpPost("ReportPost")]
        public async Task<PostListResponse> ReportPost(PostsRequest objRequest)
        {
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                string ValidationMsg = ValidationHelper.GetErrorListFromModelState(ModelState);
                if (!string.IsNullOrEmpty(ValidationMsg))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = ValidationMsg;
                    objApiResponse.ValidationMessage = ValidationMsg;
                    return objApiResponse;
                }
                TokenHelper tk = new TokenHelper(_context);
                if (!tk.ValidateToken(objRequest.Login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }

                var objPost = _context.Posts.Find(objRequest.Post.Post_Id);
                if (objPost != null)
                {
                    objPost.Report = objRequest.Post.Report + 1;
                    objPost.DTS = DateTime.Now;

                    _context.Posts.Add(objPost);
                    _context.Entry(objPost).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    await _context.SaveChangesAsync();

                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false).ToListAsync();
                    objApiResponse.Message = Messages.SuccessMessage;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                }
                else
                {
                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false).ToListAsync();
                    objApiResponse.Message = Messages.ErrorMessage;
                    objApiResponse.Status = Messages.APIStatusError;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

        // GET: api/Posts
        [HttpGet("FilterPost")]
        public async Task<PostListResponse> FilterPost(string login_Key, string keyword, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;

            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                bool isValid = tk.ValidateToken(login_Key);
                if (isValid)
                {

                    objApiResponse.posts = (from post in _context.Posts
                                            join user in _context.Users on post.User_Id equals user.User_Id
                                            where !post.IsDeleted && post.IsApproved && ((post.Body != null && post.Body.Contains(keyword)) || (post.Job_Location != null && post.Job_Location.Contains(keyword)))
                                            select new Posts
                                            {
                                                Body = post.Body,
                                                userName = user.Full_Name,
                                                Post_Id = post.Post_Id,
                                                DTS = post.DTS,
                                                IsDeleted = post.IsDeleted,
                                                IsApproved = post.IsApproved,
                                                User_Id = post.User_Id,
                                                Job_Location = post.Job_Location,
                                                Users = user
                                            }).OrderByDescending(x => x.Post_Id).Skip(ToSkip).Take(ToTake).ToList();

                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                    await _context.SaveChangesAsync();
                    return objApiResponse;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
        }

        [HttpPost("ApprovePost")]
        public async Task<PostListResponse> ApprovePost(string login_Key, int post_Id)
        {
            PostListResponse objApiResponse = new PostListResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (!tk.ValidateToken(login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }

                var objPost = _context.Posts.Find(post_Id);
                if (objPost != null)
                {
                    objPost.IsApproved = true;
                    objPost.DTS = DateTime.Now;

                    _context.Posts.Add(objPost);
                    _context.Entry(objPost).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    await _context.SaveChangesAsync();

                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false && x.IsApproved).ToListAsync();
                    objApiResponse.Message = Messages.PostDeleted;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                }
                else
                {
                    objApiResponse.posts = await _context.Posts.Where(x => x.IsDeleted == false && x.IsApproved).ToListAsync();
                    objApiResponse.Message = Messages.ErrorMessage;
                    objApiResponse.Status = Messages.APIStatusError;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
                return objApiResponse;
            }
            return objApiResponse;
        }

    }

    public class TestPostCheck
    {
        public int Post_Id { get; set; }
        public int User_Id { get; set; }

    }
}