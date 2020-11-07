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
    public class EmployeePerformancesController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public EmployeePerformancesController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: EmployeePerformances
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                int PerformanceID = -1;
                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", (int)id);
                cmd.CommandText = ("Select PerformanceID from dbo.Employee Where EmployeeNumber = @Id");
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    PerformanceID = reader.GetInt32(0);
                }
                await conn.CloseAsync();
                await cmd.DisposeAsync();
                await reader.CloseAsync();
                HttpContext.Session.SetInt32("per_employeeNumber", (int)id);
                HttpContext.Session.SetInt32("PerformanceId", PerformanceID);
            }

            var backupID = HttpContext.Session.GetInt32("PerformanceId");
            var dimention_data_demoContext = _context.EmployeePerformance.Where(e => e.PerformanceId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: EmployeePerformances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePerformance = await _context.EmployeePerformance
                .FirstOrDefaultAsync(m => m.PerformanceId == id);
            if (employeePerformance == null)
            {
                return NotFound();
            }

            return View(employeePerformance);
        }

        // GET: EmployeePerformances/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeePerformances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PerformanceId,PerformanceRating,WorkLifeBalance,JobInvolvement")] EmployeePerformance employeePerformance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeePerformance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeePerformance);
        }

        // GET: EmployeePerformances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePerformance = await _context.EmployeePerformance.FindAsync(id);
            if (employeePerformance == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldPerformanceModel", JsonConvert.SerializeObject(employeePerformance));
            return View(employeePerformance);
        }

        // POST: EmployeePerformances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PerformanceId,PerformanceRating,WorkLifeBalance,JobInvolvement")] EmployeePerformance employeePerformance)
        {
            if (id != employeePerformance.PerformanceId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(employeePerformance) == HttpContext.Session.GetString("oldPerformanceModel")) 
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        //_context.Update(employeePerformance);
                        //await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeePerformanceExists(employeePerformance.PerformanceId))
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
                return View(employeePerformance);
            }
        }

        // GET: EmployeePerformances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeePerformance = await _context.EmployeePerformance
                .FirstOrDefaultAsync(m => m.PerformanceId == id);
            if (employeePerformance == null)
            {
                return NotFound();
            }

            return View(employeePerformance);
        }

        // POST: EmployeePerformances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeePerformance = await _context.EmployeePerformance.FindAsync(id);
            _context.EmployeePerformance.Remove(employeePerformance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeePerformanceExists(int id)
        {
            return _context.EmployeePerformance.Any(e => e.PerformanceId == id);
        }
    }
}
