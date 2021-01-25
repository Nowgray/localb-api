using Localbanda.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Localbanda.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_Id { get; set; }
        public string Full_Name { get; set; }
        public string User_Type { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Bio { get; set; }
        public string Available { get; set; }
        public string URL { get; set; }
        public string Profile_Photo { get; set; }
        [DefaultValue("")]
        public string Phone { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public DateTime? DTS { get; set; }
        [DefaultValue(false)]
        public bool Is_Temp_Password { get; set; }
        public string Device_Id { get; set; }
        public string Raw_Url
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile_Photo))
                {
                    return GlobalConfig.UserProfileUrl + Profile_Photo;
                }
                else
                {
                    return GlobalConfig.UserProfileUrl + "dummy.png";
                }
            }
        }
        public string Thumb_Url
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile_Photo))
                {
                    return GlobalConfig.UserProfileUrl + Profile_Photo;
                }
                else
                {
                    return GlobalConfig.UserProfileUrl + "dummy.png";
                }
            }
        }
        public string Thumb_Url_128x128
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile_Photo))
                {
                    return GlobalConfig.UserProfileUrl + Profile_Photo;
                }
                else
                {
                    return GlobalConfig.UserProfileUrl + "dummy.png";
                }
            }
        }
        public string Thumb_Url_64x64
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile_Photo))
                {
                    return GlobalConfig.UserProfileUrl + Profile_Photo;
                }
                else
                {
                    return GlobalConfig.UserProfileUrl + "dummy.png";
                }
            }
        }
        public string Thumb_Url_32x32
        {
            get
            {
                if (!string.IsNullOrEmpty(Profile_Photo))
                {
                    return GlobalConfig.UserProfileUrl + Profile_Photo;
                }
                else
                {
                    return GlobalConfig.UserProfileUrl + "dummy.png";
                }
            }
        }
    }


    public class UsersListServiceResponse : BaseAPIResponse
    {
        public List<Users> Users { get; set; }
    }

    public class UsersRequest : BaseAPIRequest
    {
        public Users Users { get; set; }
        public ImageBase64 photo { get; set; }
    }
    public class UserServiceResponse : BaseAPIResponse
    {
        public Users users { get; set; }
        public string Key { get; set; }
    }

    public class ByIDBaseAPIRequest : BaseAPIRequest
    {
        public int Id { get; set; }
    }
    public class LoginInfoResponse : BaseAPIResponse
    {
        public string Key { get; set; }
        public int User_Id { get; set; }
        public dynamic UserInfo { get; set; }
        public bool EmailPreference { get; set; }
    }

    public class SignupWithPasswordResponse : BaseAPIResponse
    {
        public string Key { get; set; }
        public dynamic UserInfo { get; set; }
    }

    public class ImageBase64
    {
        public string base64image { get; set; }
        public string fileExtention { get; set; }
        public string fileName { get; set; }
    }


    public class UserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 TokenID { get; set; }
        public int UserID { get; set; }
        public string SessionGUID { get; set; }
        public string UserHostIPAddress { get; set; }
        public string RequestBrowsertypeVersion { get; set; }
        public string BrowserUniqueID { get; set; }
        public string AppDomainName { get; set; }
        public string Key { get; set; }
        public DateTime? IssuedOn { get; set; }
        public bool Expired { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public string UserType { get; set; }
        public int Business_Id { get; set; }
    }

    public class Token
    {
        public int UserId { get; set; }
        public string User_Type { get; set; }
        public Int64 TokenId { get; set; }
        public string UserHostIPAdress { get; set; }
    }

    public class SignupRequest
    {
        public Users users { get; set; }
    }

    public class OtpLoginRequest
    {
        public string MobileNo { get; set; }
    }

    public class LoginByPasswordRequest
    {
        public string MobileNo { get; set; }
        public string Password { get; set; }
    }

    public class OtpSubmitRequest
    {
        public int User_Id { get; set; }
        public string MobileNo { get; set; }
        public string OTP { get; set; }
    }

    public class SignupResponse : BaseAPIResponse
    {
        public Users users { get; set; }
    }

    public class UsersPostSync
    {
        [Key, Column(Order = 0)]
        public int Post_Id { get; set; }
        [Key, Column(Order = 1)]
        public int User_Id { get; set; }
        public bool Is_Intrested { get; set; }
        public bool Is_Synced { get; set; }
    }

    public class CountsServiceResponse : BaseAPIResponse
    {
        public int EmployersCount { get; set; }
        public int ApplicantsCount { get; set; }
        public int PostsCount { get; set; }
    }

}
