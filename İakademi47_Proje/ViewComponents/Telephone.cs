using İakademi47_Proje.Models;
using Microsoft.AspNetCore.Mvc;

namespace İakademi47_Proje.ViewComponents
{
	public class Telephone:ViewComponent
	{

		iakademi47Context context = new iakademi47Context();

		public string Invoke()
		{
			string? telephone = context.Settings.FirstOrDefault(s => s.SettingID == 1).Telephone;
			return $"{telephone}";
		}


	}
}
