using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using FI.AtividadeEntrevista.UTIL;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View(new Cliente());
        }

        private JsonResult GravarCliente(ClienteModel model, bool alteracao)
        {
            RetirarMascaraCPF(model);

            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                if (!ValidacaoCPF.ValidaCPF(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Format("O CPF {0} informado está inválido.", model.CPF));
                }

                if (bo.VerificarExistencia(CriarClienteAPartirViewModel(model)))
                {
                    Response.StatusCode = 400;
                    return Json(string.Format("O CPF {0} já foi cadastrado.", model.CPF));
                }

                if (alteracao)
                {
                    bo.Alterar(CriarClienteAPartirViewModel(model));

                    Response.StatusCode = 200;
                    return Json("Cadastro alterado com sucesso");
                }
                else
                {
                    bo.Incluir(CriarClienteAPartirViewModel(model));

                    Response.StatusCode = 200;
                    return Json("Cadastro efetuado com sucesso");
                }
            }
        }


        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            return GravarCliente(model, false);
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            return GravarCliente(model, true);           
        }

        private void RetirarMascaraCPF(ClienteModel model)
        {
            model.CPF = model.CPF.Replace(".", "").Replace("-", "");

            foreach (var beneficiario in model.Beneficiarios)
            {
                beneficiario.CPF = beneficiario.CPF.Replace(".", "").Replace("-", "");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };

                model.Beneficiarios.AddRange(cliente.Beneficiarios.Select(b => new BeneficiarioModel() { Id = b.Id, Nome = b.Nome, CPF = b.CPF }).ToList());
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public Cliente CriarClienteAPartirViewModel(ClienteModel model)
        {
            var cliente = new Cliente()
            {
                Id = model.Id,
                CEP = model.CEP,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone,
                CPF = model.CPF
            };

            cliente.Beneficiarios.AddRange(model.Beneficiarios.Select(b => new Beneficiario() { Id = b.Id, Nome = b.Nome, CPF = b.CPF }).ToList());
            cliente.BeneficiariosRemovidos.AddRange(model.BeneficiariosRemovidos.Select(b => new Beneficiario() { Id = b.Id, Nome = b.Nome, CPF = b.CPF }).ToList());

            return cliente;
        }
    }
}