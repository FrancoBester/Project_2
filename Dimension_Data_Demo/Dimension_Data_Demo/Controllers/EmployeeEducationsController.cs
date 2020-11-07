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
            if (id != null)
            {
                int EduactionID = -1;
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", (int)id);
                cmd.CommandText = ("Select EducationID from dbo.Employee Where EmployeeNumber = @Id");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EduactionID = reader.GetInt32(0);
                }
                conn.Close();
                cmd.Dispose();
                reader.Close();
                HttpContext.Session.SetInt32("edu_employeeNumber", (int)id);
                HttpContext.Session.SetInt32("EducationId", (int)EduactionID);
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
            List<SelectListItem> EducationFieldlist = new List<SelectListItem>();
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = (SqlConnection)conn;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = ("Select distinct(EducationField) from dbo.EmployeeEducation");
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                EducationFieldlist.Add(new SelectListItem() { Text = reader.GetValue(0).ToString()});
            }
            conn.Close();
            cmd.Dispose();
            reader.Close();
            ViewData["fieldData"] = EducationFieldlist;

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
            var testing = employeeEducation.EducationField;
            if (id != employeeEducation.EducationId)
            {
                return NotFound();
            }

            if(JsonConvert.SerializeObject(employeeEducation) == HttpContext.Session.GetString("oldEducationModel"))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        int educationId = -1;
                        var conn = _context.Database.GetDbConnection();
                        conn.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = (SqlConnection)conn;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@Level",(int)employeeEducation.Education);
                        cmd.Parameters.AddWithValue("@Field", employeeEducation.EducationField);
                        cmd.CommandText = "Select EducationID from dbo.EmployeeEducation Where Education=@Level and EducationField = @Field";
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            educationId = int.Parse(reader.GetValue(0).ToString());
                        }
                        await cmd.DisposeAsync();
                        await reader.CloseAsync();

                        cmd = new SqlCommand();
                        cmd.Connection = (SqlConnection)conn;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("edu_employeeNumber"));
                        cmd.Parameters.AddWithValue("@EduNumber", educationId);
                        cmd.CommandText = "Update dbo.Employee Set EducationID = @EduNumber Where EmployeeNumber=@EmpNumber";
                        await cmd.ExecuteNonQueryAsync();

                        cmd.Dispose();
                        conn.Close();

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
                //return View(employeeEducation);

            }
            
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
