﻿using System;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

namespace Dimension_Data_Demo.Controllers
{
    [Authorize]
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
                try
                {
                    //Is used to filter data to only show single persons performance data from database
                    int PerformanceID = ((int)_context.Employee.Where(e => e.EmployeeNumber == id).Select(e => e.PerformanceId).First());

                    HttpContext.Session.SetInt32("per_employeeNumber", (int)id);//saves id in session to be used when the user returns to the page an does not use the main navigation page
                    HttpContext.Session.SetInt32("PerformanceId", PerformanceID);//saves id in session to be used when the user returns to the page an does not use the main navigation page
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was a problem retrieving the data. Please try later";
                    return View();
                }
            }

            var backupID = HttpContext.Session.GetInt32("PerformanceId");
            var dimention_data_demoContext = _context.EmployeePerformance.Where(e => e.PerformanceId == backupID);//filter view to only show one employees performance data
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
            if (employeePerformance.PerformanceRating <= -1 || employeePerformance.WorkLifeBalance <= -1 || employeePerformance.JobInvolvement <= -1)
            {
                ViewBag.Message = "All numbers must be possitive values";
                return View();
            }

            try
            {
                int performance_ID = (int)_context.EmployeePerformance.Where(e => e.PerformanceRating == employeePerformance.PerformanceRating && e.WorkLifeBalance == employeePerformance.WorkLifeBalance && 
                e.JobInvolvement == employeePerformance.JobInvolvement).Select(e => e.PerformanceId).First();
                if(performance_ID == 0)
                {
                    int new_performance_ID = ((int)_context.EmployeePerformance.OrderByDescending(e => e.PerformanceId).Select(e => e.PerformanceId).First()) + 1;//gets the id of the new record that will be added into the table
                    employeePerformance.PerformanceId = new_performance_ID;//assignes new it to model
                    _context.Add(employeePerformance);//addes id to model that will be added to database
                    await _context.SaveChangesAsync();//addes the new models info into the database

                    HttpContext.Session.SetInt32("newPerformanceID", new_performance_ID);//addes id to session to be used later when adding user into the main table in the database
                }
                else
                {
                    HttpContext.Session.SetInt32("newPerformanceID", performance_ID);//addes id to session to be used later when adding user into the main table in the database
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "There was an error updating the information. Please try again later";
                return View();
            }
            
           return RedirectToAction("Create", "JobInformations");
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
                    if (employeePerformance.PerformanceRating <= -1 || employeePerformance.WorkLifeBalance <= -1 || employeePerformance.JobInvolvement <= -1)
                    {
                        ViewBag.Message = "All numbers must be possitive values";
                        return View();
                    }

                    try
                    {
                        try
                        {
                            int performanceId = get_set_PerformanceId(employeePerformance, "Select");
                            if(performanceId == -1)
                            {
                                performanceId = get_set_PerformanceId(employeePerformance, "Insert");
                            }
                            var conn = _context.Database.GetDbConnection();
                            await conn.OpenAsync();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = (SqlConnection)conn;
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Parameters.AddWithValue("@EmpNumber", HttpContext.Session.GetInt32("per_employeeNumber"));
                            cmd.Parameters.AddWithValue("@PerNumber", performanceId);
                            cmd.CommandText = "Update dbo.Employee Set PerformanceID = @PerNumber Where EmployeeNumber=@EmpNumber";

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
                        if (!EmployeePerformanceExists(employeePerformance.PerformanceId))
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

        private bool EmployeePerformanceExists(int id)
        {
            return _context.EmployeePerformance.Any(e => e.PerformanceId == id);
        }

        public int get_set_PerformanceId(EmployeePerformance employeePerformance,string command_type)
        {
            int performanceId = -1;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@PerRating", (int)employeePerformance.PerformanceRating);
                cmd.Parameters.AddWithValue("@WorkBal", (int)employeePerformance.WorkLifeBalance);
                cmd.Parameters.AddWithValue("@JobInv", (int)employeePerformance.JobInvolvement);
                if (command_type == "Select")
                {
                    cmd.CommandText = ("Select PerformanceID from dbo.EmployeePerformance Where PerformanceRating = @PerRating and WorkLifeBalance = @WorkBal and JobInvolvement = @JobInv");
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        performanceId = reader.GetInt32(0);
                    }
                    cmd.Dispose();
                    reader.Close();
                    conn.Close();
                }
                else if (command_type == "Insert")
                {
                    cmd.CommandText = ("Insert Into dbo.EmployeePerformance(PerformanceID,PerformanceRating,WorkLifeBalance,JobInvolvement) Values((Select count(*)+1 from dbo.EmployeePerformance),@PerRating,@WorkBal,@JobInv)");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                    performanceId = get_set_PerformanceId(employeePerformance, "Select");
                }
            }
            catch(Exception)
            {
                return -1;
            }
            return performanceId;
        }
    }
}
