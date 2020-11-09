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
    public class EmployeeHistoriesController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public EmployeeHistoriesController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: EmployeeHistories
        public async Task<IActionResult> Index(int? id)
        {
            if(id != null)
            {
                try
                {
                    int HistoryID = -1;
                    var conn = _context.Database.GetDbConnection();
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = (SqlConnection)conn;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", (int)id);
                    cmd.CommandText = ("Select HistoryID from dbo.Employee Where EmployeeNumber = @Id");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        HistoryID = reader.GetInt32(0);
                    }
                    await conn.CloseAsync();
                    await cmd.DisposeAsync();
                    await reader.CloseAsync();
                    HttpContext.Session.SetInt32("his_employeeNumber", (int)id);
                    HttpContext.Session.SetInt32("HistoryId", HistoryID);
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("HistoryId");
            var dimention_data_demoContext = _context.EmployeeHistory.Where(e => e.HistoryId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: EmployeeHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeHistory = await _context.EmployeeHistory
                .FirstOrDefaultAsync(m => m.HistoryId == id);
            if (employeeHistory == null)
            {
                return NotFound();
            }

            return View(employeeHistory);
        }

        // GET: EmployeeHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistoryId,NumCompaniesWorked,TotalWorkingYears,YearsAtCompany,YearsInCurrentRole,YearsSinceLastPromotion,YearsWithCurrManager,TrainingTimesLastYear")] EmployeeHistory employeeHistory)
        {
            if (ModelState.IsValid)
            {
                if (employeeHistory.NumCompaniesWorked <= -1 || employeeHistory.TotalWorkingYears <= -1 || employeeHistory.YearsAtCompany <= -1 || employeeHistory.YearsInCurrentRole <= -1 || employeeHistory.YearsSinceLastPromotion <= -1 || employeeHistory.YearsWithCurrManager <= -1 || employeeHistory.TrainingTimesLastYear <= -1)
                {
                    ViewBag.Message = "All numbers must be positive values";
                    return View();
                }

                try
                {
                    try
                    {
                        int historyId = get_set_HistoryId(employeeHistory, "Select");
                        if (historyId == -1)
                        {
                            historyId = get_set_HistoryId(employeeHistory, "Insert");
                        }
                        HttpContext.Session.SetInt32("newHistoryID", historyId);
                    }
                    catch (Exception)
                    {
                        ViewBag.Message = "There was an error updating the information. Please try again later";
                        return View();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeHistoryExists(employeeHistory.HistoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Create", "CostToCompanies");
        }

        // GET: EmployeeHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeHistory = await _context.EmployeeHistory.FindAsync(id);
            if (employeeHistory == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldHistoryModel", JsonConvert.SerializeObject(employeeHistory));
            return View(employeeHistory);
        }

        // POST: EmployeeHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HistoryId,NumCompaniesWorked,TotalWorkingYears,YearsAtCompany,YearsInCurrentRole,YearsSinceLastPromotion,YearsWithCurrManager,TrainingTimesLastYear")] EmployeeHistory employeeHistory)
        {
            if (id != employeeHistory.HistoryId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(employeeHistory) == HttpContext.Session.GetString("oldHistoryModel"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if(employeeHistory.NumCompaniesWorked <= -1 || employeeHistory.TotalWorkingYears <= -1 || employeeHistory.YearsAtCompany <= -1 || employeeHistory.YearsInCurrentRole <= -1 || employeeHistory.YearsSinceLastPromotion <= -1 || employeeHistory.YearsWithCurrManager <= -1 || employeeHistory.TrainingTimesLastYear <= -1 )
                    {
                        ViewBag.Message = "All numbers must be positive values";
                        return View();
                    }

                    try
                    {
                        try
                        {
                            int historyId = get_set_HistoryId(employeeHistory, "Select");
                            if(historyId == -1)
                            {
                                historyId = get_set_HistoryId(employeeHistory, "Insert");
                            }

                            var conn = _context.Database.GetDbConnection();
                            await conn.OpenAsync();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = (SqlConnection)conn;
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("his_employeeNumber"));
                            cmd.Parameters.AddWithValue("@EduNumber", historyId);
                            cmd.CommandText = "Update dbo.Employee Set HistoryID = @EduNumber Where EmployeeNumber=@EmpNumber";
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
                        if (!EmployeeHistoryExists(employeeHistory.HistoryId))
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

        private bool EmployeeHistoryExists(int id)
        {
            return _context.EmployeeHistory.Any(e => e.HistoryId == id);
        }

        public int get_set_HistoryId(EmployeeHistory employeeHistory, string command_type)
        {
            int historyId = -1;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@NumWorked", (int)employeeHistory.NumCompaniesWorked);
                cmd.Parameters.AddWithValue("@TotWorked", (int)employeeHistory.TotalWorkingYears);
                cmd.Parameters.AddWithValue("@YearComp", (int)employeeHistory.YearsAtCompany);
                cmd.Parameters.AddWithValue("@YearCurr", (int)employeeHistory.YearsInCurrentRole);
                cmd.Parameters.AddWithValue("@YearSince", (int)employeeHistory.YearsSinceLastPromotion);
                cmd.Parameters.AddWithValue("@YearCurrMan", (int)employeeHistory.YearsWithCurrManager);
                cmd.Parameters.AddWithValue("@TrainTime", (int)employeeHistory.TrainingTimesLastYear);

                if (command_type == "Select")
                {
                    cmd.CommandText = ("Select HistoryID from dbo.EmployeeHistory Where NumCompaniesWorked = @NumWorked and TotalWorkingYears = @TotWorked and " +
                             "YearsAtCompany = @YearComp and YearsInCurrentRole = @YearCurr and YearsSinceLastPromotion = @YearSince and YearsWithCurrManager = @YearCurrMan " +
                             "and TrainingTimesLastYear = @TrainTime");
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        historyId = reader.GetInt32(0);
                    }
                    cmd.Dispose();
                    reader.Close();
                    conn.Close();
                }
                else if (command_type == "Insert")
                {
                    cmd.CommandText = ("Insert Into dbo.EmployeeHistory(HistoryID,NumCompaniesWorked,TotalWorkingYears,YearsAtCompany,YearsInCurrentRole,YearsSinceLastPromotion,YearsWithCurrManager,TrainingTimesLastYear) " +
                        "Values((Select count(*)+1 from dbo.EmployeeHistory),@NumWorked,@TotWorked,@YearComp,@YearCurr,@YearSince,@YearCurrMan,@TrainTime)");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                    historyId = get_set_HistoryId(employeeHistory, "Select");
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return historyId;
        }
    }
}
