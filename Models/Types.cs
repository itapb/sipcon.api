using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ServiceType : Record
    {

        [Required]
        public string? Name { get; set; }

    }

    public class AssistanceType : ServiceType
    {

       

    }


    public class PossibleFault : ServiceType
    {



    }
}
