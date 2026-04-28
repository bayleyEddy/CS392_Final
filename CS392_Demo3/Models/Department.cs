using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS392_Demo3.Models
{
    [Table("department")]
    public class Department
    {
        [Key]
        [Column("Dnumber")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // not an IDENTITY column
        public int DNumber { get; set; }

        [Required]
        [Column("Dname", TypeName = "varchar(15)")]
        [StringLength(15)]
        public string DName { get; set; } = null!;

        [Required]
        [Column("Mgr_ssn", TypeName = "char(9)")]
        [StringLength(9)]
        public string MgrSsn { get; set; } = null!;

        [Column("Mgr_start_date", TypeName = "date")]
        public DateOnly? MgrStartDate { get; set; }
    }
}
