using Localbanda.Helpers;
using Localbanda.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Localbanda.Helpers.enumeration;

namespace Localbanda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly LocalbandaDbContext _context;

        public AccountController(LocalbandaDbContext context)
        {
            _context = context;
        }

        #region Login Using Password

        [HttpPost("SignupWithPassword")]
        [Produces("application/json")]
        public async Task<SignupWithPasswordResponse> SignupWithPassword(SignupRequest objRequest)
        {
            SignupWithPasswordResponse objAPIResponse = new SignupWithPasswordResponse();
            try
            {
                if (objRequest != null)
                {
                    if (_context.Users.Count(x => x.Phone == objRequest.users.Phone) > 0)
                    {
                        objAPIResponse.Message = Messages.AlreadyRegisterMobile;
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }
                    else
                    {
                        Users objUser = new Users();
                        objUser.Full_Name = objRequest.users.Full_Name;
                        objUser.Phone = objRequest.users.Phone;
                        objUser.User_Type = objRequest.users.User_Type;
                        objUser.Password = objRequest.users.Password;
                        objUser.DTS = DateTime.Now;
                        objUser.IsActive = true;

                        _context.Users.Add(objUser);
                        _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        await _context.SaveChangesAsync();
 
                        TokenHelper tk = new TokenHelper(_context);

                        //UserDevices objUserDevices = objRequest.userDevice;
                        //objUserDevices.UserId = objUser.UserId;
                        //objUserDevices.DeviceId = objRequest.userDevice.DeviceId;
                        //objUserDevices.PlayerID = objRequest.userDevice.PlayerID;
                        //objUserDevices.DTS = DateTime.Now;
                        //_context.UserDevices.Add(objUserDevices);
                        //await _context.SaveChangesAsync();

                        string Key = tk.GenrateToken(objUser.User_Id, objUser.User_Type);

                        objAPIResponse.Key = Key;
                        objAPIResponse.UserInfo = objUser;
                        objAPIResponse.Message = "Registration successfully.";
                        objAPIResponse.Status = Messages.APIStatusSuccess;
                        return objAPIResponse;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        [HttpPost("LoginByPassword")]
        [Produces("application/json")]
        public async Task<LoginInfoResponse> LoginByPassword(LoginByPasswordRequest objRequest)
        {

            LoginInfoResponse objAPIResponse = new LoginInfoResponse();
            try
            {
                if (objRequest != null)
                {

                    TokenHelper tk = new TokenHelper(_context);
                    if (string.IsNullOrEmpty(objRequest.MobileNo))
                    {
                        objAPIResponse.Message = "Mobile No is required.";
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }

                    Users objUser = await _context.Users.FirstOrDefaultAsync(x => x.Phone == objRequest.MobileNo);
                    if (objUser == null)
                    {
                        objAPIResponse.Message = Messages.NotRegistred;
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }
                    if (objUser.Password == objRequest.Password.ToString())
                    {
                        objUser.IsActive = true;

                        _context.Users.Add(objUser);
                        _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();

                        string Key = tk.GenrateToken(objUser.User_Id, objUser.User_Type);

                        objAPIResponse.Key = Key;
                        objAPIResponse.UserInfo = objUser;
                        objAPIResponse.Message = "Login successfully.";
                        objAPIResponse.Status = Messages.APIStatusSuccess;
                        return objAPIResponse;
                    }
                }
                objAPIResponse.Message = "Invalid password.";
                objAPIResponse.Status = Messages.APIStatusError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        [HttpPost("ForgotPIN")]
        [Produces("application/json")]
        public async Task<BaseAPIResponse> ForgotPIN(OtpLoginRequest objRequest)
        {
            BaseAPIResponse objAPIResponse = new BaseAPIResponse();
            try
            {
                Users objUser = await _context.Users.FirstOrDefaultAsync(x => x.Phone == objRequest.MobileNo);
                if (objUser != null)
                {
                    objUser.Password = GNF.GenerateRandomNo().ToString();
                    _context.Users.Add(objUser);
                    _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    string SMS = "New PIN: " + objUser.Password;
                    SMSHelper.SendOTP(objUser.Phone, SMS);
                }

                else
                {
                    objAPIResponse.Message = Messages.NotRegistred;
                    objAPIResponse.Status = Messages.APIStatusError;
                    return objAPIResponse;
                }
                objAPIResponse.Message = "A new PIN has been sent to your registered mobile number.";
                objAPIResponse.Status = Messages.APIStatusSuccess;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        #endregion

        #region Mobile OTP SignupLogin APIs

        [HttpPost("Signup")]
        [Produces("application/json")]
        public async Task<SignupResponse> SignupRequest(SignupRequest objRequest, bool IsResendOTP = false)
        {
            SignupResponse objAPIResponse = new SignupResponse();
            try
            {
                if (objRequest != null)
                {
                    if (_context.Users.Count(x => x.Phone == objRequest.users.Phone) > 0)
                    {
                        objAPIResponse.Message = Messages.AlreadyRegisterMobile;
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }
                    if (IsResendOTP == true)
                    {
                        var user = await _context.Users.FirstOrDefaultAsync(x => x.Phone == objRequest.users.Phone);
                        SMSHelper.SendOTP(user.Phone, "OTP: " + user.Password);
                    }
                    else
                    {

                        Users objUser = new Users();

                        objUser.Full_Name = objRequest.users.Full_Name;
                        objUser.User_Type = objRequest.users.User_Type;
                        objUser.Phone = objRequest.users.Phone;
                        objUser.Password = GNF.GenerateRandomNo().ToString();
                        objUser.DTS = DateTime.Now;

                        string SMS = "OTP: " + objUser.Password;

                        SMSHelper.SendOTP(objUser.Phone, SMS);

                        _context.Users.Add(objUser);
                        _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        _context.SaveChanges();

                        objAPIResponse.users = objUser;
                        objAPIResponse.Message = Messages.OTPSent;
                        objAPIResponse.Status = Messages.APIStatusSuccess;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        [HttpPost("SubmitOTP")]
        [Produces("application/json")]
        public async Task<LoginInfoResponse> SubmitOTP(OtpSubmitRequest objRequest)
        {

            LoginInfoResponse objAPIResponse = new LoginInfoResponse();
            try
            {
                if (objRequest != null)
                {

                    TokenHelper tk = new TokenHelper(_context);
                    if (string.IsNullOrEmpty(objRequest.MobileNo))
                    {
                        objAPIResponse.Message = "Mobile No is required.";
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }

                    Users objUser = await _context.Users.FirstOrDefaultAsync(x => x.Phone == objRequest.MobileNo);
                    if (objUser.Password == objRequest.OTP.ToString())
                    {
                        objUser.IsActive = true;

                        _context.Users.Add(objUser);
                        _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();

                        string Key = tk.GenrateToken(objUser.User_Id, objUser.User_Type);

                        objAPIResponse.Key = Key;
                        objAPIResponse.User_Id = objUser.User_Id;
                        objAPIResponse.UserInfo = objUser;

                        objAPIResponse.Message = Messages.OTPVerified;
                        objAPIResponse.Status = Messages.APIStatusSuccess;
                        return objAPIResponse;
                    }
                }
                objAPIResponse.Message = Messages.InvalidOTP;
                objAPIResponse.Status = Messages.APIStatusError;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        [HttpPost("LoginByPhone")]
        [Produces("application/json")]
        public async Task<BaseAPIResponse> LoginByPhone(OtpLoginRequest objRequest, bool IsResendOTP = false)
        {
            BaseAPIResponse objAPIResponse = new BaseAPIResponse();
            try
            {
                if (objRequest.MobileNo == "9876543210" || objRequest.MobileNo == "1234567890")
                {
                    objAPIResponse.Message = Messages.OTPSent;
                    objAPIResponse.Status = Messages.APIStatusSuccess;
                    return objAPIResponse;
                }
                else
                {
                    Users objUser = await _context.Users.FirstOrDefaultAsync(x => x.Phone == objRequest.MobileNo);
                    if (objUser != null)
                    {
                        if (IsResendOTP == true)
                        {
                            SMSHelper.SendOTP(objUser.Phone, "OTP: " + objUser.Password);
                        }
                        else
                        {
                            objUser.Password = GNF.GenerateRandomNo().ToString();
                            _context.Users.Add(objUser);
                            _context.Entry(objUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _context.SaveChanges();
                            string SMS = "OTP: " + objUser.Password;

                            SMSHelper.SendOTP(objUser.Phone, SMS);
                        }

                    }

                    else
                    {
                        objAPIResponse.Message = Messages.NotRegistred;
                        objAPIResponse.Status = Messages.APIStatusError;
                        return objAPIResponse;
                    }
                    objAPIResponse.Message = Messages.OTPSent;
                    objAPIResponse.Status = Messages.APIStatusSuccess;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAPIResponse;
        }

        #endregion

        [HttpPost("validateToken")]
        public async Task<BaseAPIResponse> ValidateToken(string LoginKey)
        {
            BaseAPIResponse objApiResponse = new BaseAPIResponse();
            TokenHelper tk = new TokenHelper(_context);
            try
            {
                if (tk.ValidateToken(LoginKey))
                {
                    objApiResponse.Status = Messages.APIStatusSuccess;
                    objApiResponse.Message = "valid Token";
                }
                else
                {
                    objApiResponse.Status = Messages.APIStatusError;
                    objApiResponse.Message = Messages.SessionExpired;
                    return objApiResponse;
                }
            }
            catch (Exception ex)
            {
                objApiResponse.Message = "Exception: " + ex.Message;
                objApiResponse.Status = Messages.APIStatusError;
                GNF.SaveException(ex, _context);
            }
            return objApiResponse;
        }

    }
}