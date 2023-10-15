using Microsoft.AspNetCore.Mvc;
using iakademi47_proje.Models;
using İakademi47_Proje.Models;

namespace İakademi47_Proje.ViewComponents
{
	public class Menus :ViewComponent

	{
		iakademi47Context context= new iakademi47Context();

		public	IViewComponentResult Invoke()
		{

			List<Category> categories=context.Categories.ToList();
			return View(categories);


		}
	}
}
