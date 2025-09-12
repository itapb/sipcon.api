using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class AdjustmentWithContext
    {
        public Adjustment Adjustment { get; set; } = new Adjustment();

        public List<AdjustmentDetails> Details { get; set; } = new List<AdjustmentDetails>();

    }
}