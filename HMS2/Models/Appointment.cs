using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HMS2.Models
{
    public class Appointment : BaseEntity
    {
        public int Id { get; set; }
        public string DoctorId { get; set; } // type string sepse primary key e IdentityUser eshte string
        public IdentityUser Doctor { get; set; }
        public DateTime DateOfAppointment { get; set; }
        public Boolean ConfirmationStatus { get; set; }
        public string AuthorId { get; set; } // type string sepse primary key e IdentityUser eshte string
        public IdentityUser Author { get; set; }
    }
}
