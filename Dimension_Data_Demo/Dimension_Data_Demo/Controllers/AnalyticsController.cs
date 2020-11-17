using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimension_Data_Demo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Dimension_Data_Demo.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public AnalyticsController(dimention_data_demoContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
