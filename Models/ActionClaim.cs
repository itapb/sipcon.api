using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    // test models
    public class ActionClaim: Action
    {
        public int ApproveQuantity { get; set; }

        public int TransactionQuantity { get; set; } 

    }
}
