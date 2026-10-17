using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_6.Models
{
    [Table("TeacherSubjects")]
    public class TeacherSubject
    {
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }

        [Display(Name = "Прочитані години")]
        public int HoursRead { get; set; }

        [ForeignKey("TeacherID")]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey("SubjectID")]
        public virtual Subject? Subject { get; set; }
    }
}