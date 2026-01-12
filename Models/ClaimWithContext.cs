using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Models
{
    public class ClaimWithContext
    {
        public ClaimPart Claim { get; set; } = new ClaimPart();

        public List<ClaimDetails> Details { get; set; } = new List<ClaimDetails>();

    }
}