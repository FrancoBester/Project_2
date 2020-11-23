using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dimension_Data_Demo.Data;
using Dimension_Data_Demo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Dimension_Data_Demo.Controllers
{
    [Authorize]
    public class EmployeeHistoriesController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public EmployeeHistoriesController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: EmployeeHistories
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                try
                {
                    //Used to fileter data to only show a single persons history data
                    int HistoryID = ((int)_context.Employee.Where(e => e.EmployeeNumber == id).Select(e => e.HistoryId).First());

                    HttpContext.Session.SetInt32("his_employeeNumber", (int)id);//saves data in session to be used later
                    HttpContext.Session.SetInt32("HistoryId", HistoryID);//saves data in session to be used later
                }
                catch (Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("HistoryId");
            var dimention_data_demoContext = _context.EmployeeHistory.Where(e => e.HistoryId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: EmployeeHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeHistory = await _context.EmployeeHistory
                .FirstOrDefaultAsync(m => m.HistoryId == id);
            if (employeeHistory == null)
            {
                return NotFound();
            }

            return View(employeeHistory);
        }

        // GET: EmployeeHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistoryId,NumCompaniesWorked,TotalWorkingYears,YearsAtCompany,YearsInCurrentRole,YearsSinceLastPromotion,YearsWithCurrManager,TrainingTimesLastYear")] EmployeeHistory employeeHistory)
        {
            if (ModelState.IsValid)
            {
                if (employeeHistory.NumCompaniesWorked <= -1 || employeeHistory.TotalWorkingYears <= -1 || employeeHistory.YearsAtCompany <= -1 || employeeHistory.YearsInCurrentRole <= -1 || employeeHistory.YearsSinceLastPromotion <= -1 || employeeHistory.YearsWithCurrManager <= -1 || employeeHistory.TrainingTimesLastYear <= -1)
                {
                    ViewBag.Message = "All numbers must be positive values";
                    return View();
                }

                try
                {
                    try
                    {
                        int history_ID = (int)_context.EmployeeHistory.Where(e => e.NumCompaniesWorked == employeeHistory.NumCompaniesWorked && e.TotalWorkingYears == employeeHistory.TotalWorkingYears &&
                        e.YearsAtCompany == employeeHistory.YearsAtCompany && e.YearsInCurrentRole == employeeHistory.YearsInCurrentRole && e.YearsSinceLastPromotion == employeeHistory.YearsSinceLastPromotion &&
                        e.YearsWithCurrManager == employeeHistory.YearsWithCurrManager && e.TrainingTimesLastYear == employeeHistory.TrainingTimesLastYear).Select(e => e.HistoryId).FirstOrDefault();

                        if (history_ID == 0)
                        {
                            int new_history_ID = ((int)_context.EmployeeHistory.OrderByDescending(e => e.HistoryId).Select(e => e.HistoryId).First()) + 1;//gets the new hisoty to be used when adding new record in database
                            employeeHistory.HistoryId = new_history_ID;//assignes new id to model
                            _context.Add(employeeHistory);//adds model to be added to database in context
                            await _context.SaveChangesAsync();//addes model to database

                            HttpContext.Session.SetInt32("newHistoryID", new_history_ID);//saves history in session to be used when user is added to database
                        }
                        else
                        {
                            HttpContext.Session.SetInt32("newHistoryID", history_ID);//saves history in session to be used when user is added to database
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString();//varaible used to get error when testing
                        ViewBag.Message = "There was an error updating the information. Please try again later";
                        return View();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeHistoryExists(employeeHistory.HistoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Create", "CostToCompanies");
        }

        // GET: EmployeeHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeHistory = await _context.EmployeeHistory.FindAsync(id);
            if (employeeHistory == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldHistoryModel", JsonConvert.SerializeObject(employeeHistory));
            return View(employeeHistory);
        }

        // POST: EmployeeHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HistoryId,NumCompaniesWorked,TotalWorkingYears,YearsAtCompany,YearsInCurrentRole,YearsSinceLastPromotion,YearsWithCurrManager,TrainingTimesLastYear")] EmployeeHistory employeeHistory)
        {
            if (id != employeeHistory.HistoryId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(employeeHistory) == HttpContext.Session.GetString("oldHistoryModel"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (employeeHistory.NumCompaniesWorked <= -1 || employeeHistory.TotalWorkingYears <= -1 || employeeHistory.YearsAtCompany <= -1 || employeeHistory.YearsInCurrentRole <= -1 || employeeHistory.YearsSinceLastPromotion <= -1 || employeeHistory.YearsWithCurrManager <= -1 || employeeHistory.TrainingTimesLastYear <= -1)
                    {
                        ViewBag.Message = "All numbers must be positive values";
                        return View();
                    }

                    try
                    {
                        try
                        {
                            int history_ID = (int)_context.EmployeeHistory.Where(e => e.NumCompaniesWorked == employeeHistory.NumCompaniesWorked && e.TotalWorkingYears == employeeHistory.TotalWorkingYears &&
                            e.YearsAtCompany == employeeHistory.YearsAtCompany && e.YearsInCurrentRole == employeeHistory.YearsInCurrentRole && e.YearsSinceLastPromotion == employeeHistory.YearsSinceLastPromotion &&
                            e.YearsWithCurrManager == employeeHistory.YearsWithCurrManager && e.TrainingTimesLastYear == employeeHistory.TrainingTimesLastYear).Select(e => e.HistoryId).First();

                            if (history_ID == 0)
                            {
                                history_ID = ((int)_context.EmployeeHistory.OrderByDescending(e => e.HistoryId).Select(e => e.HistoryId).First()) + 1;//gets the id of the new record that will be added into the database
                                employeeHistory.HistoryId = history_ID;//Assignes new id to model
                                _context.Add(employeeHistory);//adds id to model that will be added to database
                                await _context.SaveChangesAsync();//adds ne model info into the database
                            }

                            int employee_number = (int)HttpContext.Session.GetInt32("his_employeeNumber");//gets employee number from session
                            var employee_model = _context.Employee.FirstOrDefault(e => e.EmployeeNumber == employee_number);//gets the employee data according to the employee number

                            Employee temp_employee = (Employee)employee_model;//comverts employee data into a employee model
                            temp_employee.HistoryId = history_ID;//cahnges the eduaction id of the model to be the updated education id

                            _context.Update(temp_employee);//addes employee model to db context
                            await _context.SaveChangesAsync();//update database with new data from employee model
                        }
                        catch (Exception ex)
                        {
                            string error = ex.ToString();//variable used to see error when testing
                            ViewBag.Message = "There was an error updating the information. Please try again later";
                            return View();
                        }

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeHistoryExists(employeeHistory.HistoryId))
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

        private bool EmployeeHistoryExists(int id)
        {
            return _context.EmployeeHistory.Any(e => e.HistoryId == id);
        }
    }
}
