using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GM.Service.Gateway
{
   [Route("[controller]")]   
   [ApiController]  
   public class HomeController : ControllerBase  
   {
      [HttpGet]   
      [Route("Index")]   
      public string Index()  
      {
            return "Repo Gateway API";  
      }
   }
}