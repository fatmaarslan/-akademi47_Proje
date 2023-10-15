
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace İakademi47_Proje.Models
{
	public class Order
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderID { get; set; }
		public DateTime OrderDate { get; set; }

		[StringLength(30)]
		public string? OrderGroupGUID { get; set; }//ortak numarayla mesela 5 tane degısıık sıparıs tek yerden alırsa bu cok anlamadım ınternetten bak

		public int UserID { get; set; }
		public int ProductID { get; set; }
		public int Quantity { get; set; }

	}
}
