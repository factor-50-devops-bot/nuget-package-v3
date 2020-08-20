using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreet.Contracts.RequestService.Response
{
    public class PostNewRequestForHelpResponse
    {
        public int RequestID { get; set; }
        public Fulfillable Fulfillable { get; set; }
    }
}
