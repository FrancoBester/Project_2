using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dimension_Data_Demo.Data;
using Dimension_Data_Demo.Models;

namespace Dimension_Data_Demo.Controllers
{
    public class SurveysController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public SurveysController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            return View(await _context.Surveys.ToListAsync());
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
            if (ModelState.IsValid)
            {
                _context.Add(surveys);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surveys);
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveys);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(surveys);
        }

        // GET: Surveys/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surveys = await _context.Surveys.FindAsync(id);
            _context.Surveys.Remove(surveys);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveysExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }
    }
}
