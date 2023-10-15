
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;





namespace İakademi47_Proje.Models
{
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int CommentID { get; set; }
        public int UserID { get; set; }
        public int MyProperty { get; set; }

        [StringLength(150)]

        public int Rewiew { get; set; }


    }
}
