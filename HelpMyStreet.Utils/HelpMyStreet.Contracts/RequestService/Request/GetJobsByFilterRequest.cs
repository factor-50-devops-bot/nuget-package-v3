using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreet.Contracts.RequestService.Response;

namespace HelpMyStreet.Contracts.RequestService.Request
{
    public class GetJobsByFilterRequest : IRequest<GetJobsByFilterResponse>
    { 
        /// <summary>
        /// Support activities to be returned
        /// Supply null to return all activities
        /// </summary>
        public SupportActivityRequest SupportActivities { get; set; }

        /// <summary>
        /// Base postcode for calculating distances
        /// Required
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Default support distance
        /// Supply null to return requests nationwide
        /// </summary>
        public double? DistanceInMiles { get; set; }

        /// <summary>
        /// Overrides DistanceInMiles for specific activities
        /// Supply null for an activity type to return requets nationwide
        /// </summary>
        public Dictionary<SupportActivities,double?> ActivitySpecificSupportDistancesInMiles { get; set; }

        public int? ReferringGroupID { get; set; }
        public GroupRequest Groups { get; set; }
        public JobStatusRequest JobStatuses {get; set; }
}
}
