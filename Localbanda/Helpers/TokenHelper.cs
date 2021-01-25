using Localbanda.Models;
using System;
using System.Linq;

namespace Localbanda.Helpers
{
    public class TokenHelper
    {
        private readonly LocalbandaDbContext _context;

        public TokenHelper(LocalbandaDbContext context)
        {
            _context = context;
        }

        //public string GenrateToken(Users objUser, string AppDomainName = "", string UserHostIPAddress="")
        //{
        //    try
        //    {
        //        UserSession objSession = new UserSession();
        //        objSession.UserID = objUser.User_Id;
        //        //objSession.UserType = objUser.Role_ID;
        //        //objSession.Business_Id = objUser.Business_Id;
        //        objSession.SessionGUID = Guid.NewGuid().ToString();
        //        objSession.UserHostIPAddress = UserHostIPAddress;
        //        objSession.RequestBrowsertypeVersion = string.Empty;
        //        objSession.BrowserUniqueID = "";
        //        objSession.AppDomainName = AppDomainName;
        //        objSession.IssuedOn = DateTime.Now;
        //        objSession.ExpiredOn = DateTime.Now.AddMinutes(20);
        //        objSession.Key = "";
        //        //objSession.Business_Id = objUser.Business_Id;

        //        _context.UserSession.Add(objSession);
        //        _context.Entry(objSession).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        //        _context.SaveChanges();

        //        string tokenKey = objSession.TokenID + "-" + objSession.UserID + "-" + objSession.UserHostIPAddress + "-" + objSession.Business_Id + "-" + objSession.UserType;
        //        objSession.Key = Encryption.Base64Encrypt(tokenKey);

        //        _context.UserSession.Add(objSession);
        //        _context.Entry(objSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //        _context.SaveChanges();


        //        return objSession.Key;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

        public string GenrateToken(int UserID, string UserType)
        {
            try
            {
                UserSession objSession = new UserSession();
                objSession.UserID = UserID;
                objSession.SessionGUID = Guid.NewGuid().ToString();
                objSession.UserHostIPAddress = "127.0.0.1";
                objSession.RequestBrowsertypeVersion = string.Empty;
                objSession.BrowserUniqueID = "";
                objSession.AppDomainName = "";
                objSession.IssuedOn = DateTime.Now;
                objSession.ExpiredOn = DateTime.Now.AddMinutes(20);
                objSession.Key = "";
                objSession.UserType = UserType;

                _context.UserSession.Add(objSession);
                _context.Entry(objSession).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _context.SaveChanges();

                string tokenKey = objSession.TokenID + "-" + objSession.UserID + "-" + objSession.UserHostIPAddress + "-" + objSession.UserType;
                objSession.Key = Encryption.Base64Encrypt(tokenKey);

                _context.UserSession.Add(objSession);
                _context.Entry(objSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();


                return objSession.Key;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public static void DestroyToken(long TokenId)
        {
            try
            {
                LocalbandaDbContext _context = new LocalbandaDbContext();

                UserSession objUserSession = _context.UserSession.FirstOrDefault(x => x.TokenID == TokenId);

                if (objUserSession != null)
                {
                    objUserSession.Expired = true;
                    objUserSession.ExpiredOn = DateTime.Now;
                    _context.UserSession.Add(objUserSession);
                    _context.Entry(objUserSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                }
            }
            catch (Exception ex)
            {

            }

        }

        public bool ValidateToken(string Key)
        {
            // True - Valid Token
            // False - Not valid

            if (Key == "debug")
            {
                return true;
            }
            try
            {
                string decryptedStr = Encryption.Base64Decrypt(Key);
                string[] valueArray = decryptedStr.Split('-');

                if (valueArray != null && valueArray.Count() > 0)
                {
                    Int64 KeyTokenID = Convert.ToInt64(valueArray[0].ToString());
                    int KeyUserID = Convert.ToInt32(valueArray[1].ToString());
                    string KeyUserHostIPAddress = valueArray[2];
                    string KeyRequestBrowsertypeVersion = valueArray[3];

                    //HttpBrowserCapabilities obj = HttpContext.Current.Request.Browser;
                    string BrowserUserHostIPAddress = "127.0.0.1";
                    string BrowserRequestBrowsertypeVersion = string.Empty;

                    //if (KeyUserHostIPAddress.Trim() == BrowserUserHostIPAddress.Trim() && KeyRequestBrowsertypeVersion.Trim() == BrowserRequestBrowsertypeVersion)
                    //{
                    UserSession objUserSession = new UserSession();

                    objUserSession = _context.UserSession.FirstOrDefault(x => x.TokenID == KeyTokenID);

                    if (objUserSession != null && objUserSession.Expired == false)
                    {
                        //if (DateTime.Now > objUserSession.ExpiredOn)
                        //{
                        //    objUserSession.Expired = true;

                        //    _context.UserSession.Add(objUserSession);
                        //    _context.Entry(objUserSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        //    _context.SaveChanges();
                        //    return false;

                        //}
                        //else
                        //{
                        objUserSession.ExpiredOn = DateTime.Now.AddMinutes(30);
                        _context.UserSession.Add(objUserSession);
                        _context.Entry(objUserSession).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.SaveChanges();
                        return true;
                        //}

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Token GetTokenByKey(string Key)
        {
            // CommunityDbContext _context = new CommunityDbContext();


            try
            {
                string decryptedStr = Encryption.Base64Decrypt(Key);
                string[] valueArray = decryptedStr.Split('-');

                if (valueArray != null && valueArray.Count() > 0)
                {
                    Int64 KeyTokenID = 0; int KeyUserID = 0;
                    int Business_Id = 0;
                    string KeyUserHostIPAddress = ""; string UserType = "0";
                    if (valueArray[0] != null)
                    {
                        KeyTokenID = Convert.ToInt64(valueArray[0].ToString());
                    }
                    if (valueArray[1] != null)
                    {
                        KeyUserID = Convert.ToInt32(valueArray[1].ToString());
                    }
                    try
                    {
                        if (valueArray[2] != null)
                        {
                            KeyUserHostIPAddress = valueArray[2];
                        }
                        //if (valueArray[3] != null)
                        //{
                        //    Business_Id = Convert.ToInt32(valueArray[3]);
                        //}
                        if (valueArray[3] != null)
                        {
                            UserType = valueArray[3];
                        }
                    }
                    catch (Exception ex)
                    { }
                    Token tc = new Token();
                    tc.UserHostIPAdress = KeyUserHostIPAddress;
                    tc.UserId = KeyUserID;
                    tc.TokenId = KeyTokenID;
                    tc.User_Type = UserType;
                    //tc.Business_Id = Convert.ToInt32(Business_Id);
                    //tc.UserSession = _context.UserSession.Find(tc.TokenId);
                    return tc;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }
    }
}
