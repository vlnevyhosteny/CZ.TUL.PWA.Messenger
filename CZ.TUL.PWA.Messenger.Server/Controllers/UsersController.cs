using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace CZ.TUL.PWA.Messenger.Server.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public UsersController()
        {
        }

        [HttpGet, Authorize]
        public IEnumerable<string> Get() 
        {
            return new string[] { "lol", "lol1" };
        }
    }
}
