using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.admin.Controllers
{
    public class PagesController : Controller
    {
        //Создаем метод отображения главной страницы
        // GET: admin/Pages
        public ActionResult Index()
        {
            //Объявляем список для представления (PageVM)
            List<PageVM> pageList;

            //Инициализируем список (DB)
            using(Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Возвращаем список в представление
            return View(pageList);
        }

        //Добавляем метод создания страниц
        // GET: admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        //Добавляем метод создания страниц
        // POST: admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Объявляем переменную для краткого описания (Slug)
                string slug;

                //Инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                //Присваиваем заголовок модели
                dto.Title = model.Title.ToUpper();

                //Проверяем, есть ли краткое описание, если нет, присваиваем его
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else 
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Убеждаемся в уникальности заголовка и описания
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title allready exist.");
                    return View(model);
                }
                else if(db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug allready exist.");
                    return View(model);
                }
                //Присваиваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Сохранем модель в базу данных
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Передаём сообщение через TempData
            TempData["SM"] = "You have added a new page!";

            //Переадресовываем пользователя на метод INDEX
            return RedirectToAction("Index");
        }

        //Добавляем метод редактирования страниц
        // GET: admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаем страницу (данные страницы)
                PagesDTO dto = db.Pages.Find(id);

                //Проверяем доступна ли страница
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Инициализируем модель данными из бд
                model = new PageVM(dto);
            }

            //Возвращаем модель в представление
            return View(model);
        }

        //Добавляем метод редактирования страниц
        // POST: admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверить модель на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //Получаем Id страницы
                int id = model.Id;

                //Объявим переменную краткого заголовка (slug)
                string slug = "home";

                //Получаем страницу по "Id"
                PagesDTO dto = db.Pages.Find(id);

                //Присваиваем название из полученной модели в DTO
                dto.Title = model.Title;

                //Проверяем, есть ли краткое описание, если нет - присваиваем его 
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //Проверяем заголовок (Title) и краткое описание (Slug) на уникальность
                if (db.Pages.Where(x=> x.Id != id).Any(x => x.Title == model.Title) )
                {
                    ModelState.AddModelError("", "Thit title already exist!");
                    return View(model);
                }
                else if(db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Thit slug already exist!");
                    return View(model);
                }

                //Записываем оставшиеся данные в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Сохранить изменения в бд
                db.SaveChanges();
            }

            //Устанавливаем сообщение в TempData
            TempData["SM"] = "You have edited the page.";

            //Переадресация пользователя на исходную страницу
            return RedirectToAction("EditPage");
        }

        //Создаем метод страницы деталей
        // GET: admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            //Объявляем модель PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получить страницу
                PagesDTO dto = db.Pages.Find(id);

                //Убедить что страница доступна
                if (dto == null)
                {
                    return Content("The page does not exist!");
                }

                //Присваиваем информацию из базы данный текущей страницы модели
                model = new PageVM(dto);
            }
            //Возвращаем модель в представление
            return View(model);
        }

        //Создаём метод удаления страниц
        // GET: admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Получаем страницу
                PagesDTO dto = db.Pages.Find(id);

                //Удаляем страницу
                db.Pages.Remove(dto);

                //Сохраняем изменения в бд
                db.SaveChanges();
            }

            //Добавить сообщение об успешном удалении
            TempData["SM"] = "The page deleted successfully!";

            //Переадресовываем пользователя на страницу INDEX(начальную)
            return RedirectToAction("Index");
        }

        //Создаем метод сортировки
        // Post: admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                //Реализуем начальный счетчик
                int count = 1;

                //Инициализируем можель данных
                PagesDTO dto;
                //Устанавливаем сортировку для каждой страницы
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    
                    count++;
                }
            }
        }

        // GET: admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Объявляем модель
            SidebarVM model;

            using (Db db = new Db())
            {
                //Получаем данные из DTO
                SidebarDTO dto = db.Sidebars.Find(1);                   //говнакодинг, потом исправлю!!! (1 - это id)

                //Заполняем модель данными
                model = new SidebarVM(dto);
            }

            //Вернуть представление с моделью
            return View(model);
        }

        // Post: admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //Получить данные из бд(DTO)
                SidebarDTO dto = db.Sidebars.Find(1);                   //Говнакодинг!!!! Исправлю!

                //Присвоить данные в тело (в свойство Body)
                dto.Body = model.Body;

                //Сохранить изменения
                db.SaveChanges();
            }

            //Выводим сообщение с помощью TempData
            TempData["SM"] = "You have edited the sidebar!";

            //Переадресация пользователя
            return RedirectToAction("EditSidebar");
        }
    }
}