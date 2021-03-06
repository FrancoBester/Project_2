﻿using System;
using System.Collections.Generic;
using Dimension_Data_Demo.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Dimension_Data_Demo.Models
{
    public partial class Employee
    {
        private string connectionString = "Data Source=dimention-data-demo.cr0jdxtn9ll5.us-west-2.rds.amazonaws.com;Initial Catalog=dimention_data_demo;Persist Security Info=True;User ID=masterUsername;Password=Dd#20201023";
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
