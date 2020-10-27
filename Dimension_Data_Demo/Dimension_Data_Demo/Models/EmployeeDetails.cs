using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class EmployeeDetails
    {
        public EmployeeDetails()
        {
            Employee = new HashSet<Employee>();
        }

        public int DetailsId { get; set; }
        public int? Age { get; set; }
        public string Attrition { get; set; }
        public int? DistanceFromHome { get; set; }
        public string Over18 { get; set; }
        public int? MaritalId { get; set; }
        public int? GenderId { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual MaritalStatus Marital { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
    }
}
