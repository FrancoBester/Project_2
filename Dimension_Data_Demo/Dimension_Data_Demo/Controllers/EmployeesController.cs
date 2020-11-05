using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dimension_Data_Demo.Data;
using Dimension_Data_Demo.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Dimension_Data_Demo.Controllers
{
    [Authorize]//stops non authorized users from accessing page
    public class EmployeesController : Controller
    {
        private readonly dimention_data_demoContext _context;
        public EmployeesController(dimention_data_demoContext context)
        {
            _context = context;
            try
            {
                var conn = _context.Database.GetDbConnection();
                //conn.Open();
            }
            catch(Exception ex)
            {
                string serror = ex.ToString();
            }
            //string connectionstring = conn?.ConnectionString;        
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            //try Row level security tried
            //{
            //    var dimention_data_demoContext = _context.Employee.FromSqlRaw("Execute as user = '1' Select * from dbo.Employee").Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey);
            //    var value = TempData["Email"];
            //    return View(await dimention_data_demoContext.ToListAsync());
            //}
            //catch (Exception ex)
            //{
            //    string stest = ex.ToString();
            //    return null;
            //}

            //Gets user info from user that was stored in sessions from other pages
            var user_EmpNumber = HttpContext.Session.GetString("EmployeeNumber");
            var user_Role = HttpContext.Session.GetString("JobRole");
            var user_Department = HttpContext.Session.GetString("Department");

            int iEmployeeNumbeer = int.Parse(user_EmpNumber.ToString());
            
            if (user_Role.ToString() == "Manager")//if the user is a manager they are allowed to see all other employee info that are part of the same department as they are
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                //SqlConnection conn = new SqlConnection((_context.Database.GetDbConnection()).ToString());
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Department", user_Department.ToString());
                cmd.CommandText = "Select JobID from dbo.JobInformation Where Department = @Department";
                List<int> listOfJobIds = new List<int>();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    string stest = reader.GetValue(0).ToString();
                    listOfJobIds.Add(int.Parse(reader.GetValue(0).ToString()));
                }

                var dimention_data_demoContext_2 = _context.Employee.Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey).Where(b => listOfJobIds.Contains((int)b.JobId));
                conn.Close();
                reader.Close();
                cmd.Dispose();
                return View(await dimention_data_demoContext_2.ToListAsync());
            }
            else
            {
                //if user is not a manager they are only allowed to see their own info
                var dimention_data_demoContext = _context.Employee.Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey).Where(b => b.EmployeeNumber == iEmployeeNumbeer);
                return View(await dimention_data_demoContext.ToListAsync());
            }
            //var dimention_data_demoContext = _context.Employee.Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey).Where(b => b.EmployeeNumber == valu) ;
            //CostToCompaniesController employeeCost = new CostToCompaniesController(_context);
            //await employeeCost.Details(valu);
            
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var conn = _context.Database.GetDbConnection();
            

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Details)

                .Include(e => e.Education)

                .Include(e => e.History)
                .Include(e => e.Job)
                .Include(e => e.Pay)
                .Include(e => e.Performance)
                .Include(e => e.Survey) 
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DetailsId"] = new SelectList(_context.EmployeeDetails, "DetailsId", "DetailsId");
            ViewData["EducationId"] = new SelectList(_context.EmployeeEducation, "EducationId", "EducationId");
            ViewData["HistoryId"] = new SelectList(_context.EmployeeHistory, "HistoryId", "HistoryId");
            ViewData["JobId"] = new SelectList(_context.JobInformation, "JobId", "JobId");
            ViewData["PayId"] = new SelectList(_context.CostToCompany, "PayId", "PayId");
            ViewData["PerformanceId"] = new SelectList(_context.EmployeePerformance, "PerformanceId", "PerformanceId");
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "SurveyId");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeNumber,JobId,DetailsId,PayId,EducationId,SurveyId,HistoryId,PerformanceId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DetailsId"] = new SelectList(_context.EmployeeDetails, "DetailsId", "DetailsId", employee.DetailsId);
            ViewData["EducationId"] = new SelectList(_context.EmployeeEducation, "EducationId", "EducationId", employee.EducationId);
            ViewData["HistoryId"] = new SelectList(_context.EmployeeHistory, "HistoryId", "HistoryId", employee.HistoryId);
            ViewData["JobId"] = new SelectList(_context.JobInformation, "JobId", "JobId", employee.JobId);
            ViewData["PayId"] = new SelectList(_context.CostToCompany, "PayId", "PayId", employee.PayId);
            ViewData["PerformanceId"] = new SelectList(_context.EmployeePerformance, "PerformanceId", "PerformanceId", employee.PerformanceId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "SurveyId", employee.SurveyId);
            return View(employee);
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DetailsId"] = new SelectList(_context.EmployeeDetails, "DetailsId", "DetailsId", employee.DetailsId);
            ViewData["EducationId"] = new SelectList(_context.EmployeeEducation, "EducationId", "EducationId", employee.EducationId);
            ViewData["HistoryId"] = new SelectList(_context.EmployeeHistory, "HistoryId", "HistoryId", employee.HistoryId);
            ViewData["JobId"] = new SelectList(_context.JobInformation, "JobId", "JobId", employee.JobId);
            ViewData["PayId"] = new SelectList(_context.CostToCompany, "PayId", "PayId", employee.PayId);
            ViewData["PerformanceId"] = new SelectList(_context.EmployeePerformance, "PerformanceId", "PerformanceId", employee.PerformanceId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "SurveyId", employee.SurveyId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeNumber,JobId,DetailsId,PayId,EducationId,SurveyId,HistoryId,PerformanceId")] Employee employee)
        {
            if (id != employee.EmployeeNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DetailsId"] = new SelectList(_context.EmployeeDetails, "DetailsId", "DetailsId", employee.DetailsId);
            ViewData["EducationId"] = new SelectList(_context.EmployeeEducation, "EducationId", "EducationId", employee.EducationId);
            ViewData["HistoryId"] = new SelectList(_context.EmployeeHistory, "HistoryId", "HistoryId", employee.HistoryId);
            ViewData["JobId"] = new SelectList(_context.JobInformation, "JobId", "JobId", employee.JobId);
            ViewData["PayId"] = new SelectList(_context.CostToCompany, "PayId", "PayId", employee.PayId);
            ViewData["PerformanceId"] = new SelectList(_context.EmployeePerformance, "PerformanceId", "PerformanceId", employee.PerformanceId);
            ViewData["SurveyId"] = new SelectList(_context.Surveys, "SurveyId", "SurveyId", employee.SurveyId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Details)
                .Include(e => e.Education)
                .Include(e => e.History)
                .Include(e => e.Job)
                .Include(e => e.Pay)
                .Include(e => e.Performance)
                .Include(e => e.Survey)
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeNumber == id);
        }

        //public ActionResult test()
        //{
        //    string connection = 
        //}
        
    }
  
}
