using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Imię")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required]
        public string LastName { get; set; }

        [Display(Name = "Data urodzenia")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}