using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_6.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherID { get; set; }

        [Required(ErrorMessage = "Прізвище є обов'язковим")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "По батькові")]
        public string? MiddleName { get; set; }

        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Display(Name = "Місце роботи")]
        public string? Workplace { get; set; }

        [Required(ErrorMessage = "Оберіть посаду")]
        [Display(Name = "Посада")]
        public int PositionID { get; set; }

        [Display(Name = "Домашня адреса")]
        public string? HomeAddress { get; set; }

        [Display(Name = "Характеристика")]
        public string? Characteristic { get; set; }

        [ForeignKey("PositionID")]
        public virtual Position? Position { get; set; }

        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    }
}