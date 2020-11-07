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
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace Dimension_Data_Demo.Controllers
{
    public class EmployeeDetailsController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public EmployeeDetailsController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: EmployeeDetails
        public async Task<IActionResult> Index(int? id)
        {
            if(id != null)
            {
                HttpContext.Session.SetInt32("DetailID", (int)id);
            }

            var backupID = HttpContext.Session.GetInt32("DetailID");

            var dimention_data_demoContext = _context.EmployeeDetails.Include(e => e.Gender).Include(e => e.Marital).Where(e => e.DetailsId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: EmployeeDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDetails = await _context.EmployeeDetails
                .Include(e => e.Gender)
                .Include(e => e.Marital)
                .FirstOrDefaultAsync(m => m.DetailsId == id);
            if (employeeDetails == null)
            {
                return NotFound();
            }

            return View(employeeDetails);
        }

        // GET: EmployeeDetails/Create
        public IActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "GenderId");
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalId");
            return View();
        }

        // POST: EmployeeDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetailsId,Age,Attrition,DistanceFromHome,Over18,MaritalId,GenderId")] EmployeeDetails employeeDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "GenderId", employeeDetails.GenderId);
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalId", employeeDetails.MaritalId);
            return View(employeeDetails);
        }

        // GET: EmployeeDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var employeeDetails = await _context.EmployeeDetails.FindAsync(id);
            if (employeeDetails == null)
            {
                return NotFound();
            }
            
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "Gender1", employeeDetails.GenderId);
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalStatus1", employeeDetails.MaritalId);
            HttpContext.Session.SetString("oldEmployeeDetails", JsonConvert.SerializeObject(employeeDetails));//changes current model information into string and stores in a session,used later
            return View(employeeDetails);
        }

        // POST: EmployeeDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetailsId,Age,Attrition,DistanceFromHome,Over18,MaritalId,GenderId")] EmployeeDetails employeeDetails)
        {
            if (id != employeeDetails.DetailsId)
            {
                return NotFound();
            }

            var old_Model_Info = HttpContext.Session.GetString("oldEmployeeDetails");//gets model details before changes could have been model
            var new_Model_Info = JsonConvert.SerializeObject(employeeDetails);//get new model info te see if info was changed

            if(old_Model_Info == new_Model_Info)//compares string to see if model has been changed or if is still the same
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(employeeDetails);
                        await _context.SaveChangesAsync();
                        //var conn = _context.Database.GetDbConnection();
                        //conn.Open();
                        //SqlCommand cmd = new SqlCommand();
                        //cmd.Connection = (SqlConnection)conn;
                        //cmd.CommandType = System.Data.CommandType.Text;
                        //cmd.Parameters.AddWithValue()
                        //cmd.CommandText = "Select "
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeDetailsExists(employeeDetails.DetailsId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    //return RedirectToAction("Index","EmployeeDetailsController",new { id = 1});
                    return RedirectToAction(nameof(Index));
                }
            }
            
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "GenderId", employeeDetails.GenderId);
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalId", employeeDetails.MaritalId);
            return View(employeeDetails);
        }

        // GET: EmployeeDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeDetails = await _context.EmployeeDetails
                .Include(e => e.Gender)
                .Include(e => e.Marital)
                .FirstOrDefaultAsync(m => m.DetailsId == id);
            if (employeeDetails == null)
            {
                return NotFound();
            }

            return View(employeeDetails);
        }

        // POST: EmployeeDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeDetails = await _context.EmployeeDetails.FindAsync(id);
            _context.EmployeeDetails.Remove(employeeDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeDetailsExists(int id)
        {
            return _context.EmployeeDetails.Any(e => e.DetailsId == id);
        }
    }
}
