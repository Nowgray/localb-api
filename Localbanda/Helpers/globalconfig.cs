using Microsoft.Extensions.Configuration;

namespace Localbanda.Helpers
{
    public class GlobalConfig
    {


        public static string Get_LogoUrl = "";

        /// <summary>
        /// *******************************PRODUCTION
        /// </summary>
        public static string ConnectionString = @"data source=sql1-p4stl.ezhostingserver.com;initial catalog=Localbanda_DB;user id=localbanda2020;password=Nowgray@2019;multipleactiveresultsets=True;application name=EntityFramework";

        /// <summary>
        /// *******************************LOCALHOST
        /// </summary>
       // public static string ConnectionString = @"Data Source=tcp:localbandadbserver.database.windows.net,1433;Initial Catalog=Localbanda_db;User Id=nowgray@localbandadbserver;Password=Nowgray@2019";

        /// <summary>
        /// ******************************DEV
        /// </summary>
        //public static string ConnectionString = @"data source=sql1-p4stl.ezhostingserver.com;initial catalog=localbanda_DEV;user id=localbanda2020;password=Nowgray@2019;multipleactiveresultsets=True;application name=EntityFramework";







        /// <summary>
        /// ******************************COMMON FIELDS
        /// </summary>

        public static string UserProfilePhotoUpload = "../../wwwroot/assets/userprofilephoto";

        public static string AdminWebUrl = "http://admin.localbanda.in/";
        public static string MainWebUrl = "http://localbanda.in/";
        public static string UserProfileUrl = "http://localbanda.in/assets/userprofilephoto/";

        public static string FromAddress = "nowgray@gmail.com";
        public static string SMTPHost = "smtp.gmail.com";
        public static int SMTPPort = 587;
        public static bool SSL = true;
        public static string SenderAddress = "nowgray@gmail.com";
        public static string SenderPassword = "lqfmmybtkmqmnpjl";//Fake Cred
        public static string supportAdmin = "askdev@nowgray.com";


    }
}
