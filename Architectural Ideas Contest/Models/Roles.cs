using Microsoft.AspNetCore.Mvc;

namespace Architectural_Ideas_Contest.Models
{
    public class Roles : Controller
    {
        public const string Admin = "Admin";
        public const string Judge = "Judge";
        public const string Candidate = "Candidate";
    }
}
