
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace İakademi47_Proje.Models
{
	public class User
	{

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int UserID { get; set; }


		[Required]
		[StringLength(50)]
		[DisplayName("Kulllanıcı Adı")]
		public string? NameSurname { get; set; }

		[Required]
		[StringLength(50)]
		[EmailAddress] //@ işareti arıcak
		public string? Email { get; set; }

		[Required]
		[StringLength(50)]
		[DataType(DataType.Password)]   //şifre oldugu için yıldızlı yazıyor
		[DisplayName("şifre")]

		public string? Password { get; set; }

		[DisplayName("Telefon")]
		public string? Telephone { get; set; }
		public string? InoviceAddress { get; set; }

        [DisplayName("Admin")]
        public bool IsAdmin { get; set; }

		[DisplayName("Aktif")]
		public bool Active { get; set; }


	}
}
