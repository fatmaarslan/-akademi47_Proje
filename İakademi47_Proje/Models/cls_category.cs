using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace İakademi47_Proje.Models
{

	public class cls_category
	{

		iakademi47Context context = new iakademi47Context();

		public async Task<List<Category>> CategorySelect()
		{


			List<Category> categories = await context.Categories.ToListAsync();
			return categories;

		}

		public List<Category> CategorySelectMain()
		{


			List<Category> categories = context.Categories.Where(c => c.ParentID == 0).ToList();
			return categories;

		}


		public static string CategoryInsert(Category category)
		{

			//metod static oldugu icin burda contexti görmicek bundan dolayı using ile getiriyorum contexti

			using (iakademi47Context context = new iakademi47Context())
			{

				try
				{

					Category cat = context.Categories.FirstOrDefault(c => c.CategoryName.ToLower() ==
					category.CategoryName.ToLower());

					if (cat == null)
					{
						context.Add(category);
						context.SaveChanges();
						return "Kaydetme Başarılı";
					}
					else
					{
						return "Bu Kategori Zaten Var";
					}


				}
				catch (Exception)
				{
					return "Kaydetme Başarısız ";

				}

			}

		}


		public async Task<Category> CategoryDetails(int? id)
		{
			//gelen id ye ait category bilgisi
			//select * from Categories where CategoryID=id
			//entity framework core sorgusu
			//yada FindAsync de diyebilirim
			//yani sorgum söyle de olabilirdi;
			//Category category =await context.Categories.FindAsync(id);
			//yani bir kaydın bütün kolonları ;
			//Category category =sorgu
			//direkt bütün kayıtlar için 
			//List<Category>categories=sorgu
			//BU SORGULAR ÇOK ÖNEMLİ
			//Category category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);--sadece kayıt 
			//int ParentID=context.Categories.FirstorDefault(c=>c.CategoryID==id) => Tek kaydın ID si
			//string CategoryName=context.Categories.FirstorDefault(c=>c.CategoryID==id).CategoryName



			Category category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);
			return category;

		}


		public static bool CategoryUpdate(Category category)
		{

			// metod statik oldugu için contexti görmüyor bundan dolayı using kullanarak getirdim

		  using (iakademi47Context context=new iakademi47Context()) 
			
			{

				try
				{
					context.Update(category);
					context.SaveChanges();
					return true;	
				}
				catch (Exception)
				{

					return false;				}			
			}
		}


		
		public static bool CategoryDelete(int id) 
		{

			try
			{
				using (iakademi47Context context = new iakademi47Context())
				{

					Category category = context.Categories.FirstOrDefault(c=>c.CategoryID == id);
					category.Active= false;

					List<Category> categories = context.Categories.Where(c=>c.ParentID == id).ToList();
					//silinecek olan katregoriye ait alt kategori varsa onları da pasif yapıyoruz

					foreach (var item in categories)
					{
						item.Active = false;
					}

					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{

				return false;			}
		
		}




    }
}
