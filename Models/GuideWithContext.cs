using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GuideWithContext
    {
        public Guide Guide { get; set; } = new Guide();
        public List< GuideDetails> GuideDetails { get; set; } = new List<GuideDetails>();

    }
}
