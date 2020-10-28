using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class Employee
    {
        public int EmployeeNumber { get; set; }
        public int? JobId { get; set; }
        public int? DetailsId { get; set; }
        public int? PayId { get; set; }
        public int? EducationId { get; set; }
        public int? SurveyId { get; set; }
        public int? HistoryId { get; set; }
        public int? PerformanceId { get; set; }

        public virtual EmployeeDetails Details { get; set; }
        public virtual EmployeeEducation Education { get; set; }
        public virtual EmployeeHistory History { get; set; }
        public virtual JobInformation Job { get; set; }
        public virtual CostToCompany Pay { get; set; }
        public virtual EmployeePerformance Performance { get; set; }
        public virtual Surveys Survey { get; set; }
    }
}
