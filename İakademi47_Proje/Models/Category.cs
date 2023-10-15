
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace İakademi47_Proje.Models
{
	public class Category
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]


		[DisplayName("ID")]
		public int CategoryID { get; set; }



		[DisplayName("Üst Kategori Adı")]
		public int ParentID { get; set; } //ders2 dakika 2.2.10saniye50



		[DisplayName("Kategori Adı")]
		[Required(ErrorMessage ="Kategori Adı Zorunlu")]
		[StringLength(50,ErrorMessage ="En fazla 50 Karakter Girilebilir")]
		public string? CategoryName { get; set; }


		[DisplayName("Aktif")]
		public bool Active { get; set; }
	}
}
