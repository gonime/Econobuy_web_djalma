//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econobuy_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_item
    {
        public int item_in_codigo { get; set; }
        public int prod_in_codigo { get; set; }
        public int ped_in_codigo { get; set; }
        public int item_in_qtde { get; set; }
        public decimal item_dec_valor { get; set; }
    
        public virtual tb_pedido tb_pedido { get; set; }
        public virtual tb_produto tb_produto { get; set; }
    }
}
