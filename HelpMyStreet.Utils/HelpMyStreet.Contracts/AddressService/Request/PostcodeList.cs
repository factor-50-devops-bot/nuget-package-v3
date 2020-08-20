using HelpMyStreet.Contracts.AddressService.Response;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelpMyStreet.Contracts.AddressService.Request
{
    public class PostcodeList
    {
        public List<string> Postcodes { get; set; }
    }
}
