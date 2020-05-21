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
    }
}