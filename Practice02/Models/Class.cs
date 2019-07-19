using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Practice02.Models
{
    public class Member
    {
        [MaxLength(10)]
        public string Account { get; set; }

        [Phone(ErrorMessage ="Wrong Form")]
        public string Phone { get; set; }

        public string Birthday { get; set; }
        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime TimeInsert { get; set; }

        [Required(ErrorMessage = "Update without date record")]
        public DateTime TimeLastUpdate { get; set; }
    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}

//[
//  {
//    "Account": "user123",
//    "Phone": "000123",
//    "Birthday": "2000-01-01T00:00:00",
//    "Address": "lalalala rd.",
//    "TimeInsert": "2019-07-12T00:00:00",
//    "TimeLastUpdate": "2019-07-15T00:00:00"
//  }
//]
