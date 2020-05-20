using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult EmailConfirmed(string userId,string code)
        {
            return View();
        }
    }
}