namespace Localbanda.Helpers
{
    public class enumeration
    {
        public enum eStatusType
        {
            Open = 0,
            Pending = 1,
            Completed = 2,
        }
        public enum eApprovalType
        {
            InReview = 0,
            Approved = 1,
            Rejected = 2,
        }
        public enum eUserType
        {
            Admin = 1,
            Verifier,
            Employer,
            Applicant
        }
        public enum eLevel
        {
            Low,
            Medium,
            High
        }
        public enum eNotificationType
        {

            CorporateRegistration = 1,
            BusinessRegistration = 2,
            JobSiteCreation = 3,
            UserRegistration = 7,
            ResetPassword = 9,
            ForgetPassword = 10,
        }

        public enum eBusinessType
        {
            Corporate,
            Business
        }
        public static class eCheckInStatus
        {
            public const string
                In = "In",
                Out = "Out";
        }

        public static class eTransferStatus
        {
            public const string
                Pending = "Pending",
                Approved = "Approved",
                Rejected = "Rejected";
        }
        public static class eSMSMessageType
        {
            public const string
             Text = "1", Flash = "2", Unicode = "3";
        }
    }
}
