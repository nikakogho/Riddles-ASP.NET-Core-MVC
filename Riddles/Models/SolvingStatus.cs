using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riddles.Models
{
    [Table("UserRiddleStatus")]
    public class SolvingStatus
    {
        public int UserID { get; set; }
        public virtual User User { get; set; }


        public int RiddleID { get; set; }
        public virtual Riddle Riddle { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(15)")]
        public UserRiddleStatus Status { get; set; }
    }

    public enum UserRiddleStatus
    {
        None = 0,
        Created = 1,
        Solved = 2,
        Surrendered = 3
    }
}
