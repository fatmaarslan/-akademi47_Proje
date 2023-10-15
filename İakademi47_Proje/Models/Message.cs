
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace İakademi47_Proje.Models
{
    public class Message
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int MessageID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }

        [StringLength(50)]

        public string? Content { get; set; }
    }
}
