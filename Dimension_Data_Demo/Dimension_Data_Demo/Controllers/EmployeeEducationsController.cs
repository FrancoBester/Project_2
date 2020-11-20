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
                    //Is used to filter data to only show single persons education data from database
                    int EduactionID = ((int)_context.Employee.Where(e => e.EmployeeNumber == id).Select(e => e.EducationId).First());

                    HttpContext.Session.SetInt32("edu_employeeNumber", (int)id);//saves id in session to be used when the user returns to the page an does not use the main navigation page
                    HttpContext.Session.SetInt32("EducationId", (int)EduactionID);//saves id in session to be used when the user returns to the page an does not use the main navigation page
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("EducationId");
            var dimention_data_demoContext = _context.EmployeeEducation.Where(e => e.EducationId == backupID);//filter view to only show one employees education data
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

                var field_education = _context.EmployeeEducation.Select(e => e.EducationField).Distinct();

                foreach (var field in field_education)
                {
                    EducationFieldlist.Add(new SelectListItem() { Text = field.ToString() });
                }
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
                int education_ID = (int)_context.EmployeeEducation.Where(e => e.Education == employeeEducation.Education && e.EducationField == employeeEducation.EducationField).Select(e => e.EducationId).First();
                if (education_ID == 0)
                {
                    int new_education_ID = ((int)_context.EmployeeEducation.OrderByDescending(e => e.EducationId).Select(e => e.EducationId).First()) + 1;//gets the id of the new record that will be added into the table
                    employeeEducation.EducationId = new_education_ID;//assignes new it to model
                    _context.Add(employeeEducation);//addes id to model that will be added to database
                    await _context.SaveChangesAsync();//addes the new models info into the database

                    HttpContext.Session.SetInt32("newEducationID", new_education_ID);//addes id to session to be used later when adding user into the main table in the database
                }
                else
                {
                    HttpContext.Session.SetInt32("newEducationID", education_ID);//addes id to session to be used later when adding user into the main table in the database
                }
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

                var field_education = _context.EmployeeEducation.Select(e => e.EducationField).Distinct();

                foreach (var field in field_education)
                {
                    string stest = field.ToString();
                    EducationFieldlist.Add(new SelectListItem() { Text = field.ToString() });
                }
                ViewData["fieldData"] = EducationFieldlist;
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
                            int education_ID = (int)_context.EmployeeEducation.Where(e => e.Education == employeeEducation.Education && e.EducationField == employeeEducation.EducationField).Select(e => e.EducationId).First();
                            if (education_ID == 0)
                            {
                                education_ID = ((int)_context.EmployeeEducation.OrderByDescending(e => e.EducationId).Select(e => e.EducationId).First()) + 1;//gets the id of the new record that will be added into the database
                                employeeEducation.EducationId = education_ID;//assignes new id to model
                                _context.Add(employeeEducation);//addes id to model that will be added to database
                                await _context.SaveChangesAsync();//addes the new models info into the database
                            }

                            int employee_number = (int)HttpContext.Session.GetInt32("edu_employeeNumber");//gets employee number from session
                            var employee_model = _context.Employee.FirstOrDefault(e => e.EmployeeNumber == employee_number);//gets the employee data according to the employee number

                            Employee temp_employee = (Employee)employee_model;//comverts employee data into a employee model
                            temp_employee.EducationId = education_ID;//cahnges the eduaction id of the model to be the updated education id

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
    }
}
