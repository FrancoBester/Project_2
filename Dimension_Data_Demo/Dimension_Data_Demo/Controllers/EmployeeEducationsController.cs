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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Dimension_Data_Demo.Controllers
{
    [Authorize]
    public class EmployeeEducationsController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public EmployeeEducationsController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: EmployeeEducations
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                try
                {
                    //Is used to filter data to only show single person data
                    int EduactionID = -1;
                    var conn = _context.Database.GetDbConnection();
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = (SqlConnection)conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", (int)id);
                    cmd.CommandText = ("Select EducationID from dbo.Employee Where EmployeeNumber = @Id");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        EduactionID = reader.GetInt32(0);
                    }
                    await conn.CloseAsync();
                    await cmd.DisposeAsync();
                    await reader.CloseAsync();
                    HttpContext.Session.SetInt32("edu_employeeNumber", (int)id);
                    HttpContext.Session.SetInt32("EducationId", (int)EduactionID);
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("EducationId");
            var dimention_data_demoContext = _context.EmployeeEducation.Where(e => e.EducationId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: EmployeeEducations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeEducation = await _context.EmployeeEducation
                .FirstOrDefaultAsync(m => m.EducationId == id);
            if (employeeEducation == null)
            {
                return NotFound();
            }

            return View(employeeEducation);
        }

        // GET: EmployeeEducations/Create
        public IActionResult Create()
        {
            try//gets only existing education fields from the database and addes to a drop down list
            {
                List<SelectListItem> EducationFieldlist = new List<SelectListItem>();
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(EducationField) from dbo.EmployeeEducation");
                SqlDataReader reader =  cmd.ExecuteReader();
                while (reader.Read())
                {
                    EducationFieldlist.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }

                conn.Close();
                cmd.Dispose();
                reader.Close();
                ViewData["fieldData"] = EducationFieldlist;
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: EmployeeEducations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EducationId,Education,EducationField")] EmployeeEducation employeeEducation)
        {
           
            if (employeeEducation.Education <= -1)
            {
                return RedirectToAction(nameof(Index));
            }
            else if (employeeEducation.EducationField == null)
            {
                employeeEducation.EducationField = "Human Resources";
            }


            try
            {
                int educationId = get_set_EducationId(employeeEducation, "Select");
                if (educationId == -1)
                {
                    educationId = get_set_EducationId(employeeEducation, "Insert");
                }
                HttpContext.Session.SetInt32("newEducationID", educationId);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }

            return  RedirectToAction("Create", "EmployeeHistories"); ;
        }

        // GET: EmployeeEducations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try//gets only existing education fields from the database and addes to a drop down list
            {
                List<SelectListItem> EducationFieldlist = new List<SelectListItem>();
                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = ("Select distinct(EducationField) from dbo.EmployeeEducation");
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    EducationFieldlist.Add(new SelectListItem() { Text = reader.GetValue(0).ToString() });
                }


                await conn.CloseAsync();
                await cmd.DisposeAsync();
                await reader.CloseAsync();
                ViewData["fieldData"] = EducationFieldlist;
            }
            catch(Exception)
            {
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var employeeEducation = await _context.EmployeeEducation.FindAsync(id);
            if (employeeEducation == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldEducationModel", JsonConvert.SerializeObject(employeeEducation));
            return View(employeeEducation);
        }

        // POST: EmployeeEducations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EducationId,Education,EducationField")] EmployeeEducation employeeEducation)
        {
            if (id != employeeEducation.EducationId)
            {
                return NotFound();
            }

            if(JsonConvert.SerializeObject(employeeEducation) == HttpContext.Session.GetString("oldEducationModel"))// compares old and current model to limit update query to database when models are same
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if(employeeEducation.Education <= -1)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else if (employeeEducation.EducationField == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    try
                    {
                        try
                        {
                            int educationId = get_set_EducationId(employeeEducation, "Select");
                            if (educationId == -1)
                            {
                                educationId = get_set_EducationId(employeeEducation, "Insert");
                            }

                            var conn = _context.Database.GetDbConnection();
                            await conn.OpenAsync();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = (SqlConnection)conn;
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("edu_employeeNumber"));
                            cmd.Parameters.AddWithValue("@EduNumber", educationId);
                            cmd.CommandText = "Update dbo.Employee Set EducationID = @EduNumber Where EmployeeNumber=@EmpNumber";

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
                        if (!EmployeeEducationExists(employeeEducation.EducationId))
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

        private bool EmployeeEducationExists(int id)
        {
            return _context.EmployeeEducation.Any(e => e.EducationId == id);
        }

        public int get_set_EducationId(EmployeeEducation employeeEducation, string command_type)
        {
            int educationId = -1;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Education", (int)employeeEducation.Education);
                cmd.Parameters.AddWithValue("@EduField", employeeEducation.EducationField.ToString());

                if (command_type == "Select")
                {
                    cmd.CommandText = ("Select EducationID from dbo.EmployeeEducation Where Education = @Education and EducationField = @EduField");
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        educationId = reader.GetInt32(0);
                    }
                    cmd.Dispose();
                    reader.Close();
                    conn.Close();
                }
                else if (command_type == "Insert")
                {
                    cmd.CommandText = ("Insert Into dbo.EmployeeEducation(EducationID,Education,EducationField) " +
                        "Values((Select count(*)+1 from dbo.EmployeeEducation),@Education,@EduField)");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                    educationId = get_set_EducationId(employeeEducation, "Select");
                }
            }
            catch(Exception)
            {
                return -1;
            }
            return educationId;
        }
    }
}
