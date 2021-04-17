using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Riddles.Models
{
    [Table("Users")]
    public class User : IdentityUser<int>
    {
        [InverseProperty("Author")]
        public virtual ICollection<Riddle> Created { get; set; }

        public virtual ICollection<SolvingStatus> Solved { get; set; }
    }
}
