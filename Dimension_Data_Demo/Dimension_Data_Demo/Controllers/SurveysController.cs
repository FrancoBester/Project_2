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
    public class SurveysController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public SurveysController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: Surveys
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                try
                {
                    //Is used to filter data to only show single persons education data from database
                    int SurveyID = ((int)_context.Employee.Where(e => e.EmployeeNumber == id).Select(e => e.SurveyId).First());

                    HttpContext.Session.SetInt32("sur_employeeNumber", (int)id);//saves id in session to be used when the user returns to the page an does not use the main navigation page
                    HttpContext.Session.SetInt32("SurveyId", (int)SurveyID);//saves id in session to be used when the user returns to the page an does not use the main navigation page

                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("SurveyId");
            var dimention_data_demoContext = _context.Surveys.Where(e => e.SurveyId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: Surveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveys = await _context.Surveys
                .FirstOrDefaultAsync(m => m.SurveyId == id);
            if (surveys == null)
            {
                return NotFound();
            }

            return View(surveys);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyId,EnvironmentSatisfaction,JobSatisfaction,RelationshipSatisfaction")] Surveys surveys)
        {
            if (surveys.EnvironmentSatisfaction <= -1 || surveys.JobSatisfaction <= -1 || surveys.RelationshipSatisfaction <= -1)
            {
                ViewBag.Message = "All numbers must be positive values";
                return View();
            }

            try
            {
                int survey_ID = (int)_context.Surveys.Where(e => e.EnvironmentSatisfaction == surveys.EnvironmentSatisfaction && e.JobSatisfaction == surveys.JobSatisfaction &&
                e.RelationshipSatisfaction == surveys.RelationshipSatisfaction).Select(e => e.SurveyId).FirstOrDefault();//gets id of record that meets all where clauses

                if (survey_ID == 0)//if 0 then a new record needs to be added
                {
                    int new_survey_ID = ((int)_context.Surveys.OrderByDescending(e => e.SurveyId).Select(e => e.SurveyId).First()) + 1;//gets the id of the new record that will be added into the table
                    surveys.SurveyId = new_survey_ID;//assignes new id to model
                    _context.Add(surveys);//adds if to model that will be added to database
                    await _context.SaveChangesAsync();//adds the new model info into the database

                    HttpContext.Session.SetInt32("newSurveyID", new_survey_ID);//addes id to session to be used later when adding user into the main table in the database
                }
                else
                {
                    HttpContext.Session.SetInt32("newSurveyID", survey_ID);//addes id to session to be used later when adding user into the main table in the database
                }

            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                ViewBag.Message = "There was an error updating the information. Please try again later";
                return View();
            }

            return RedirectToAction("Create", "Employees");
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveys = await _context.Surveys.FindAsync(id);
            if (surveys == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldSurveyModel", JsonConvert.SerializeObject(surveys));
            return View(surveys);
        }

        // POST: Surveys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SurveyId,EnvironmentSatisfaction,JobSatisfaction,RelationshipSatisfaction")] Surveys surveys)
        {
            if (id != surveys.SurveyId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(surveys) == HttpContext.Session.GetString("oldSurveyModel"))//checks if changes where made to the data, if not dont waste time updating database
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (surveys.EnvironmentSatisfaction <= -1 || surveys.JobSatisfaction <= -1 || surveys.RelationshipSatisfaction <= -1)
                    {
                        ViewBag.Message = "All numbers must be positive values";
                        return View();
                    }

                    try
                    {
                        try
                        {
                            int survey_ID = (int)_context.Surveys.Where(e => e.EnvironmentSatisfaction == surveys.EnvironmentSatisfaction && e.JobSatisfaction == surveys.JobSatisfaction &&
                            e.RelationshipSatisfaction == surveys.RelationshipSatisfaction).Select(e => e.SurveyId).First();//gets id of record that meets all where clauses

                            if (survey_ID == 0)
                            {
                                survey_ID = ((int)_context.Surveys.OrderByDescending(e => e.SurveyId).Select(e => e.SurveyId).First()) + 1;//gets the id of the new record that will be added into the database
                                surveys.SurveyId = survey_ID;//assignes new id to model
                                _context.Add(surveys);//addes model to context that will be used for update
                                await _context.SaveChangesAsync();//adds the new model info into the database
                            }

                            int employee_number = (int)HttpContext.Session.GetInt32("edu_employeeNumber");//gets employee number from session
                            var employee_model = _context.Employee.FirstOrDefault(e => e.EmployeeNumber == employee_number);//gets the employee data according to the employee number

                            Employee temp_employee = (Employee)employee_model;//comverts employee data into a employee model
                            temp_employee.SurveyId = survey_ID;//cahnges the survey id of the model to be the updated survey id

                            _context.Update(temp_employee);//addes employee model to db context
                            await _context.SaveChangesAsync();//update database with new data from employee model
                        }
                        catch (Exception ex)
                        {
                            string error = ex.ToString();
                            ViewBag.Message = "There was an error updating the information. Please try again later";
                            return View();
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SurveysExists(surveys.SurveyId))
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

        private bool SurveysExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }

    }
}
