using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class EmployeePerformance
    {
        public EmployeePerformance()
        {
            Employee = new HashSet<Employee>();
        }

        public int PerformanceId { get; set; }
        public int? PerformanceRating { get; set; }
        public int? WorkLifeBalance { get; set; }
        public int? JobInvolvement { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
