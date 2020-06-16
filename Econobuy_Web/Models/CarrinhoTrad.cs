using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CarrinhoTrad
    {
        [Key]
        public int Id { get; set; }
        public int ProdID { get; set; }
        public string Nome { get; set; }
        [Required(ErrorMessage = "Adicione pelo menos uma unidade")]
        public int Qtde { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }

        public string Cat01 { get; set; }
        public string Cat02 { get; set; }
        public string Cat03 { get; set; }
        public string Descricao { get; set; }
    }
}