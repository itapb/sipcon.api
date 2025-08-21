using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MovementWithContext
    {
        public Movement Movement { get; set; } = new Movement();

        public List<MovementDetails> Details { get; set; } = new List<MovementDetails>();
    }
}
