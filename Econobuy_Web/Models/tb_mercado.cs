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
    using System.ComponentModel.DataAnnotations;

    public partial class tb_mercado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_mercado()
        {
            this.tb_avaliacao_mercado = new HashSet<tb_avaliacao_mercado>();
            this.tb_mercado_img = new HashSet<tb_mercado_img>();
            this.tb_pedido = new HashSet<tb_pedido>();
            this.tb_produto = new HashSet<tb_produto>();
        }
    
        public int mer_in_codigo { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Este campo � obrigat�rio")]
        public string mer_st_nome { get; set; }
        [Display(Name = "Usu�rio")]
        [Required(ErrorMessage = "Este campo � obrigat�rio")]
        public string mer_st_user { get; set; }
        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Este campo � obrigat�rio")]
        public string mer_st_senha { get; set; }
        [Display(Name = "CPNJ")]
        [Required(ErrorMessage = "Este campo � obrigat�rio")]
        public string mer_st_CPNJ { get; set; }
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Este campo � obrigat�rio")]
        public string mer_st_email { get; set; }
        public int end_in_codigo { get; set; }
        public bool mer_bit_active { get; set; }
        public bool mer_bit_advert { get; set; }
        public bool cli_bit_conf_email { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_avaliacao_mercado> tb_avaliacao_mercado { get; set; }
        public virtual tb_endereco tb_endereco { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_mercado_img> tb_mercado_img { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_pedido> tb_pedido { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_produto> tb_produto { get; set; }
    }
}
