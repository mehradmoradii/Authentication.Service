using Auth.Infrastracture.Messages.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Dtos.AppUsers
{
    public class AppUsersSimpleDto 
    {
        public Guid Id { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? NationalId { get; set; }
        
       

    }
}
