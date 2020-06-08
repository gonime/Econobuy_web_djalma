using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class Carrinho
    {
        [Key]
        public int Id { get; set; }
        public int ProdID { get; set; }
        public string ProdNome { get; set; }
        public int Qtde { get; set; }
        public decimal ValorUnidade { get; set; }
        public decimal ValorTotal { get; set; }
    }
}