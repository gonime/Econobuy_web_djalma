using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class TempItemDBContext : DbContext
    {
        public TempItemDBContext()
            : base("Name=TempItemDBContext")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TempItemDBContext>());
        }
        public DbSet<Carrinho> Carrinhos { get; set; }
    }
}