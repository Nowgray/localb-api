using Localbanda.Helpers;
using Localbanda.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Localbanda.Helpers.enumeration;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Localbanda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly LocalbandaDbContext _context;

        public UsersController(LocalbandaDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet("GetUsersForAdmin")]
        public async Task<UsersListServiceResponse> GetUsersForAdmin(string login_Key, bool isActive, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;

            UsersListServiceResponse objApiResponse = new UsersListServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (tk.ValidateToken(login_Key))
                {
                    List<Users> lstUsers = new List<Users>();
                    if (isActive)
                    {
                        lstUsers = await _context.Users.Where(x => x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    else
                    {
                        lstUsers = await _context.Users.Where(x => !x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    objApiResponse.Users = lstUsers;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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

        // GET: api/Users
        [HttpGet("FilterUsersForAdmin")]
        public async Task<UsersListServiceResponse> FilterUsersForAdmin(string login_Key, string keyword, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;

            UsersListServiceResponse objApiResponse = new UsersListServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (tk.ValidateToken(login_Key))
                {
                    List<Users> lstUsers = new List<Users>();

                    objApiResponse.Users = await _context.Users.OrderBy(x => ((x.Full_Name != null && x.Full_Name.Contains(keyword)) || (x.Title != null && x.Title.Contains(keyword)) || (x.Bio != null && x.Bio.Contains(keyword)) || (x.Email != null && x.Email.Contains(keyword)) || (x.Phone != null && x.Phone.Contains(keyword)))).Skip(ToSkip).Take(ToTake).ToListAsync(); ;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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

        // GET: api/Users
        [HttpGet("GetUsers")]
        public async Task<UsersListServiceResponse> GetUsers(string login_Key, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;

            UsersListServiceResponse objApiResponse = new UsersListServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    List<Users> lstUsers = new List<Users>();
                    if (objToken.User_Type == "Employer")
                    {
                        lstUsers = await _context.Users.Where(x => x.User_Type == "Applicant" && x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    else if (objToken.User_Type == "Applicant")
                    {
                        lstUsers = await _context.Users.Where(x => x.User_Type == "Employer" && x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    else
                    {
                        lstUsers = await _context.Users.OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    objApiResponse.Users = lstUsers;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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

        // GET: api/Users/5
        [HttpGet("GetUserById")]
        public async Task<UserServiceResponse> GetUserById(string login_Key, int userId)
        {
            UserServiceResponse objApiResponse = new UserServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (!tk.ValidateToken(login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
                if (userId > 0)
                {
                    objApiResponse.users = await _context.Users.FindAsync(userId);
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.NothingFound;
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

        [HttpPost("SaveUpdate")]
        public async Task<UserServiceResponse> PostUserss(UsersRequest objRequest)
        {
            bool EmailNotification = false;
            UserServiceResponse objApiResponse = new UserServiceResponse();
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

                if (objRequest.Users.User_Id == 0 && _context.Users.Count(x => x.Email == objRequest.Users.Email) > 0)
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.ErrorMessage;
                    return objApiResponse;
                }
                var objUser = objRequest.Users;
                if (objRequest.Users.User_Id == 0)
                {
                    EmailNotification = true;
                    objUser.Email = objRequest.Users.Email;
                    objUser.Full_Name = objRequest.Users.Full_Name;
                    objUser.Password = objRequest.Users.Password;
                    objUser.Phone = objRequest.Users.Phone;
                    objUser.User_Type = objRequest.Users.User_Type;
                    objUser.Title = objRequest.Users.Title;
                    objUser.Bio = objRequest.Users.Bio;
                    objUser.Available = objRequest.Users.Available;
                    objUser.URL = objRequest.Users.URL;
                    objUser.IsActive = false;

                    _context.Users.Add(objUser);
                    _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
                else
                {
                    objUser = _context.Users.Find(objRequest.Users.User_Id);
                    if (objUser != null)
                    {
                        objUser.Email = objRequest.Users.Email;
                        objUser.Full_Name = objRequest.Users.Full_Name;
                        objUser.Password = objRequest.Users.Password;
                        objUser.Phone = objRequest.Users.Phone;
                        objUser.User_Type = objRequest.Users.User_Type;
                        objUser.Title = objRequest.Users.Title;
                        objUser.Bio = objRequest.Users.Bio;
                        objUser.Available = objRequest.Users.Available;
                        objUser.URL = objRequest.Users.URL;
                        objUser.IsActive = objRequest.Users.IsActive;
                    }

                    _context.Users.Add(objUser);
                    _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                await _context.SaveChangesAsync();

                #region Upload Images
                try
                {
                    ImageBase64 image = objRequest.photo;
                    if (image != null && image.base64image != null && image.fileName != null && image.fileExtention != null)
                    {
                        try
                        {
                            string fileName = ImageHelper.UploadImage(image, @GlobalConfig.UserProfilePhotoUpload, objUser.User_Id, _context);
                            objUser.Profile_Photo = fileName;
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
                #endregion

                await _context.SaveChangesAsync();
                if (EmailNotification)
                {
                    //NotificationHelper.SendNotification_NewUserCreated(objUser, _context);
                }
                //var Userss = await _context.Userss.FindAsync(objRequest.Userss.Userss_Id);
                objApiResponse.users = objUser;
                objApiResponse.Message = Messages.SuccessMessage;
                objApiResponse.Status = Messages.APIStatusSuccess;
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

        [HttpPost("UpdateUserType")]
        public async Task<UserServiceResponse> UpdateUserType(string login_Key, string userType, string Email = "")
        {
            UserServiceResponse objApiResponse = new UserServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (!tk.ValidateToken(login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
                else
                {
                    var objUser = _context.Users.Find(objToken.UserId);
                    if (objUser != null)
                    {
                        objUser.User_Type = userType;
                        if (!string.IsNullOrEmpty(Email))
                        {
                            objUser.Email = Email;
                        }
                    }
                    _context.Users.Add(objUser);
                    _context.Entry(objUser).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    string Key = tk.GenrateToken(objUser.User_Id, objUser.User_Type);

                    objApiResponse.Key = Key;

                    objApiResponse.users = objUser;
                    objApiResponse.Message = Messages.SuccessMessage;
                    objApiResponse.Status = Messages.APIStatusSuccess;
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

        [HttpPost("DeleteUsers")]
        public async Task<BaseAPIResponse> DeleteUserss(ByIDBaseAPIRequest objRequest)
        {
            BaseAPIResponse objApiResponse = new BaseAPIResponse();
            try
            {
                var Userss = await _context.Users.FindAsync(objRequest.Id);
                if (Userss == null)
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.NothingFound;
                    return objApiResponse;
                }
                if (Userss.User_Id == 1)
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = "Not authorized.";
                    return objApiResponse;
                }
                var Session = _context.UserSession.Where(x => !x.Expired && x.UserID == objRequest.Id).ToList();
                _context.UserSession.RemoveRange(Session);
                _context.Users.Remove(Userss);

                await _context.SaveChangesAsync();
                objApiResponse.Status = Messages.APIStatusSuccess;
                objApiResponse.Message = Messages.SuccessMessage;
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

        [HttpPost("EnableDisableUser")]
        public async Task<BaseAPIResponse> EnableDisableUser(ByIDBaseAPIRequest objRequest)
        {
            BaseAPIResponse objApiResponse = new BaseAPIResponse();
            try
            {
                var Users = await _context.Users.FindAsync(objRequest.Id);
                if (Users == null)
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.NothingFound;
                    return objApiResponse;
                }
                if (Users.User_Id == 1)
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.ErrorMessage;
                    return objApiResponse;
                }
                var Session = _context.UserSession.Where(x => !x.Expired && x.UserID == objRequest.Id).ToList();
                _context.UserSession.RemoveRange(Session);
                Users.IsActive = !Users.IsActive;
                _context.Users.Add(Users);
                _context.Entry(Users).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                objApiResponse.Status = Messages.APIStatusSuccess;
                objApiResponse.Message = Messages.SuccessMessage;
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

        [HttpPost("UpdateDeviceId")]
        public async Task<UserServiceResponse> UpdateDeviceId(string login_Key, string UUID)
        {
            UserServiceResponse objApiResponse = new UserServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (!tk.ValidateToken(login_Key))
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.InvalidToken;
                    return objApiResponse;
                }
                else
                {
                    var objUser = _context.Users.Find(objToken.UserId);
                    if (objUser != null)
                    {
                        objUser.Device_Id = UUID;
                    }
                    _context.Users.Add(objUser);
                    _context.Entry(objUser).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    string Key = tk.GenrateToken(objUser.User_Id, objUser.User_Type);

                    objApiResponse.Key = Key;

                    objApiResponse.users = objUser;
                    objApiResponse.Message = Messages.SuccessMessage;
                    objApiResponse.Status = Messages.APIStatusSuccess;
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

        // GET: api/Users
        [HttpGet("FilterUsers")]
        public async Task<UsersListServiceResponse> FilterUsers(string login_Key, string keyword, int page = 1)
        {
            int pageSize = 20;
            int ToSkip = (page - 1) * pageSize;
            int ToTake = pageSize;

            UsersListServiceResponse objApiResponse = new UsersListServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    List<Users> lstUsers = new List<Users>();
                    if (objToken.User_Type == "Employer")
                    {
                        lstUsers = await _context.Users.Where(x => x.User_Type == "Applicant" && ((x.Full_Name != null && x.Full_Name.Contains(keyword)) || (x.Title != null && x.Title.Contains(keyword)) || (x.Bio != null && x.Bio.Contains(keyword)) || (x.Email != null && x.Email.Contains(keyword)) || (x.Phone != null && x.Phone.Contains(keyword))) && x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    else if (objToken.User_Type == "Applicant")
                    {
                        lstUsers = await _context.Users.Where(x => x.User_Type == "Employer" && ((x.Full_Name != null && x.Full_Name.Contains(keyword)) || (x.Title != null && x.Title.Contains(keyword)) || (x.Bio != null && x.Bio.Contains(keyword)) || (x.Email != null && x.Email.Contains(keyword)) || (x.Phone != null && x.Phone.Contains(keyword))) && x.IsActive).OrderBy(x => x.Full_Name).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    else
                    {
                        lstUsers = await _context.Users.OrderBy(x => ((x.Full_Name != null && x.Full_Name.Contains(keyword)) || (x.Title != null && x.Title.Contains(keyword)) || (x.Bio != null && x.Bio.Contains(keyword)) || (x.Email != null && x.Email.Contains(keyword)) || (x.Phone != null && x.Phone.Contains(keyword))) && x.IsActive).Skip(ToSkip).Take(ToTake).ToListAsync();
                    }
                    objApiResponse.Users = lstUsers;
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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

        // GET: api/Users
        [HttpGet("getCounts")]
        public async Task<CountsServiceResponse> getCounts(string login_Key)
        {

            CountsServiceResponse objApiResponse = new CountsServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                Token objToken = tk.GetTokenByKey(login_Key);
                if (objToken != null)
                {
                    objApiResponse.ApplicantsCount = await _context.Users.Where(x => x.User_Type == "Applicant").CountAsync();
                    objApiResponse.EmployersCount = await _context.Users.Where(x => x.User_Type == "Employer").CountAsync();
                    objApiResponse.PostsCount = await _context.Posts.OrderBy(x => x.IsDeleted == false).CountAsync();
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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

        // GET: api/Users
        [HttpGet("getStatsForAdmin")]
        public async Task<CountsServiceResponse> getStatsForAdmin(string login_Key)
        {

            CountsServiceResponse objApiResponse = new CountsServiceResponse();
            try
            {
                TokenHelper tk = new TokenHelper(_context);
                if (tk.ValidateToken(login_Key))
                {
                    objApiResponse.ApplicantsCount = await _context.Users.Where(x => x.User_Type == "Applicant").CountAsync();
                    objApiResponse.EmployersCount = await _context.Users.Where(x => x.User_Type == "Employer").CountAsync();
                    objApiResponse.PostsCount = await _context.Posts.OrderBy(x => !x.IsDeleted).CountAsync();
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = Messages.SuccessMessage;
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
        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.User_Id == id);
        }
    }
}
