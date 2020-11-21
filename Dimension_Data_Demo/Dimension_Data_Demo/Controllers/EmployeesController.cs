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
using ReflectionIT.Mvc.Paging;

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
        public async Task<IActionResult> Index(int page=1)
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

            //var qry = _context.Employee.AsNoTracking().OrderBy(p => p.EmployeeNumber);
            //var model = await PagingList.CreateAsync(qry, 10, page);
            //return View(model);

            //Gets user info from user that was stored in sessions from other pages

            var user_EmpNumber = HttpContext.Session.GetString("EmployeeNumber");
            var user_Role = HttpContext.Session.GetString("JobRole");
            var user_Department = HttpContext.Session.GetString("Department");

            int iEmployeeNumbeer = int.Parse(user_EmpNumber.ToString());
            
            if (user_Role.ToString() == "Manager")//if the user is a manager they are allowed to see all other employee info that are part of the same department as they are
            {
                try
                {
                    var list_job_ID = _context.JobInformation.Where(e => e.Department == user_Department).Select(e => e.JobId);
                    List<int> listOfJobIds = new List<int>();

                    foreach (var jobID in list_job_ID)
                    {
                        listOfJobIds.Add((int)jobID);
                    }


                    var dimention_data_demoContext_2 = _context.Employee.Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey).Where(b => listOfJobIds.Contains((int)b.JobId)).AsNoTracking().OrderBy(e => e.EmployeeNumber);
                    var model = await PagingList.CreateAsync(dimention_data_demoContext_2, 10, page);
                    return View(model);
                }
                catch(Exception ex)
                {
                    string error = ex.ToString();
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
                int employee_ID = ((int)_context.Employee.OrderByDescending(e => e.EmployeeNumber).Select(e => e.EmployeeNumber).First()) + 1; //gets the employee number of the new employee

                Employee new_employee = new Employee();
                new_employee.EmployeeNumber = employee_ID;
                new_employee.DetailsId = (int)HttpContext.Session.GetInt32("newDetailsID");
                new_employee.EducationId = (int)HttpContext.Session.GetInt32("newEducationID");
                new_employee.HistoryId = (int)HttpContext.Session.GetInt32("newHistoryID");
                new_employee.PayId = (int)HttpContext.Session.GetInt32("newPayID");
                new_employee.PerformanceId = (int)HttpContext.Session.GetInt32("newPerformanceID");
                new_employee.JobId = (int)HttpContext.Session.GetInt32("newJobID");
                new_employee.SurveyId = (int)HttpContext.Session.GetInt32("newSurveyID");

                _context.Add(new_employee);
                _context.SaveChangesAsync();

                //int value = 0;
                //var conn = _context.Database.GetDbConnection();
                //conn.OpenAsync();
                //SqlCommand cmd = new SqlCommand();
                //cmd.Connection = (SqlConnection)conn;
                //cmd.CommandType = System.Data.CommandType.Text;
                //cmd.CommandText = ("Select top 1 * from dbo.Employee Order By EmployeeNumber DESC");
                //SqlDataReader reader = cmd.ExecuteReader();
                //while(reader.Read())
                //{
                //    value = reader.GetInt32(0);
                //}
                //cmd.Dispose();
                //reader.Close();

                //cmd = new SqlCommand();
                //cmd.Connection = (SqlConnection)conn;
                //cmd.CommandType = System.Data.CommandType.Text;
                //cmd.Parameters.AddWithValue("@Del", (int)HttpContext.Session.GetInt32("newDetailsID"));
                //cmd.Parameters.AddWithValue("@Edu", (int)HttpContext.Session.GetInt32("newEducationID"));
                //cmd.Parameters.AddWithValue("@His", (int)HttpContext.Session.GetInt32("newHistoryID"));
                //cmd.Parameters.AddWithValue("@Pay", (int)HttpContext.Session.GetInt32("newPayID"));
                //cmd.Parameters.AddWithValue("@Per", (int)HttpContext.Session.GetInt32("newPerformanceID"));
                //cmd.Parameters.AddWithValue("@Job", (int)HttpContext.Session.GetInt32("newJobID"));
                //cmd.Parameters.AddWithValue("@Sur", (int)HttpContext.Session.GetInt32("newSurveyID"));
                //cmd.Parameters.AddWithValue("@Emp", value +1);
                //cmd.CommandText = ("Insert into dbo.Employee(EmployeeNumber,JobID,DetailsID,PayID,EducationID,SurveyID,HistoryID,PerformanceID) " +
                //    "Values(@Emp,@Job,@Del,@Pay,@Edu,@Sur,@His,@Per)");
                //cmd.ExecuteNonQuery();

                //cmd.Dispose();
                //conn.Close();
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
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
            //Ages
            var age_all = _context.EmployeeDetails.ToList();//gets all age database
            List<int> repeat_age = new List<int>();
            var emp_ages = age_all.Select(e => e.Age).Distinct();//gets unique ages from database

            foreach (var item in emp_ages)
            {
                repeat_age.Add(age_all.Count(e => e.Age == item));//adds amount of each age in database to list
            }

            ViewBag.AGE = emp_ages;//returns data to view
            ViewBag.AGEREP = repeat_age.ToList();//returns data to view

            //Genders
            var gender_all = _context.EmployeeDetails.ToList();//get all employee genders from database
            List<int> repeat_gender = new List<int>();
            var emp_gender = gender_all.Select(e => e.GenderId).Distinct();//gets uniquw age types from database

            foreach(var gender in emp_gender)
            {
                repeat_gender.Add(gender_all.Count(e => e.GenderId == gender));//add amnount of each gender in database to list
            }

            ViewBag.GENDER = emp_gender;//returns gender to view
            ViewBag.GENREP = repeat_gender.ToList();//returns data to view

            //Marital
            var marital_all = _context.EmployeeDetails.ToList();
            List<int> repeat_marital = new List<int>();
            var emp_marital = marital_all.Select(e => e.MaritalId).Distinct();

            foreach(var marrige in emp_marital)
            {
                repeat_marital.Add(marital_all.Count(e => e.MaritalId == marrige));
            }

            ViewBag.MARRIGE = emp_marital;
            ViewBag.MARREP = repeat_marital.ToList();


            return View();

        }
    }
  
}
