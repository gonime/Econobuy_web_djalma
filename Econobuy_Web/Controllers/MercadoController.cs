using Econobuy_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econobuy_Web.Controllers
{
    public class MercadoController : Controller
    {
        // GET: Mercado
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginMercado(tb_mercado user)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var userDetail = db.tb_mercado.Where(x => x.mer_st_user == user.mer_st_user && x.mer_st_senha == user.mer_st_senha).FirstOrDefault();
                if (userDetail == null)
                {
                    TempData["Erro"] = "Usuário ou senha inválidos";
                    return View("Index", user);
                }
                else
                {
                    Session["mercadoID"] = userDetail.mer_in_codigo;
                    Session["mercadoNome"] = userDetail.mer_st_nome;
                    return RedirectToAction("Home", "Mercado");
                }
            }
        }
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ConsultarPedidos()
        {
            int Id = Convert.ToInt32(Session["mercadoID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from ped in db.tb_pedido join cli in db.tb_cliente on
                             ped.cli_in_codigo equals cli.cli_in_codigo
                             join en in db.tb_endereco on ped.end_in_codigo
                             equals en.end_in_codigo where ped.mer_in_codigo == Id
                             select new ConsultaPedidosMercado
                             {
                                 Id = ped.ped_in_codigo,
                                 Valor = ped.ped_dec_valor,
                                 Status = ped.ped_status,
                                 Data = ped.data_dt_pedido,
                                 Cliente = cli.cli_st_nome,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Logradouro = en.end_st_log
                             }
                             ).OrderBy(u => u.Status).ToList();
                return View(model);
            }
        }

        public ActionResult AlterarUsuario()
        {
            return View();
        }

        public ActionResult ConsultarAvaliacoes()
        {
            return View();
        }

        public ActionResult ConsultarProdutos()
        {
            return View();
        }

        public ActionResult CadastrarProduto()
        {
            return View();
        }

        
    }
}