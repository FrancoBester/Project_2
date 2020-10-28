using System;
using System.Collections.Generic;

namespace Dimension_Data_Demo.Models
{
    public partial class Surveys
    {
        public Surveys()
        {
            Employee = new HashSet<Employee>();
        }

        public int SurveyId { get; set; }
        public int? EnvironmentSatisfaction { get; set; }
        public int? JobSatisfaction { get; set; }
        public int? RelationshipSatisfaction { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
