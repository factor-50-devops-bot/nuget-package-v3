using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.ReportService.Response
{
    public class ReportItem
    {
        public string Section { get; set; }
        public int Last2Hours { get; set; }
        public int Today { get; set; }
        public int SinceLaunch { get; set; }
    }
}
