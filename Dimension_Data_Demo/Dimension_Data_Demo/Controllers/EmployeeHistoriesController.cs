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

namespace Dimension_Data_Demo.Controllers
{
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
                int HistoryID = -1;
                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", (int)id);
                cmd.CommandText = ("Select HistoryID from dbo.Employee Where EmployeeNumber = @Id");
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while(reader.Read())
                {
                    HistoryID = reader.GetInt32(0);
                }
                await conn.CloseAsync();
                await cmd.DisposeAsync();
                await reader.CloseAsync();
                HttpContext.Session.SetInt32("his_employeeNumber", (int)id);
                HttpContext.Session.SetInt32("HistoryId", HistoryID);
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
                _context.Add(employeeHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeHistory);
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
                    try
                    {
                        //_context.Update(employeeHistory);
                        //await _context.SaveChangesAsync();
                        int historyId = -1;
                        var conn = _context.Database.GetDbConnection();
                        await conn.OpenAsync();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = (SqlConnection)conn;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@NumWorked",(int)employeeHistory.NumCompaniesWorked);
                        cmd.Parameters.AddWithValue("@TotWorked",(int)employeeHistory.TotalWorkingYears);
                        cmd.Parameters.AddWithValue("@YearComp",(int)employeeHistory.YearsAtCompany);
                        cmd.Parameters.AddWithValue("@YearCurr",(int)employeeHistory.YearsInCurrentRole);
                        cmd.Parameters.AddWithValue("@YearSince",(int)employeeHistory.YearsSinceLastPromotion);
                        cmd.Parameters.AddWithValue("@YearCurrMan",(int)employeeHistory.YearsWithCurrManager);
                        cmd.Parameters.AddWithValue("@TrainTime",(int)employeeHistory.TrainingTimesLastYear);
                        cmd.CommandText = ("Select HistoryID from dbo.EmployeeHistory Where NumCompaniesWorked = @NumWorked and TotalWorkingYears = @TotWorked and " +
                            "YearsAtCompany = @YearComp and YearsInCurrentRole = @YearCurr and YearsSinceLastPromotion = @YearSince and YearsWithCurrManager = @YearCurrMan " +
                            "and TrainingTimesLastYear = @TrainTime");
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        while(reader.Read())
                        {
                            historyId = reader.GetInt32(0);
                        }
                        await cmd.DisposeAsync();
                        await reader.CloseAsync();

                        cmd = new SqlCommand();
                        cmd.Connection = (SqlConnection)conn;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("his_employeeNumber"));
                        cmd.Parameters.AddWithValue("@EduNumber", historyId);
                        cmd.CommandText = "Update dbo.Employee Set HistoryID = @EduNumber Where EmployeeNumber=@EmpNumber";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.Dispose();
                        conn.Close();
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
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction("Index", "Employees");
                //return View(employeeHistory);
            }
        }

        // GET: EmployeeHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: EmployeeHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeHistory = await _context.EmployeeHistory.FindAsync(id);
            _context.EmployeeHistory.Remove(employeeHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeHistoryExists(int id)
        {
            return _context.EmployeeHistory.Any(e => e.HistoryId == id);
        }
    }
}
