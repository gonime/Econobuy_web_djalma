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
                    Session["clienteID"] = userDetail.cli_in_codigo;
                    Session["clienteNome"] = userDetail.cli_st_nome;
                    return RedirectToAction("Home", "Cliente");
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
                cli_st_user = cad.User,
                cli_bit_active = true,
                cli_bit_advert = false,
                cli_bit_conf_email = false
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

        public ActionResult ConsultarPedidos()
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from ped in db.tb_pedido
                             join mer in db.tb_mercado on
                             ped.mer_in_codigo equals mer.mer_in_codigo
                             join en in db.tb_endereco on ped.end_in_codigo
                             equals en.end_in_codigo
                             where ped.cli_in_codigo == Id
                             select new ConsultaPedidosCliente
                             {
                                 Id = ped.ped_in_codigo,
                                 Valor = ped.ped_dec_valor,
                                 Status = ped.ped_status,
                                 Data = ped.data_dt_pedido,
                                 Mercado = mer.mer_st_nome,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Logradouro = en.end_st_log
                             }
                             ).OrderBy(u => u.Status).ToList();
                return View(model);
            }
        }

        public ActionResult VisualizarPedido(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from en in db.tb_endereco join mer
                             in db.tb_mercado on en.end_in_codigo 
                             equals mer.end_in_codigo join ped
                             in db.tb_pedido on mer.mer_in_codigo 
                             equals ped.mer_in_codigo where 
                             ped.ped_in_codigo == id
                             select new VisualizarPedido
                             {
                                 Mercado_Ou_Cliente = mer.mer_st_nome,
                                 Data = ped.data_dt_pedido,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Numero = en.end_st_num,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Status = ped.ped_status,
                                 Valor = ped.ped_dec_valor,
                                 PedID = ped.ped_in_codigo
                             }
                             ).First();
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Cliente");
        }
    }
}