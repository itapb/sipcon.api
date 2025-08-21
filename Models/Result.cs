using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Result
    {
        public int Value {  get; set; }  
        public int InsertedRows { get; set; }
        public int UpdatedRows { get; set; }
        public int LastId { get; set; }

    }
}
