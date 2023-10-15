using Microsoft.EntityFrameworkCore;
using XAct;

namespace İakademi47_Proje.Models
{
    public class Cls_Supplier
    {
        iakademi47Context context= new iakademi47Context();
        public async Task<List<Supplier>> SupplierSelect()
        {


            List<Supplier> suppliers = await context.Suppliers.ToListAsync(); 
            return suppliers;

        }

        public static string SupplierInsert(Supplier supplier)
        {

            //metod static oldugu icin burda contexti görmicek bundan dolayı using ile getiriyorum contexti

            using (iakademi47Context context = new iakademi47Context())
            {

                try
                {

                    Supplier sup = context.Suppliers.FirstOrDefault(c => c.BrandName.ToLower() ==
                    supplier.BrandName.ToLower());

                    if (sup == null)
                    {
                        context.Add(supplier);
                        context.SaveChanges();
                        return " başarılı";
                    }
                    else
                    {
                        return " Zaten Var";
                    }


                }
                catch (Exception)
                {
                    return "Kaydetme Başarısız ";

                }

            }

        }


        public async Task<Supplier?> SupplierDetails(int? id)
        {


            Supplier? supplier = await context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);
            return supplier;

        }



        public static bool SupplierUpdate(Supplier supplier)
        {

            // metod statik oldugu için contexti görmüyor bundan dolayı using kullanarak getirdim

            using (iakademi47Context context = new iakademi47Context())

            {

                try
                {
                    context.Update(supplier);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }

		public static bool SupplierDelete(int id)
		{

			try
			{
				using (iakademi47Context context = new iakademi47Context())
				{

					Supplier supplier = context.Suppliers.FirstOrDefault(s => s.SupplierID == id);
					supplier.Active = false;



					context.SaveChanges();
					return true;
				}
			}
			catch (Exception)
			{

				return false;
			}

		}



	}
}

  
