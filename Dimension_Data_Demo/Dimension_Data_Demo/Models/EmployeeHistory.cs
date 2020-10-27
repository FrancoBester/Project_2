using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class EmployeeHistory
    {
        public EmployeeHistory()
        {
            Employee = new HashSet<Employee>();
        }

        public int HistoryId { get; set; }
        public int? NumCompaniesWorked { get; set; }
        public int? TotalWorkingYears { get; set; }
        public int? YearsAtCompany { get; set; }
        public int? YearsInCurrentRole { get; set; }
        public int? YearsSinceLastPromotion { get; set; }
        public int? YearsWithCurrManager { get; set; }
        public int? TrainingTimesLastYear { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
