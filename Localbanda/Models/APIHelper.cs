using System.ComponentModel.DataAnnotations;

namespace Localbanda.Models
{
    public class BaseAPIResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string ValidationMessage { get; set; }
        public string ExecutionTime { get; set; }
    }

    public class BaseAPIRequest
    {
        [Required]
        public string Login_Key { get; set; }
    }
}
