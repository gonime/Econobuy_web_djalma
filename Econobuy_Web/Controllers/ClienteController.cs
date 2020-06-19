using Econobuy_Web.Models;
using Econobuy_Web.WSCorreios;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Econobuy_Web.Controllers
{
    public class ClienteController : Controller
    {
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

        public ActionResult RecuperarSenha(tb_cliente cli)
        {
            return View();
        }

        public ActionResult RecuperaSenha(tb_cliente cli)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int cliID = db.tb_cliente.Where(x => x.cli_st_email == cli.cli_st_email).Select(x => x.cli_in_codigo).SingleOrDefault();
                if (cliID > 0)
                {
                    Random rnd = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    string senha = new string(Enumerable.Repeat(chars, 10)
                      .Select(s => s[rnd.Next(s.Length)]).ToArray());
                    tb_cliente cl = db.tb_cliente.Find(cliID);
                    cl.cli_st_senha = senha;
                    db.SaveChanges();
                    EnviaSenhaEmail(cl.cli_st_email, cl.cli_st_user, senha);
                    TempData["Query"] = "Seus dados de acesso foram enviados para seu e-mail";
                    return View("RecuperarSenha", cli);
                }
                else
                {
                    TempData["Erro"] = "E-mail não encontrado no sistema";
                    return View("RecuperarSenha", cli);
                }
            }
        }

        public void EnviaSenhaEmail(string email, string usuario, string senha)
        {
            try
            {
                string msg = "Seguem os dados de acesso solicitados: \n\n Usuario: " + usuario + " \n Senha: " + senha +
                    " \n\n Caso não tenha feito esta solicitação entre em contato conosco respondendo este e-mail. \n\n Atenciosamente, \n Equipe Econobuy.";
                MailMessage mensagemEmail = new MailMessage("sistemaeconobuy@gmail.com", email, "Recuperação de Senha - Econobuy", msg);
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sistemaeconobuy@gmail.com", "Nmb159nmb!");
                client.Send(mensagemEmail);
            }
            finally { }
            return;
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
            var av = new tb_avaliacao_cliente
            {
                av_cli_dec_nota = 0
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
                    av.cli_in_codigo = cli.cli_in_codigo;
                    db.tb_avaliacao_cliente.Add(av);
                    db.SaveChanges();
                    Session["clienteID"] = cli.cli_in_codigo;
                    Session["clienteNome"] = cli.cli_st_nome;
                    return RedirectToAction("Home", "Cliente");
                }
            };
        }


        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
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
                var model = (from en in db.tb_endereco  join mer
                            in db.tb_mercado on en.end_in_codigo
                            equals mer.end_in_codigo
                            join ped
                            in db.tb_pedido on mer.mer_in_codigo
                            equals ped.mer_in_codigo
                            where
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
                var itens = (from ped in db.tb_pedido
                             join
                            itn in db.tb_item on ped.ped_in_codigo
                            equals itn.ped_in_codigo
                             join
                            prod in db.tb_produto on itn.prod_in_codigo
                            equals prod.prod_in_codigo
                             where ped.ped_in_codigo == id
                             select new VisualizarItens
                             {
                                 Nome = prod.prod_st_nome,
                                 valor_un = prod.prod_dec_valor_un,
                                 Qtde = itn.item_in_qtde,
                                 valor_total = prod.prod_dec_valor_un * itn.item_in_qtde,
                                 ProdID = prod.prod_in_codigo
                             }
                             ).OrderBy(u => u.Nome).ToList();
                model.Itens = itens;
                return View(model);
            }
        }

        public ActionResult marcarPedidoEntregue(int id)
        {
            using(EconobuyEntities db = new EconobuyEntities())
            {
                tb_pedido end = db.tb_pedido.Find(id);
                end.ped_status = "Entregue";
                db.SaveChanges();
                return RedirectToAction("avaliaPedidoEntregue", "Cliente", new { id = id });
            }
        }

        public ActionResult avaliaPedidoEntregue(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                TempData["pedID"] = id;
                int merID = db.tb_pedido.Where(x => x.ped_in_codigo == id).Select(x => x.mer_in_codigo).SingleOrDefault();
                int AvMerID = db.tb_avaliacao_mercado.Where(x => x.mer_in_codigo == merID).Select(x => x.av_mer_in_codigo).SingleOrDefault();
                TempData["AvMerID"] = AvMerID;
                return View();
            }
        }

        public ActionResult avaliarPedidoEntregue(tb_pedido_avaliacao_cliente pedav)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var av = new tb_pedido_avaliacao_cliente
                {
                    av_mer_in_codigo = Convert.ToInt32(TempData["AvMerID"]),
                    ped_av_cli_dec_nota = pedav.ped_av_cli_dec_nota,
                    ped_av_cli_st_descricao = pedav.ped_av_cli_st_descricao,
                    ped_in_codigo = Convert.ToInt32(TempData["pedID"]),
                    ped_bit_active = true
                };
                db.tb_pedido_avaliacao_cliente.Add(av);
                db.SaveChanges();
                return RedirectToAction("VisualizarPedido", "Cliente", new { Id = av.ped_in_codigo });
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Cliente");
        }

        public ActionResult NovoPedidoTradicional()
        {
            int Id = Convert.ToInt32(Session["clienteID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == Id).Select(x => x.end_in_codigo).SingleOrDefault();
                tb_endereco end = db.tb_endereco.Find(end_id);
                var model = (from av in db.tb_avaliacao_mercado
                             join mer in db.tb_mercado on av.mer_in_codigo
                             equals mer.mer_in_codigo join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             en.end_st_uf == end.end_st_uf &&
                             en.end_st_cidade == end.end_st_cidade
                             select new ListaMercadosModoTradicional
                             {
                                 Mercado = mer.mer_st_nome,
                                 Avaliacao = av.av_mer_dec_nota,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Numero = en.end_st_num,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Email = mer.mer_st_email,
                                 MerID = mer.mer_in_codigo
                             }
                             ).OrderBy(u => u.Mercado).ToList();
                return View(model);
            }
        }
        public ActionResult excluirPedidoTrad()
        {
            Session["mercadoTradID"] = null;
            Session["Itens"] = null;
            return RedirectToAction("NovoPedidoTradicional", "Cliente");
        }


        public ActionResult MostraLogoMercado(int Id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_mercado_img where img.mer_in_codigo == Id select img.mer_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        public ActionResult ListarProdutosModoTradicional(int id)
        {
            Session["mercadoTradID"] = id;
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto join cat01
                            in db.tb_categoria_n01 on prod.cat01_in_codigo
                            equals cat01.cat01_in_codigo join cat02 in
                            db.tb_categoria_n02 on prod.cat02_in_codigo
                            equals cat02.cat02_in_codigo join cat03 in
                            db.tb_categoria_n03 on prod.cat03_in_codigo
                            equals cat03.cat03_in_codigo where
                            prod.mer_in_codigo == id &&
                            prod.prod_bit_trad_active == true &&
                            prod.prod_bit_active == true
                            select new ConsultaProdutos
                            {
                                Id = prod.prod_in_codigo,
                                Nome = prod.prod_st_nome,
                                Preco = prod.prod_dec_valor_un,
                                Categoria_01 = cat01.cat01_st_nome,
                                Categoria_02 = cat02.cat02_st_nome,
                                Categoria_03 = cat03.cat03_st_nome
                            }
                            ).OrderBy(u => u.Nome).ToList();
                return View(model);
            }
        }

        public ActionResult MostraImagemProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var result = (from img in db.tb_produto_img where img.prod_in_codigo == id select img.prod_img).First();
                if (result.Any())
                {
                    byte[] logo = result;
                    return File(logo, "image/jpg");
                }
                else return null;
            }
        }

        public ActionResult MostraProduto(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto
                             join cat01 in db.tb_categoria_n01 on prod.cat01_in_codigo
                             equals cat01.cat01_in_codigo
                             join cat02 in db.tb_categoria_n02 on prod.cat02_in_codigo
                             equals cat02.cat02_in_codigo
                             join cat03 in db.tb_categoria_n03 on prod.cat03_in_codigo
                             equals cat03.cat03_in_codigo
                             where prod.prod_in_codigo == id
                             select new CarrinhoTrad
                             {
                                 Nome = prod.prod_st_nome,
                                 Descricao = prod.prod_st_descricao,
                                 Valor = prod.prod_dec_valor_un,
                                 Cat01 = cat01.cat01_st_nome,
                                 Cat02 = cat02.cat02_st_nome,
                                 Cat03 = cat03.cat03_st_nome,
                                 ProdID = prod.prod_in_codigo
                             }
                             ).First();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult AdicionaAoCarrinhoTrad(CarrinhoTrad item)
        {

            if (ModelState.IsValid)
            {
                item.ValorTotal = item.Valor * item.Qtde;
                CarrinhoTradTemp.ArmazenaItens(item);
                return RedirectToAction("ListaCarrinhoTrad", "Cliente");
            }
                return View();
            
        }

        public ActionResult ListaCarrinhoTrad()
        {
            var carrinho = CarrinhoTradTemp.RetornaItens();
            return View(carrinho);
        }

        public ActionResult DeletaItemCarrinhoTrad(int id)
        {
            CarrinhoTradTemp.RemoveItem(id);
            return RedirectToAction("ListaCarrinhoTrad", "Cliente");
        }

        public ActionResult FinalizaPedidoTrad()
        {
            decimal valor = 0;
            var carrinho = CarrinhoTradTemp.RetornaItens().ToList();
            foreach (var item in carrinho)
            {
                valor += item.ValorTotal;
            }
            int Id = Convert.ToInt32(Session["mercadoTradID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from av in db.tb_avaliacao_mercado
                             join mer in db.tb_mercado on av.mer_in_codigo
                             equals mer.mer_in_codigo join en
                             in db.tb_endereco on mer.end_in_codigo
                             equals en.end_in_codigo where
                             mer.mer_in_codigo == Id
                             select new FinalizaPedidoTrad
                             {
                                 Mercado = mer.mer_st_nome,
                                 AvMer = av.av_mer_dec_nota,
                                 CEP = en.end_st_CEP,
                                 Cidade = en.end_st_cidade,
                                 Bairro = en.end_st_bairro,
                                 Logradouro = en.end_st_log,
                                 Complemento = en.end_st_compl,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = en.end_st_tel1,
                                 Telefone_2 = en.end_st_tel2,
                                 Valor = valor
                             }
                             ).First();
                model.Carrinho = carrinho;
                return View(model);
            }
        }

        public ActionResult FinalizarPedidoTrad(FinalizaPedidoTrad tr)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                int cli_id = Convert.ToInt32(Session["clienteID"]);
                int end_id = db.tb_cliente.Where(x => x.cli_in_codigo == cli_id).Select(x => x.end_in_codigo).SingleOrDefault();
                int mer_id = Convert.ToInt32(Session["mercadoTradID"]);
                var ped = new tb_pedido
                {
                    cli_in_codigo = cli_id,
                    mer_in_codigo = mer_id,
                    data_dt_pedido = DateTime.Now,
                    end_in_codigo = end_id,
                    ped_status = "Aguardando",
                    ped_st_msg = "",
                    ped_dec_valor = 0
                };
                db.tb_pedido.Add(ped);
                decimal valor = 0;
                var carrinho = CarrinhoTradTemp.RetornaItens().ToList();
                foreach (var pro in carrinho)
                {
                    var item = new tb_item
                    {
                        prod_in_codigo = pro.ProdID,
                        ped_in_codigo = ped.ped_in_codigo,
                        item_in_qtde = pro.Qtde,
                        item_dec_valor = pro.ValorTotal
                    };
                    valor += pro.ValorTotal;
                    db.tb_item.Add(item);
                }
                ped.ped_dec_valor = valor;
                db.SaveChanges();
                Session["mercadoTradID"] = null;
                Session["Itens"] = null;
                return RedirectToAction("VisualizarPedido", "Cliente", new { Id = ped.ped_in_codigo });
            }
        }


    }
}