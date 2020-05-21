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
                    Session["mercadoID"] = user.mer_in_codigo;
                    Session["mercadoNome"] = user.mer_st_nome;
                    return RedirectToAction("Index", "Home");
                }
            }
        }
    }
}