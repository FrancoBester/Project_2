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
                    int SurveyID = -1;
                    var conn = _context.Database.GetDbConnection();
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = (SqlConnection)conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", (int)id);
                    cmd.CommandText = ("Select SurveyID from dbo.Employee Where EmployeeNumber = @Id");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        SurveyID = reader.GetInt32(0);
                    }
                    await conn.CloseAsync();
                    await cmd.DisposeAsync();
                    await reader.CloseAsync();
                    HttpContext.Session.SetInt32("sur_employeeNumber", (int)id);
                    HttpContext.Session.SetInt32("SurveyId", SurveyID);
                }
                catch(Exception)
                {
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
                int surveyId = get_set_SurveyId(surveys, "Select"); ;
                if (surveyId == -1)
                {
                    surveyId = get_set_SurveyId(surveys, "Insert");
                }
                HttpContext.Session.SetInt32("newSurveyID", surveyId);
            }
            catch (Exception)
            {
                ViewBag.Message = "There was an error updating the information. Please try again later";
                return View();
            }

            return RedirectToAction("Create","Employees");
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

            if (JsonConvert.SerializeObject(surveys) == HttpContext.Session.GetString("oldSurveyModel"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if(surveys.EnvironmentSatisfaction <= -1 || surveys.JobSatisfaction <= -1 || surveys.RelationshipSatisfaction <= -1)
                    {
                        ViewBag.Message = "All numbers must be positive values";
                        return View();
                    }

                    try
                    {
                        try
                        {
                            int surveyId = get_set_SurveyId(surveys, "Select"); ;
                            if (surveyId == -1)
                            {
                                surveyId = get_set_SurveyId(surveys, "Insert");
                            }
                            var conn = _context.Database.GetDbConnection();
                            await conn.OpenAsync();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = (SqlConnection)conn;
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("sur_employeeNumber"));
                            cmd.Parameters.AddWithValue("@SurNumber", surveyId);
                            cmd.CommandText = ("Update dbo.Employee Set SurveyID = @SurNumber Where EmployeeNumber = @EmpNumber");
                            await cmd.ExecuteNonQueryAsync();

                            await cmd.DisposeAsync();
                            await conn.CloseAsync();
                        }
                        catch(Exception)
                        {
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
        public int get_set_SurveyId(Surveys surveys, string command_type)
        {
            int surveyId = -1;
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = (SqlConnection)conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddWithValue("@EnvSat", (int)surveys.EnvironmentSatisfaction);
            cmd.Parameters.AddWithValue("@JobSat", (int)surveys.JobSatisfaction);
            cmd.Parameters.AddWithValue("@RelSat",(int)surveys.RelationshipSatisfaction);

            if (command_type == "Select")
            {
                cmd.CommandText = ("Select SurveyID from dbo.Surveys Where EnvironmentSatisfaction = @EnvSat and JobSatisfaction = @JobSat and RelationshipSatisfaction = @RelSat");
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    surveyId = reader.GetInt32(0);
                }
                cmd.Dispose();
                reader.Close();
                conn.Close();
            }
            else if (command_type == "Insert")
            {
                cmd.CommandText = ("Insert Into dbo.Surveys(SurveyID,EnvironmentSatisfaction, JobSatisfaction,RelationshipSatisfaction) " +
                    "Values((Select count(*)+1 from dbo.Surveys),@EnvSat,@JobSat,@RelSat)");
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                surveyId = get_set_SurveyId(surveys, "Select");
            }
            return surveyId;
        }
    }
}
