using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class JobInformation
    {
        public JobInformation()
        {
            Employee = new HashSet<Employee>();
        }

        public int JobId { get; set; }
        public string JobRole { get; set; }
        public string Department { get; set; }
        public int? JobLevel { get; set; }
        public int? StandardHours { get; set; }
        public int? EmployeeCount { get; set; }
        public string BusinessTravel { get; set; }
        public int? StockOptionLevel { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
