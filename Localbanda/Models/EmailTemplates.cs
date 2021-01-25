using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Localbanda.Models
{
    public class EmailTemplates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(55), Required]
        public string ID { get; set; }
        [StringLength(250)]
        public string Purpose { get; set; }
        [StringLength(225)]
        public string FromAddress { get; set; }
        [StringLength(225)]
        public string Subject { get; set; }
        public string Body { get; set; }
        [StringLength(250)]
        public string Cc { get; set; }
        [StringLength(250)]
        public string Bcc { get; set; }
        public short Importance { get; set; }
        public short BodyFormat { get; set; }
        public short MailFormat { get; set; }
    }
    public class EmailAPIResponses : BaseAPIResponse
    {
        public EmailTemplates Email_Templates { get; set; }
    }
    public class EmailListAPIResponses : BaseAPIResponse
    {
        public List<EmailTemplates> Email_Templates { get; set; }
    }

    public class Email_Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Business_Id { get; set; }
        [StringLength(100)]
        public string To_Email { get; set; }
        [StringLength(500)]
        public string Subject { get; set; }
        public int User_Id { get; set; }
        public string Email_Content { get; set; }
        [StringLength(100)]
        public string From_Email { get; set; }
        public bool Is_Sent { get; set; }
        [StringLength(500)]
        public string Failed_Error { get; set; }
        public DateTime? Sent_On { get; set; }
        public DateTime DTS { get; set; }

    }
}
