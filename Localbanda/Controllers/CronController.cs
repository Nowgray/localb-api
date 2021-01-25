using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Localbanda.Helpers;
using Localbanda.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Localbanda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class CronController : ControllerBase
    {
        private readonly LocalbandaDbContext _context;

        public CronController(LocalbandaDbContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet("DailyJobNotifications")]
        public async Task DailyJobNotifications(string login_Key)
        {
            int posts = _context.Posts.Count(x => x.DTS <= DateTime.Now.AddDays(1) && x.DTS >= DateTime.Now.AddDays(-1));
            if(posts > 0)
            {
                var users = _context.Users.Where(x => x.User_Type == "Applicant").ToList();
                PushHelper.NotifyToUsersDaily(users,_context, "Job Alert: New jobs posted today! Check them out. ");
            }
        }

        
    }
}
