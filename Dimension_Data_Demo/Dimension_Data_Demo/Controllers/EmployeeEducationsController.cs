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

namespace Dimension_Data_Demo.Controllers
{
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
            var backupID = HttpContext.Session.GetInt32("EducationID");
            if (id == null)
            {
                backupID = HttpContext.Session.GetInt32("EducationID");
                var dimention_data_demoContext = _context.EmployeeEducation.Where(e => e.EducationId == backupID);
                return View(await dimention_data_demoContext.ToListAsync());
            }
            else if (backupID == null)
            {
                HttpContext.Session.SetInt32("EducationID", (int)id);
                var dimention_data_demoContext = _context.EmployeeEducation.Where(e =>e.EducationId == id);
                return View(await dimention_data_demoContext.ToListAsync());
            }
            else
            {
                var dimention_data_demoContext = _context.EmployeeEducation.Where(e => e.EducationId == id);
                return View(await dimention_data_demoContext.ToListAsync());
            }

            //return View(await _context.EmployeeEducation.ToListAsync());
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
            return View();
        }

        // POST: EmployeeEducations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EducationId,Education,EducationField")] EmployeeEducation employeeEducation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeEducation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeEducation);
        }

        // GET: EmployeeEducations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
            if(JsonConvert.SerializeObject(employeeEducation) == HttpContext.Session.GetString("oldEducationModel"))
            {

            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeEducation);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(employeeEducation);
        }

        // GET: EmployeeEducations/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: EmployeeEducations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeEducation = await _context.EmployeeEducation.FindAsync(id);
            _context.EmployeeEducation.Remove(employeeEducation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeEducationExists(int id)
        {
            return _context.EmployeeEducation.Any(e => e.EducationId == id);
        }
    }
}
