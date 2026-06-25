using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FIGO_Report
    {
        public int Id { get; set; }
        public string? NameReport { get; set; }
        public int AccessGroupId { get; set; }
        public Boolean IsPdfReport { get; set;  }
    }

    public class FIGO_Filters
    {
        public int Id { get; set; }
        public string? Field { get; set; }
        public string? FieldType { get; set; }
        public string? ActionType { get; set; }
        public int ReportFigoId { get; set; }
    }

    public class FIGO_Query
    {
        public int Id { get; set; }
        public string? Query { get; set; }
    }

    public class FIGO_Options
    {
        public int ReportFigoId { get; set; }
        public int FilterReportId { get; set; }
        public int FilterOptionId { get; set; }
        public string? Name {  get; set; }
        public string? Value { get; set; }
    }
}
