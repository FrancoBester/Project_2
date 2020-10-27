using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class MaritalStatus
    {
        public MaritalStatus()
        {
            EmployeeDetails = new HashSet<EmployeeDetails>();
        }

        public int MaritalId { get; set; }
        public string MaritalStatus1 { get; set; }

        public virtual ICollection<EmployeeDetails> EmployeeDetails { get; set; }
    }
}
