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

        // GET: EmployeeDetails/Create
        public IActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "Gender1");
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalStatus1");
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
                if (employeeDetails.Age < 1 || employeeDetails.DistanceFromHome < 0)
                {
                    ViewBag.Message = "Age and Distance from home must be positive number values";
                    return View();
                }
                else if (employeeDetails.Attrition.ToUpper() != "YES" & employeeDetails.Attrition.ToUpper() != "NO")
                {
                    ViewBag.Message = "Attrition field can only be Yes or No";
                    return View();
                }
                else if ((employeeDetails.Over18[0].ToString()).ToUpper() != "Y" & (employeeDetails.Over18[0].ToString()).ToUpper() != "N")
                {
                    ViewBag.Message = "Over 18 value can only be a Y or an N";
                    return View();
                }

                try
                {
                    int detailNumber = get_set_DetailsId(employeeDetails, "Insert");
                    
                    HttpContext.Session.SetInt32("newDetailsID", detailNumber);
                }
                catch(Exception)
                {
                    ViewBag.Message = "There was an error updating the information. Please try again later";
                    return View();
                }

            }
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "GenderId", employeeDetails.GenderId);
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalId", employeeDetails.MaritalId);

            return RedirectToAction("Create", "EmployeeEducations");
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
                    if(employeeDetails.Age < 1 || employeeDetails.DistanceFromHome < 0)
                    {
                        ViewBag.Message = "Age and Distance from home must be positive number values";
                        return View();
                    }
                    else if(employeeDetails.Attrition.ToUpper() != "YES" & employeeDetails.Attrition.ToUpper() != "NO")
                    {
                        ViewBag.Message = "Attrition field can only be Yes or No";
                        return View();
                    }
                    else if((employeeDetails.Over18[0].ToString()).ToUpper() != "Y" & (employeeDetails.Over18[0].ToString()).ToUpper() != "N")
                    {
                        ViewBag.Message = "Over 18 value can only be a Y or an N";
                        return View();
                    }
                    employeeDetails.Attrition = employeeDetails.Attrition.ToUpper();
                    employeeDetails.Over18 = (employeeDetails.Over18[0].ToString()).ToUpper();
                    try
                    {
                        _context.Update(employeeDetails);
                        await _context.SaveChangesAsync();
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
                    return RedirectToAction("Index", "Employees");
                }
            }
            
            ViewData["GenderId"] = new SelectList(_context.Gender, "GenderId", "GenderId", employeeDetails.GenderId);
            ViewData["MaritalId"] = new SelectList(_context.MaritalStatus, "MaritalId", "MaritalId", employeeDetails.MaritalId);
            return View(employeeDetails);
        }

        private bool EmployeeDetailsExists(int id)
        {
            return _context.EmployeeDetails.Any(e => e.DetailsId == id);
        }

        public int get_set_DetailsId(EmployeeDetails employeeDetails, string command_type)
        {
            int detailNumber = -1;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Age", (int)employeeDetails.Age);
                cmd.Parameters.AddWithValue("@Attrition", employeeDetails.Attrition);
                cmd.Parameters.AddWithValue("@Distance", (int)employeeDetails.DistanceFromHome);
                cmd.Parameters.AddWithValue("@Over", employeeDetails.Over18);
                cmd.Parameters.AddWithValue("@Marital", (int)employeeDetails.MaritalId);
                cmd.Parameters.AddWithValue("@Gender", (int)employeeDetails.GenderId);

                if (command_type == "Select")
                {
                    cmd.CommandText = ("Select DetailsID from dbo.employeeDetails Where Age = @Age and Attrition = @Attrition and DistanceFromHome = @Distance and Over18 = @Over and MaritalID = @Marital and GenderID = @Gender");
                    SqlDataReader reader =  cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        detailNumber = reader.GetInt32(0);
                    }
                    cmd.Dispose();
                    reader.Close();
                    conn.Close();
                }
                else if (command_type == "Insert")
                {
                    cmd.CommandText = ("Insert Into dbo.EmployeeDetails(DetailsID,Age,Attrition,DistanceFromHome,Over18,MaritalID,GenderID) " +
                        "Values((Select count(*)+1 from dbo.EmployeeDetails),@Age,@Attrition,@Distance,@Over,@Marital,@Gender)");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                    detailNumber = get_set_DetailsId(employeeDetails, "Select");
                }
            }
            catch (Exception ex)
            {
                string serror = ex.ToString();
                return -1;
            }
            return detailNumber;
        }
    }
}
