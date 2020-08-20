using HelpMyStreet.Contracts.GroupService.Request;
using System.Collections.Generic;

namespace HelpMyStreet.Contracts.GroupService.Response
{
    public class GetLandingPageContentResponse
    {
        public bool IsLoggedIn { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZoomLevel { get; set; }
        public string SignUpLink { get; set; }
        public string CommunityName { get; set; }
        public string CommunityShortName { get; set; }
        public string BannerImageLocation { get; set; }
        public string Header { get; set; }
        public string HeaderHelpButtonText { get; set; }
        public string HeaderVolunteerButtonText { get; set; }
        public string HeaderHTML { get; set; }
        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersTextHtml { get; set; }
        public bool ShowRequestHelp { get; set; }
        public bool ShowHelpExampleCards { get; set; }
        public bool DisableButtons { get; set; }
        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }
        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText { get; set; }
        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public IEnumerable<string> UsefulLinksHtml { get; set; }
    }
}
