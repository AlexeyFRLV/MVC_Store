using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: admin/Shop
        public ActionResult Categories()
        {
            //Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                //Инициализируем модель данными
                categoryVMList = db.Categories                                         //Выборка всех категорий
                                   .ToArray()
                                   .OrderBy(x => x.Sorting)
                                   .Select(x => new CategoryVM(x))
                                   .ToList();
            }
            //Возвращаем List в представление
            return View(categoryVMList);
        }
    }
}