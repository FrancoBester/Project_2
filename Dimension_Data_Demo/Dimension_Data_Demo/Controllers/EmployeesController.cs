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
                try
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
                    while (reader.Read())
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
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
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
            try
            {
                int value = 0;
                var conn = _context.Database.GetDbConnection();
                conn.OpenAsync();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select top 1 * from dbo.Employee Order By EmployeeNumber DESC");
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    value = reader.GetInt32(0);
                }
                cmd.Dispose();
                reader.Close();

                cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Del", (int)HttpContext.Session.GetInt32("newDetailsID"));
                cmd.Parameters.AddWithValue("@Edu", (int)HttpContext.Session.GetInt32("newEducationID"));
                cmd.Parameters.AddWithValue("@His", (int)HttpContext.Session.GetInt32("newHistoryID"));
                cmd.Parameters.AddWithValue("@Pay", (int)HttpContext.Session.GetInt32("newPayID"));
                cmd.Parameters.AddWithValue("@Per", (int)HttpContext.Session.GetInt32("newPerformanceID"));
                cmd.Parameters.AddWithValue("@Job", (int)HttpContext.Session.GetInt32("newJobID"));
                cmd.Parameters.AddWithValue("@Sur", (int)HttpContext.Session.GetInt32("newSurveyID"));
                cmd.Parameters.AddWithValue("@Emp", value +1);
                cmd.CommandText = ("Insert into dbo.Employee(EmployeeNumber,JobID,DetailsID,PayID,EducationID,SurveyID,HistoryID,PerformanceID) " +
                    "Values(@Emp,@Job,@Del,@Pay,@Edu,@Sur,@His,@Per)");
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                conn.Close();
            }
            catch(Exception)
            {
                ViewBag.Message = "Error adding employee. Please try again later.";
                return View();
            }
            ViewBag.Message = "Employee succesfully added.";
            return View();
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

        public ActionResult Data()
        {
            var d_age = _context.EmployeeDetails.ToList();
            List<int> repartition = new List<int>();
            var test = _context.Database.ExecuteSqlRaw("Select * from dbo.EmployeeDetails");

            var ages = d_age.Select(e => e.Age).Distinct();

            foreach (var item in ages)
            {
               repartition.Add(d_age.Count(e => e.Age == item));
            }

            var rep = repartition;
            ViewBag.AGES = ages;
            ViewBag.REP = repartition.ToList();
            return View();

        }
    }
  
}
