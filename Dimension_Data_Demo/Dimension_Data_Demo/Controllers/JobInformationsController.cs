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
    public class JobInformationsController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public JobInformationsController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: JobInformations
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                try
                {
                    //used to filter data to only show a single persons job information
                    int JobID = (int)_context.Employee.Where(e => e.EmployeeNumber == id).Select(e => e.JobId).First();

                    HttpContext.Session.SetInt32("job_employeeNumber", (int)id);//saces data in session to be used later
                    HttpContext.Session.SetInt32("JobId", JobID);//saves data in session to be used later
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("JobId");
            var dimention_data_demoContext = _context.JobInformation.Where(e => e.JobId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: JobInformations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobInformation = await _context.JobInformation
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (jobInformation == null)
            {
                return NotFound();
            }

            return View(jobInformation);
        }

        // GET: JobInformations/Create
        public IActionResult Create()
        {
            try
            {
                List<SelectListItem> JobRoleFieldList = new List<SelectListItem>();
                List<SelectListItem> JobDepartmentFieldList = new List<SelectListItem>();
                List<SelectListItem> TravelFieldList = new List<SelectListItem>();


                var field_job = _context.JobInformation.Select(e => e.JobRole).Distinct();

                foreach(var job in field_job)
                {
                    JobRoleFieldList.Add(new SelectListItem() { Text = job.ToString() });
                }

                var field_department = _context.JobInformation.Select(e => e.Department).Distinct();

                foreach(var department in field_department)
                {
                    JobDepartmentFieldList.Add(new SelectListItem() { Text = department.ToString() });
                }

                var field_travel = _context.JobInformation.Select(e => e.BusinessTravel).Distinct();

                foreach(var travel in field_travel)
                {
                    TravelFieldList.Add(new SelectListItem() { Text = travel.ToString() });
                }

                
                ViewData["RolefieldData"] = JobRoleFieldList;
                ViewData["DepartmentfieldData"] = JobDepartmentFieldList;
                ViewData["TravelfieldData"] = TravelFieldList;
                      
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: JobInformations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,JobRole,Department,JobLevel,StandardHours,EmployeeCount,BusinessTravel,StockOptionLevel")] JobInformation jobInformation)
        {
            if (jobInformation.JobLevel <= -1 || jobInformation.StandardHours <= -1 || jobInformation.EmployeeCount <= -1 || jobInformation.StockOptionLevel <= -1)//ensures that all data intered is logically correct
            {
                return RedirectToAction(nameof(Index));
            }

            if (jobInformation.JobRole == null )//assignes value to model if user did not assign them 
            {
                jobInformation.JobRole = "Healthcare Representative";
            }

            if (jobInformation.Department == null)//assignes value to model if user did not assign them 
            {
                jobInformation.Department = "Human Resources";
            }

            if (jobInformation.BusinessTravel == null)//assignes value to model if user did not assign them 
            {
                jobInformation.BusinessTravel = "Non-Travel";
            }

            try
            {
                int job_ID = (int)_context.JobInformation.Where(e => e.JobRole == jobInformation.JobRole && e.Department == jobInformation.Department && e.JobLevel == jobInformation.JobLevel &&
                e.StandardHours == jobInformation.StandardHours && e.EmployeeCount == jobInformation.EmployeeCount && e.BusinessTravel == jobInformation.BusinessTravel && 
                e.StockOptionLevel == jobInformation.StockOptionLevel).Select(e => e.JobId).FirstOrDefault();//gets id of record that meets all where clauses

                if(job_ID == 0)//if 0 then a new record needs to be added
                {
                    int new_job_ID = ((int)_context.JobInformation.OrderByDescending(e => e.JobId).Select(e => e.JobId).First()) + 1;//gets the id of the new record that will be added into the table
                    jobInformation.JobId = new_job_ID;//assignes new it to model
                    _context.Add(jobInformation);//addes id to model that will be added to database
                    await _context.SaveChangesAsync();//addes the new models info into the database

                    HttpContext.Session.SetInt32("newJobID", new_job_ID);//addes id to session to be used later when adding user into the main table in the database
                }
                else
                {
                    HttpContext.Session.SetInt32("newJobID", job_ID);//addes id to session to be used later when adding user into the main table in the database
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return RedirectToAction("Index","Home");
            }
            
            return RedirectToAction("Create", "Surveys");
        }

        // GET: JobInformations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                List<SelectListItem> JobRoleFieldList = new List<SelectListItem>();
                List<SelectListItem> JobDepartmentFieldList = new List<SelectListItem>();
                List<SelectListItem> TravelFieldList = new List<SelectListItem>();


                var field_job = _context.JobInformation.Select(e => e.JobRole).Distinct();

                foreach (var job in field_job)
                {
                    JobRoleFieldList.Add(new SelectListItem() { Text = job.ToString() });
                }

                var field_department = _context.JobInformation.Select(e => e.Department).Distinct();

                foreach (var department in field_department)
                {
                    JobDepartmentFieldList.Add(new SelectListItem() { Text = department.ToString() });
                }

                var field_travel = _context.JobInformation.Select(e => e.BusinessTravel).Distinct();

                foreach (var travel in field_travel)
                {
                    TravelFieldList.Add(new SelectListItem() { Text = travel.ToString() });
                }

                ViewData["RolefieldData"] = JobRoleFieldList;
                ViewData["DepartmentfieldData"] = JobDepartmentFieldList;
                ViewData["TravelfieldData"] = TravelFieldList;
                
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var jobInformation = await _context.JobInformation.FindAsync(id);
            if (jobInformation == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldJobModel", JsonConvert.SerializeObject(jobInformation));
            return View(jobInformation);
        }

        // POST: JobInformations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,JobRole,Department,JobLevel,StandardHours,EmployeeCount,BusinessTravel,StockOptionLevel")] JobInformation jobInformation)
        {
            if (id != jobInformation.JobId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(jobInformation) == HttpContext.Session.GetString("oldJobModel"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (jobInformation.JobLevel <= -1 || jobInformation.StandardHours <= -1 || jobInformation.EmployeeCount <= -1 || jobInformation.StockOptionLevel <= -1)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else if (jobInformation.JobRole == null || jobInformation.Department == null || jobInformation.BusinessTravel == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    try
                    {
                        try
                        {
                            int job_ID = (int)_context.JobInformation.Where(e => e.JobRole == jobInformation.JobRole && e.Department == jobInformation.Department && e.JobLevel == jobInformation.JobLevel &&
                            e.StandardHours == jobInformation.StandardHours && e.EmployeeCount == jobInformation.EmployeeCount && e.BusinessTravel == jobInformation.BusinessTravel &&
                            e.StockOptionLevel == jobInformation.StockOptionLevel).Select(e => e.JobId).First();//gets id of record that meets all where clauses

                            if (job_ID == 0)//if 0 then a new record needs to be added
                            {
                                int new_job_ID = ((int)_context.JobInformation.OrderByDescending(e => e.JobId).Select(e => e.JobId).First()) + 1;//gets the id of the new record that will be added into the table
                                jobInformation.JobId = new_job_ID;//assignes new it to model
                                _context.Add(jobInformation);//addes id to model that will be added to database
                                await _context.SaveChangesAsync();//addes the new models info into the database
                            }

                            int employee_number = (int)HttpContext.Session.GetInt32("job_employeeNumber");//gets employee number from session
                            var employee_model = _context.Employee.FirstOrDefault(e => e.EmployeeNumber == employee_number);//gets the employee data according to the employee number

                            Employee temp_employee = (Employee)employee_model;//comverts employee data into a employee model
                            temp_employee.JobId = job_ID;//cahnges the eduaction id of the model to be the updated education id

                            _context.Update(temp_employee);//addes employee model to db context
                            await _context.SaveChangesAsync();//update database with new data from employee model
                        }
                        catch(Exception ex)
                        {
                            string error = ex.ToString();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!JobInformationExists(jobInformation.JobId))
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

        private bool JobInformationExists(int id)
        {
            return _context.JobInformation.Any(e => e.JobId == id);
        }

    }
}
