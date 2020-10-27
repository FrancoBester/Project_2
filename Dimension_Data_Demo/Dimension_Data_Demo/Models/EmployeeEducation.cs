using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class EmployeeEducation
    {
        public EmployeeEducation()
        {
            Employee = new HashSet<Employee>();
        }

        public int EducationId { get; set; }
        public int? Education { get; set; }
        public string EducationField { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
