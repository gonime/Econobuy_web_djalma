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
        public ActionResult CadastraCliente(CadastroCliente cad, string submitButton)
        {
            switch (submitButton)
            {
                case "Cadastrar":
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
                            return RedirectToAction("Home", "Cliente");
                        }
                    };
                case "Validar CEP":
                    try
                    {
                        var ws = new WSCorreios.AtendeClienteClient();
                        var resposta = ws.consultaCEP(cad.CEP);
                        cad.Logradouro = resposta.end;
                        cad.Bairro = resposta.bairro;
                        cad.Cidade = resposta.cidade;
                        cad.UF = resposta.uf;
                        return View("Cadastro", cad);
                    }
                    catch 
                    {
                        return View("Cadastro", cad);
                    }
            }
            return View("Cadastro", cad);
        }


        public ActionResult AlterarUsuario()
        {
            int Id = Convert.ToInt32(Session["mercadoID"]);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var alter = (from mer in db.tb_mercado
                             join end in db.tb_endereco on
                             mer.end_in_codigo equals end.end_in_codigo
                             join merImg
                             in db.tb_mercado_img on mer.mer_in_codigo
                             equals merImg.mer_in_codigo
                             where mer.mer_in_codigo == Id
                             select new AlteraMercado
                             {
                                 User = mer.mer_st_user,
                                 Senha = mer.mer_st_senha,
                                 Email = mer.mer_st_email,
                                 Telefone_1 = end.end_st_tel1,
                                 Telefone_2 = end.end_st_tel2,
                                 EndID = end.end_in_codigo,
                                 MerID = mer.mer_in_codigo,
                                 ImgID = merImg.mer_img_in_codigo
                             });
                if (alter != null)
                {
                    AlteraMercado alt = alter.First();
                    return View(alt);
                }
                else return View();
            }
        }

        [HttpPost]
        public ActionResult AlteraUsuario(AlteraMercado alt, HttpPostedFileBase imgMercado)
        {
            HttpPostedFileBase file = Request.Files["img"];
            if (file.ContentLength > 0) alt.imgMercado = ConvertToBytes(file);
            using (EconobuyEntities db = new EconobuyEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View("AlterarUsuario", alt);
                }
                else
                {
                    tb_endereco end = db.tb_endereco.Find(alt.EndID);
                    tb_mercado mer = db.tb_mercado.Find(alt.MerID);
                    tb_mercado_img img = db.tb_mercado_img.Find(alt.ImgID);
                    if (alt != null)
                    {
                        mer.mer_st_user = alt.User;
                        mer.mer_st_senha = alt.Senha;
                        mer.mer_st_email = alt.Email;
                        end.end_st_tel1 = alt.Telefone_1;
                        end.end_st_tel2 = alt.Telefone_2;
                        if (alt.imgMercado != null) img.mer_img = alt.imgMercado;
                    }
                    db.SaveChanges();
                    return RedirectToAction("ConsultarProdutos", "Mercado");
                }
            }
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
                var model = (from en in db.tb_endereco
                             join mer
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
                return View(model);
            }
        }

        public ActionResult VisualizarItensPedido(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from ped in db.tb_pedido
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
                                 valor_total = prod.prod_dec_valor_un * itn.item_in_qtde
                             }
                             ).OrderBy(u => u.Nome).ToList();
                return View(model);
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
                             join
mer in db.tb_mercado on av.mer_in_codigo
equals mer.mer_in_codigo
                             join en
    in db.tb_endereco on mer.end_in_codigo
    equals en.end_in_codigo
                             where
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

        public ActionResult ListarProdutosModoTradicional(int id)
        {
            using (EconobuyEntities db = new EconobuyEntities())
            {
                var model = (from prod in db.tb_produto join cat01
                            in db.tb_categoria_n01 on prod.cat01_in_codigo
                            equals cat01.cat01_in_codigo join cat02 in
                            db.tb_categoria_n02 on prod.cat02_in_codigo
                            equals cat02.cat02_in_codigo join cat03 in
                            db.tb_categoria_n03 on prod.cat03_in_codigo
                            equals cat03.cat03_in_codigo where
                            prod.mer_in_codigo == id
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
                             select new CadastroProduto
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

        //public ActionResult AdicionaAoCarrinho(int ProdID_, string ProdNome_, int Qtde_, decimal ValorUnidade_, decimal ValorTotal_)
        //{
        //    using (TempItemDBContext db = new TempItemDBContext())
        //    {
        //        var Carrinho = new Carrinho
        //        {
        //            ProdID = ProdID_,
        //            ProdNome = ProdNome_,
        //            Qtde = Qtde_,
        //            ValorUnidade = ValorUnidade_,
        //            ValorTotal = ValorTotal_
        //        };
        //        db.Carrinhos.Add(Carrinho);
        //        db.SaveChanges();
        //        return View();
        //    }
        //}
    }
}