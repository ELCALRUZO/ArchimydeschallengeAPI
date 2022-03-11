using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchimydeschallengeAPI.Models
{
    
        [ApiExplorerSettings(IgnoreApi = true)]
        public class UserSignUpApiModel
        {
            [Required]
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }

            public int RoleID { get; set; }

            public int UserID { get; set; }
            public string CompanyName { get; set; }
            public DateTime DateCreated { get; set; }
 
        }
     

     

}
