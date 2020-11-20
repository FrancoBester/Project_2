using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dimension_Data_Demo.Data;
using Dimension_Data_Demo.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace Dimension_Data_Demo.Controllers
{
    public class CostToCompaniesController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public CostToCompaniesController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: CostToCompanies
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                HttpContext.Session.SetInt32("PayID", (int)id);
            }
            var backupID = HttpContext.Session.GetInt32("PayID");

            var dimention_data_demoContext = _context.CostToCompany.Where(e => e.PayId == backupID);


            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: CostToCompanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CostToCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayId,HourlyRate,MonthlyRate,MonthlyIncome,DailyRate,OverTime,PercentSalaryHike")] CostToCompany costToCompany)
        {
            if (ModelState.IsValid)
            {
                string sOverTime = costToCompany.OverTime;
                sOverTime = sOverTime.ToUpper();//ensures that all data is in the same format for the column
                //Ensure user enter positive numbers into database
                if (costToCompany.HourlyRate < 0 || costToCompany.MonthlyRate < 0 || costToCompany.MonthlyIncome < 0 || costToCompany.DailyRate < 0 || costToCompany.DailyRate < 0 || costToCompany.PercentSalaryHike < 0)
                {
                    ViewBag.Message = "All numbers must be possitive values";
                    return View();
                }
                else if (sOverTime != "NO" & sOverTime != "YES")//Ensures user does not enter other value than yes and no into database
                {
                    ViewBag.Message = "OverTime value must be Yes or No";
                    return View();
                }
                costToCompany.OverTime = sOverTime;

                try
                {
                    int pay_ID = ((int)_context.CostToCompany.OrderByDescending(e => e.PayId).Select(e => e.PayId).First()) + 1;//gets the id of the last record in the table and increase by one as a new id
                    costToCompany.PayId = pay_ID;//assignes new id to model
                    _context.Add(costToCompany);//add model to be added into the db context
                    await _context.SaveChangesAsync();//addes the new row into the database using the db context

                    HttpContext.Session.SetInt32("newPayID", pay_ID);//add id to a session to be used later when creating the new user in the employee table
                }
                catch (Exception ex)//catches any error that may happen in try brackets
                {
                    string error = ex.ToString();//variable used to see error when testing
                    ViewBag.Message = "There was an error updating the information. Please try again later";
                    return View();
                }
            }
            return RedirectToAction("Create", "EmployeePerformances");
        }

        // GET: CostToCompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costToCompany = await _context.CostToCompany.FirstOrDefaultAsync(m => m.PayId == id);
            if (costToCompany == null)
            {
                return NotFound();
            }

            return View(costToCompany);
        }

        // GET: CostToCompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costToCompany = await _context.CostToCompany.FindAsync(id);
            if (costToCompany == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldCostModel", JsonConvert.SerializeObject(costToCompany));//Saves model as string in session to be used to compare later
            return View(costToCompany);
        }

        // POST: CostToCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayId,HourlyRate,MonthlyRate,MonthlyIncome,DailyRate,OverTime,PercentSalaryHike")] CostToCompany costToCompany)
        {
            if (id != costToCompany.PayId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(costToCompany) == HttpContext.Session.GetString("oldCostModel"))//compares old and current model to limit update query to database when models are same
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    string sOverTime = costToCompany.OverTime;
                    sOverTime = sOverTime.ToUpper();
                    //Ensure user enter positive numbers into database
                    if (costToCompany.HourlyRate < 0 || costToCompany.MonthlyRate < 0 || costToCompany.MonthlyIncome < 0 || costToCompany.DailyRate < 0 || costToCompany.DailyRate < 0 || costToCompany.PercentSalaryHike < 0)
                    {
                        ViewBag.Message = "All numbers must be possitive values";
                        return View();
                    }
                    else if (sOverTime != "NO" & sOverTime != "YES")//Ensures user does not enter other value than yes and no into database
                    {
                        ViewBag.Message = "OverTime value must be Yes or No";
                        return View();
                    }
                    costToCompany.OverTime = sOverTime;
                    try
                    {
                        _context.Update(costToCompany);//updates database
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CostToCompanyExists(costToCompany.PayId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction("Index", "Employees");
            }
        }

        private bool CostToCompanyExists(int id)
        {
            return _context.CostToCompany.Any(e => e.PayId == id);
        }

    }
}
