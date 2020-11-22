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
                    ViewBag.job = user_Role.ToString();
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
                var dimention_data_demoContext = _context.Employee.Include(e => e.Details).Include(e => e.Education).Include(e => e.History).Include(e => e.Job).Include(e => e.Pay).Include(e => e.Performance).Include(e => e.Survey).Where(b => b.EmployeeNumber == iEmployeeNumbeer).AsNoTracking().OrderBy(e => e.EmployeeNumber);
                var model = await PagingList.CreateAsync(dimention_data_demoContext, 10, page);
                return View(model);
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
            int employee_ID = 0;
            try
            {
                 employee_ID = ((int)_context.Employee.OrderByDescending(e => e.EmployeeNumber).Select(e => e.EmployeeNumber).First()) + 1; //gets the employee number of the new employee

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
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                ViewBag.Message = "Error adding employee. Please try again later.";
                return View();
            }
            ViewBag.Message = "Employee succesfully added.";
            ViewBag.Employee_new = "New employee number is: " + employee_ID.ToString();
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
            var user_Role = HttpContext.Session.GetString("JobRole");
            if (user_Role.ToString() == "Manager")
            {
                var employee = _context.Employee.ToList();//used by multiple caculations

                //Ages
                var employeeDetails_all = _context.EmployeeDetails.ToList();//gets all age database
                List<int> repeat_age = new List<int>();
                var emp_ages = employeeDetails_all.OrderBy(e => e.Age).Select(e => e.Age).Distinct();//gets unique ages from database

                foreach (var item in emp_ages)
                {
                    repeat_age.Add(employeeDetails_all.Count(e => e.Age == item));//adds amount of each age in database to list
                }

                ViewBag.AGE = emp_ages;//returns data to view
                ViewBag.AGEREP = repeat_age.ToList();//returns data to view

                //Genders
                var gender_all = _context.Gender.ToList();
                List<int> repeat_gender = new List<int>();
                var emp_gender = gender_all.Select(e => e.Gender1).Distinct();
                var emp_gender_2 = employeeDetails_all.Select(e => e.GenderId).Distinct();//gets unique age types from database

                foreach (var gender in emp_gender_2)
                {
                    repeat_gender.Add(employeeDetails_all.Count(e => e.GenderId == gender));//add amnount of each gender in database to list
                }

                ViewBag.GENDER = emp_gender;//returns gender to view
                ViewBag.GENREP = repeat_gender.ToList();//returns data to view

                //Marital
                var marrige_all = _context.MaritalStatus.ToList();
                List<int> repeat_marital = new List<int>();
                var emp_marital = marrige_all.Select(e => e.MaritalStatus1).Distinct();
                var emp_marital_id = employeeDetails_all.Select(e => e.MaritalId).Distinct();


                foreach (var marrige in emp_marital_id)
                {
                    repeat_marital.Add(employeeDetails_all.Count(e => e.MaritalId == marrige));
                }

                ViewBag.MARRIGE = emp_marital;
                ViewBag.MARREP = repeat_marital.ToList();



                //Departments
                var jobInformation_all = _context.JobInformation.ToList();
                List<int> repeat_department = new List<int>();
                var emp_departmet = jobInformation_all.OrderBy(e => e.Department).Select(e => e.Department).Distinct();

                foreach (var department in emp_departmet)
                {
                    repeat_department.Add(jobInformation_all.Count(e => e.Department == department));
                }

                ViewBag.DEPARTMENT = emp_departmet;
                ViewBag.DEPREP = repeat_department.ToList();

                //Travel
                List<int> repeat_travel = new List<int>();
                var emp_travel = jobInformation_all.Select(e => e.BusinessTravel).Distinct();

                foreach (var travel in emp_travel)
                {
                    var total = 0;
                    var unique_number = jobInformation_all.Where(e => e.BusinessTravel == travel).Select(e => e.JobId);
                    foreach (var number in unique_number)
                    {
                        total = total + employee.Count(e => e.JobId == number);
                    }
                    repeat_travel.Add(total);
                }

                ViewBag.TRAVEL = jobInformation_all.Select(e => e.BusinessTravel).Distinct();
                ViewBag.TRAREP = repeat_travel.ToList();



                //Educations
                var education_all = _context.EmployeeEducation.ToList();
                List<int> repeat_education = new List<int>();
                var emp_education = education_all.OrderBy(e => e.EducationField).Select(e => e.EducationField).Distinct();

                foreach (var education in emp_education)
                {
                    var total = 0;
                    var unique_number = education_all.Where(e => e.EducationField == education).Select(e => e.EducationId);

                    foreach (var number in unique_number)
                    {
                        total = total + employee.Count(e => e.EducationId == number);
                    }
                    repeat_education.Add(total);
                }

                ViewBag.EDUCATION = emp_education;
                ViewBag.EDUREP = repeat_education.ToList();



                //Environment
                var survey_all = _context.Surveys.ToList();
                List<int> repeat_environment = new List<int>();
                var emp_environment = survey_all.OrderBy(e => e.EnvironmentSatisfaction).Select(e => e.EnvironmentSatisfaction).Distinct();

                foreach (var environment in emp_environment)
                {
                    int total = 0;
                    var unique_number = survey_all.Where(e => e.EnvironmentSatisfaction == environment).Select(e => e.SurveyId);

                    foreach (var number in unique_number)
                    {
                        total = total + employee.Count(e => e.SurveyId == number);
                    }
                    repeat_environment.Add(total);
                }

                ViewBag.ENVIRONMENT = emp_environment;
                ViewBag.ENVREP = repeat_environment.ToList();

                //Job
                List<int> repeat_job = new List<int>();
                var emp_job = survey_all.OrderBy(e => e.JobSatisfaction).Select(e => e.JobSatisfaction).Distinct();

                foreach (var job in emp_job)
                {
                    int total = 0;
                    var unique_number = survey_all.Where(e => e.JobSatisfaction == job).Select(e => e.SurveyId);

                    foreach (var number in unique_number)
                    {
                        total = total + employee.Count(e => e.SurveyId == number);
                    }
                    repeat_job.Add(total);
                }

                ViewBag.JOB = emp_job;
                ViewBag.REPJOB = repeat_job.ToList();

                //relationship
                List<int> repeat_relationship = new List<int>();
                var emp_relationship = survey_all.OrderBy(e => e.RelationshipSatisfaction).Select(e => e.RelationshipSatisfaction).Distinct();

                foreach (var relationship in emp_relationship)
                {
                    int total = 0;
                    var unique_number = survey_all.Where(e => e.RelationshipSatisfaction == relationship).Select(e => e.SurveyId);

                    foreach (var number in unique_number)
                    {
                        total = total + employee.Count(e => e.SurveyId == number);
                    }
                    repeat_relationship.Add(total);
                }

                ViewBag.REl = emp_relationship;
                ViewBag.RELREP = repeat_relationship.ToList();

                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
  
}
