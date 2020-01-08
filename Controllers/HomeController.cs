using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WDXWebApiDespachoJuridico.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return "Bienvenido a la api del Despacho Jurìdico Gómez Mora";
        }
    }
}
