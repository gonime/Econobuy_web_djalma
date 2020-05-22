using Econobuy_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econobuy_Web.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult LoginCliente(tb_cliente user)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var userDetail = db.tb_cliente.Where(x => x.cli_st_user == user.cli_st_user && x.cli_st_senha == user.cli_st_senha).FirstOrDefault();
                if (userDetail == null)
                {
                    TempData["Erro"] = "Usuário ou senha inválidos";
                    return View("Index", user);
                }
                else
                {
                    Session["clienteID"] = user.cli_in_codigo;
                    Session["clienteNome"] = user.cli_st_nome;
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CadastraCliente(CadastroCliente cad)
        {
            var end = new tb_endereco
            {
                end_st_bairro = cad.Bairro,
                end_st_CEP = cad.CEP,
                end_st_cidade = cad.Cidade,
                end_st_compl = cad.Complemento,
                end_st_log = cad.Logradouro,
                end_st_num = cad.Numero,
                end_st_uf = cad.UF,
                end_st_tel1 = cad.Telefone_1,
                end_st_tel2 = cad.Telefone_2
            };
            var cli = new tb_cliente
            {
                cli_st_CPF = cad.CPF,
                cli_st_email = cad.email,
                cli_st_nome = cad.Nome,
                cli_st_senha = cad.Senha,
                cli_st_user = cad.User
            };
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("Cadastro", cad);
                }
                else
                {
                    db.tb_endereco.Add(end);
                    cli.end_in_codigo = end.end_in_codigo;
                    db.tb_cliente.Add(cli);
                    db.SaveChanges();
                    Session["clienteID"] = cli.cli_in_codigo;
                    Session["clienteNome"] = cli.cli_st_nome;
                    return RedirectToAction("Home","Cliente");
                }
            }
        }

        public ActionResult Home()
        {
            return View();
        }
    }
}