using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Econobuy_Web.Models
{
    public class CadastroCliente
    {
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Usuário")]
        public string User { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
        [Display(Name = "CPF")]
        
        public string CPF { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "E-mail")]
        public string email { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "CEP")]
        public string CEP { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Número")]
        public string Numero { get; set; }
        [Display(Name = "Complemento")]
        public string Complemento { get; set; }
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "UF")]
        public string UF { get; set; }
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [Display(Name = "Telefone 1")]
        public string Telefone_1 { get; set; }
        [Display(Name = "Telefone 2")]
        public string Telefone_2 { get; set; }
    }
}