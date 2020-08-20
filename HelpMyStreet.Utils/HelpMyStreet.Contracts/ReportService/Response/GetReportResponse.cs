using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.ReportService.Response
{
    public class GetReportResponse
    {
        public List<ReportItem> ReportItems { get; set; }
    }
}
