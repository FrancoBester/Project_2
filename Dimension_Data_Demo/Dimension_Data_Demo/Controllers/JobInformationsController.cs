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
                    int JobID = -1;
                    var conn = _context.Database.GetDbConnection();
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = (SqlConnection)conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", (int)id);
                    cmd.CommandText = ("Select JobID from dbo.Employee Where EmployeeNumber = @Id");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        JobID = reader.GetInt32(0);
                    }
                    await conn.CloseAsync();
                    await cmd.DisposeAsync();
                    await reader.CloseAsync();
                    HttpContext.Session.SetInt32("job_employeeNumber", (int)id);
                    HttpContext.Session.SetInt32("JobId", JobID);
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

                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(JobRole) from dbo.JobInformation");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    JobRoleFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                cmd.Dispose();
                reader.Close();

                cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(Department) from dbo.JobInformation");
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    JobDepartmentFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                cmd.Dispose();
                reader.Close();

                cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(BusinessTravel) from dbo.JobInformation");
                reader =cmd.ExecuteReader();
                while (reader.Read())
                {
                    TravelFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                cmd.Dispose();
                reader.Close();
                conn.Close();

                ViewData["RolefieldData"] = JobRoleFieldList;
                ViewData["DepartmentfieldData"] = JobDepartmentFieldList;
                ViewData["TravelfieldData"] = TravelFieldList;

            }
            catch (Exception)
            {
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
            if (jobInformation.JobLevel <= -1 || jobInformation.StandardHours <= -1 || jobInformation.EmployeeCount <= -1 || jobInformation.StockOptionLevel <= -1)
            {
                return RedirectToAction(nameof(Index));
            }
            if (jobInformation.JobRole == null )
            {
                jobInformation.JobRole = "Healthcare Representative";
            }
            if (jobInformation.Department == null)
            {
                jobInformation.Department = "Human Resources";
            }
            if (jobInformation.BusinessTravel == null)
            {
                jobInformation.BusinessTravel = "Non-Travel";
            }

            try
            {
                int jobId = get_set_JobId(jobInformation, "Select"); ;
                if (jobId == -1)
                {
                    jobId = get_set_JobId(jobInformation, "Insert");
                }
                HttpContext.Session.SetInt32("newJobID", jobId);
            }
            catch (Exception)
            {
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

                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(JobRole) from dbo.JobInformation");
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    JobRoleFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                await cmd.DisposeAsync();
                await reader.CloseAsync();

                cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(Department) from dbo.JobInformation");
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    JobDepartmentFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                await cmd.DisposeAsync();
                await reader.CloseAsync();

                cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(BusinessTravel) from dbo.JobInformation");
                reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    TravelFieldList.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }
                await cmd.DisposeAsync();
                await reader.CloseAsync();
                await conn.CloseAsync();

                ViewData["RolefieldData"] = JobRoleFieldList;
                ViewData["DepartmentfieldData"] = JobDepartmentFieldList;
                ViewData["TravelfieldData"] = TravelFieldList;
                
            }
            catch(Exception)
            {
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
                            int jobId = get_set_JobId(jobInformation, "Select"); ;
                            if (jobId == -1)
                            {
                                jobId = get_set_JobId(jobInformation, "Insert");
                            }
                            var conn = _context.Database.GetDbConnection();
                            await conn.OpenAsync();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = (SqlConnection)conn;
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("job_employeeNumber"));
                            cmd.Parameters.AddWithValue("@JobNumber", jobId);
                            cmd.CommandText = ("Update dbo.Employee Set JobID = @JobNumber Where EmployeeNumber = @EmpNumber");
                            await cmd.ExecuteNonQueryAsync();
                            await cmd.DisposeAsync();
                            await conn.CloseAsync();
                        }
                        catch(Exception)
                        {
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

        public int get_set_JobId(JobInformation jobInformation, string command_type)
        {
            int jobId = -1;
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = (SqlConnection)conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddWithValue("@JobRole", (jobInformation.JobRole).ToString());
            cmd.Parameters.AddWithValue("@Department", (jobInformation.Department).ToString());
            cmd.Parameters.AddWithValue("@JobLevel", (int)jobInformation.JobLevel);
            cmd.Parameters.AddWithValue("@StanHours", (int)jobInformation.StandardHours);
            cmd.Parameters.AddWithValue("@EmpCount", (int)jobInformation.EmployeeCount);
            cmd.Parameters.AddWithValue("@BusTravel", (jobInformation.BusinessTravel).ToString());
            cmd.Parameters.AddWithValue("@StockLevel", (int)jobInformation.StockOptionLevel);

            if (command_type == "Select")
            {
                cmd.CommandText = ("Select JobID from dbo.JobInformation Where JobRole = @JobRole and Department = @Department and JobLevel = @JobLevel and StandardHours = @StanHours " +
                    "and EmployeeCount = @EmpCount and BusinessTravel = @BusTravel and StockOptionLevel = @StockLevel");
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    jobId = reader.GetInt32(0);
                }
                cmd.Dispose();
                reader.Close();
                conn.Close();
            }
            else if (command_type == "Insert")
            {
                cmd.CommandText = ("Insert Into dbo.JobInformation(JobID,JobRole,Department,JobLevel,StandardHours,EmployeeCount,BusinessTravel,StockOptionLevel) " +
                    "Values((Select count(*)+1 from dbo.JobInformation),@JobRole,@Department,@JobLevel,@StanHours,@EmpCount,@BusTravel,@StockLevel)");
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                jobId = get_set_JobId(jobInformation, "Select");
            }
            return jobId;
        }
    }
}
