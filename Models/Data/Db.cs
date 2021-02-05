using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_Store.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PagesDTO> Pages { get; set; }      //связь между сущностью и базой данных

        public DbSet<SidebarDTO> Sidebars { get; set; }

        public DbSet<CategoryDTO> Categories { get; set; }          //Подключаем таблицы Categories
    }
}