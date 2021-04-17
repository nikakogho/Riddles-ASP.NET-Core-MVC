using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Riddles.Models
{
    [Table("Riddles")]
    public class Riddle
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public string ComplimentForWinning { get; set; }

        [Required]
        public string InsultForLosing { get; set; }

        [ForeignKey("Author")]
        public int AuthorID { get; set; }
        [Required]
        public virtual User Author { get; set; }

        public virtual ICollection<SolvingStatus> Solvers { get; set; }
    }
}
