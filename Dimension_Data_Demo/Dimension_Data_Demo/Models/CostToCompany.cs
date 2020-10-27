using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class CostToCompany
    {
        public CostToCompany()
        {
            Employee = new HashSet<Employee>();
        }

        public int PayId { get; set; }
        public int? HourlyRate { get; set; }
        public int? MonthlyRate { get; set; }
        public int? MonthlyIncome { get; set; }
        public int? DailyRate { get; set; }
        public string OverTime { get; set; }
        public int? PercentSalaryHike { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
