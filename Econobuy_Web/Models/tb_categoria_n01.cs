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

    public partial class tb_categoria_n01
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tb_categoria_n01()
        {
            this.tb_categoria_n02 = new HashSet<tb_categoria_n02>();
            this.tb_produto = new HashSet<tb_produto>();
        }
    
        public int cat01_in_codigo { get; set; }
        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "Este Campo � obrigat�rio")]
        public string cat01_st_nome { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_categoria_n02> tb_categoria_n02 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tb_produto> tb_produto { get; set; }
    }
}
