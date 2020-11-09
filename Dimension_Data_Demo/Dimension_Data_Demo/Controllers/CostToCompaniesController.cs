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
using Microsoft.Data.SqlClient;

namespace Dimension_Data_Demo.Controllers
{
    public class CostToCompaniesController : Controller
    {
        private readonly dimention_data_demoContext _context;

        public CostToCompaniesController(dimention_data_demoContext context)
        {
            _context = context;
        }

        // GET: CostToCompanies
        public async Task<IActionResult> Index(int? id)
        {
            if(id != null)
            {
                HttpContext.Session.SetInt32("PayID", (int)id);
            }
            var backupID = HttpContext.Session.GetInt32("PayID");

            var dimention_data_demoContext = _context.CostToCompany.Where(e => e.PayId == backupID);
            return View(await dimention_data_demoContext.ToListAsync());
        }

        // GET: CostToCompanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CostToCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayId,HourlyRate,MonthlyRate,MonthlyIncome,DailyRate,OverTime,PercentSalaryHike")] CostToCompany costToCompany)
        {
            if (ModelState.IsValid)
            {
                string sOverTime = costToCompany.OverTime;
                sOverTime = sOverTime.ToUpper();
                //Ensure user enter positive numbers into database
                if (costToCompany.HourlyRate < 0 || costToCompany.MonthlyRate < 0 || costToCompany.MonthlyIncome < 0 || costToCompany.DailyRate < 0 || costToCompany.DailyRate < 0 || costToCompany.PercentSalaryHike < 0)
                {
                    ViewBag.Message = "All numbers must be possitive values";
                    return View();
                }
                else if (sOverTime != "NO" & sOverTime != "YES")//Ensures user does not enter other value than yes and no into database
                {
                    ViewBag.Message = "OverTime value must be Yes or No";
                    return View();
                }
                costToCompany.OverTime = sOverTime;

                
                int costNumber = get_set_CostId(costToCompany, "Insert");
                
                HttpContext.Session.SetInt32("newPayID", costNumber);
            }
            return RedirectToAction("Create", "EmployeePerformances");
        }

        // GET: CostToCompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costToCompany = await _context.CostToCompany.FirstOrDefaultAsync(m => m.PayId == id);
            if (costToCompany == null)
            {
                return NotFound();
            }

            return View(costToCompany);
        }

        // GET: CostToCompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var costToCompany = await _context.CostToCompany.FindAsync(id);
            if (costToCompany == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetString("oldJCostModel", JsonConvert.SerializeObject(costToCompany));//Saves model as string in session to be used to compare later
            return View(costToCompany);
        }

        // POST: CostToCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayId,HourlyRate,MonthlyRate,MonthlyIncome,DailyRate,OverTime,PercentSalaryHike")] CostToCompany costToCompany)
        {
            if (id != costToCompany.PayId)
            {
                return NotFound();
            }

            if (JsonConvert.SerializeObject(costToCompany) == HttpContext.Session.GetString("oldCostModel"))//compares old and current model to limit update query to database when models are same
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    string sOverTime = costToCompany.OverTime;
                    sOverTime = sOverTime.ToUpper();
                    //Ensure user enter positive numbers into database
                    if(costToCompany.HourlyRate < 0 || costToCompany.MonthlyRate < 0|| costToCompany.MonthlyIncome < 0 || costToCompany.DailyRate < 0 || costToCompany.DailyRate < 0 || costToCompany.PercentSalaryHike < 0)
                    {
                        ViewBag.Message = "All numbers must be possitive values";
                        return View();
                    }
                    else if (sOverTime != "NO" & sOverTime != "YES")//Ensures user does not enter other value than yes and no into database
                    {
                        ViewBag.Message = "OverTime value must be Yes or No";
                        return View();
                    }
                    costToCompany.OverTime = sOverTime;
                    try
                    {
                        _context.Update(costToCompany);//updates database
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CostToCompanyExists(costToCompany.PayId))
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

        private bool CostToCompanyExists(int id)
        {
            return _context.CostToCompany.Any(e => e.PayId == id);
        }
        public int get_set_CostId(CostToCompany costToCompany, string command_type)
        {
            int costNumber = -1;
            try
            {
                var conn = _context.Database.GetDbConnection();
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)conn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@hourRate", (int)costToCompany.HourlyRate);
                cmd.Parameters.AddWithValue("@montRate", costToCompany.MonthlyRate);
                cmd.Parameters.AddWithValue("@montIn", (int)costToCompany.MonthlyIncome);
                cmd.Parameters.AddWithValue("@dailRate", costToCompany.DailyRate);
                cmd.Parameters.AddWithValue("@Over", costToCompany.OverTime.ToString().ToUpper());
                cmd.Parameters.AddWithValue("@perSal", (int)costToCompany.PercentSalaryHike);

                if (command_type == "Select")
                {
                    cmd.CommandText = ("Select PayID from dbo.CostToCompany Where HourlyRate = @hourRate and MonthlyRate = @montRate and MonthlyIncome = @montIn and DailyRate = @dailRate and OverTime = @Over and PercentSalaryHike = @perSal");
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        costNumber = reader.GetInt32(0);
                    }
                    cmd.Dispose();
                    reader.Close();
                    conn.Close();
                }
                else if (command_type == "Insert")
                {
                    cmd.CommandText = ("Insert Into dbo.CostToCompany(PayID,HourlyRate,MonthlyRate,MonthlyIncome,DailyRate,OverTime,PercentSalaryHike) " +
                        "Values((Select count(*)+1 from dbo.CostToCompany),@hourRate,@montRate,@montIn,@dailRate,@Over,@perSal)");
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                    costNumber = get_set_CostId(costToCompany, "Select");
                }
            }
            catch (Exception ex)
            {
                string serror = ex.ToString();
                return -1;
            }
            return costNumber;
        }

    }
}
