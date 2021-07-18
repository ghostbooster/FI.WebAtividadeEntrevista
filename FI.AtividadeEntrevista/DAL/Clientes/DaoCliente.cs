using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Cliente
    /// </summary>
    internal class DaoCliente : AcessoDados
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <param name="parametros">Objeto de lista de parâmetros</param>
        internal void DefinirValorParametros(DML.Cliente cliente, List<System.Data.SqlClient.SqlParameter> parametros)
        {
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", cliente.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Sobrenome", cliente.Sobrenome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nacionalidade", cliente.Nacionalidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CEP", cliente.CEP));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Estado", cliente.Estado));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Cidade", cliente.Cidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Logradouro", cliente.Logradouro));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Email", cliente.Email));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Telefone", cliente.Telefone));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", cliente.CPF));
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <param name="cliente">Objeto de beneficiário para o cliente</param>
        /// <param name="parametros">Objeto de lista de parâmetros</param>
        internal void DefinirValorParametrosBeneficiarioCliente(DML.Cliente cliente, DML.Beneficiario beneficiario, List<System.Data.SqlClient.SqlParameter> parametros)
        {
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", cliente.Id));
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal long Incluir(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            DefinirValorParametros(cliente, parametros);

            DataSet ds = base.Consultar("FI_SP_IncClienteV2", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);

            GravarBeneficiarios(cliente);

            return ret;
        }

        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal DML.Cliente Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli.FirstOrDefault();
        }

        internal bool VerificarExistencia(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", cliente.Id));
            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", cliente.CPF));

            DataSet ds = base.Consultar("FI_SP_VerificaCliente", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        internal List<Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("iniciarEm", iniciarEm));
            parametros.Add(new System.Data.SqlClient.SqlParameter("quantidade", quantidade));
            parametros.Add(new System.Data.SqlClient.SqlParameter("campoOrdenacao", campoOrdenacao));
            parametros.Add(new System.Data.SqlClient.SqlParameter("crescente", crescente));

            DataSet ds = base.Consultar("FI_SP_PesqCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            int iQtd = 0;

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out iQtd);

            qtd = iQtd;

            return cli;
        }

        /// <summary>
        /// Lista todos os clientes
        /// </summary>
        internal List<DML.Cliente> Listar()
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", 0));

            DataSet ds = base.Consultar("FI_SP_ConsCliente", parametros);
            List<DML.Cliente> cli = Converter(ds);

            return cli;
        }

        /// <summary>
        /// Altera o cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Alterar(DML.Cliente cliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", cliente.Id));

            DefinirValorParametros(cliente, parametros);

            base.Executar("FI_SP_AltCliente", parametros);

            GravarBeneficiarios(cliente);
        }

        /// <summary>
        /// Excluir Cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelCliente", parametros);
        }


        /// <summary>
        /// Grava, exclui ou altera os beneficiarios do cliente.
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public void GravarBeneficiarios(Cliente cliente)
        {
            foreach (var beneficiario in cliente.Beneficiarios)
            {
                if (beneficiario.Id == 0)
                {
                    IncluirBeneficiario(cliente, beneficiario);
                }
                else
                {
                    AlterarBeneficiario(cliente, beneficiario);
                }
            }

            foreach (var beneficiario in cliente.BeneficiariosRemovidos)
            {
                ExcluirBeneficiario(beneficiario);
            }
        }

        /// <summary>
        /// Inclui um novo beneficiario para o cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <param name="cliente">Objeto de beneficiário para o cliente</param>
        public void IncluirBeneficiario(DML.Cliente cliente, DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            DefinirValorParametrosBeneficiarioCliente(cliente, beneficiario, parametros);

            base.Executar("FI_SP_IncBeneficiarioCliente", parametros);
        }

        /// <summary>
        /// Altera o beneficiário do cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <param name="cliente">Objeto de beneficiário do cliente</param>
        public void AlterarBeneficiario(DML.Cliente cliente, DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", beneficiario.Id));

            DefinirValorParametrosBeneficiarioCliente(cliente, beneficiario, parametros);

            base.Executar("FI_SP_AltBeneficiarioCliente", parametros);
        }

        /// <summary>
        /// Altera o beneficiário do cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        /// <param name="cliente">Objeto de beneficiário do cliente</param>
        public void ExcluirBeneficiario(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", beneficiario.Id));

            base.Executar("FI_SP_DelBeneficiarioCliente", parametros);
        }

        private List<DML.Cliente> Converter(DataSet ds)
        {
            List<DML.Cliente> lista = new List<DML.Cliente>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Cliente cli = new DML.Cliente();
                    cli.Id = row.Field<long>("Id");
                    cli.CEP = row.Field<string>("CEP");
                    cli.Cidade = row.Field<string>("Cidade");
                    cli.Email = row.Field<string>("Email");
                    cli.Estado = row.Field<string>("Estado");
                    cli.Logradouro = row.Field<string>("Logradouro");
                    cli.Nacionalidade = row.Field<string>("Nacionalidade");
                    cli.Nome = row.Field<string>("Nome");
                    cli.Sobrenome = row.Field<string>("Sobrenome");
                    cli.Telefone = row.Field<string>("Telefone");
                    cli.CPF = row.Field<string>("CPF");

                    cli.Beneficiarios = ConsultarBeneficiariosCliente(cli.Id);
                    lista.Add(cli);
                }
            }

            return lista;
        }
        private List<DML.Beneficiario> ConverterEmBeneficiarios(DataSet ds)
        {
            List<DML.Beneficiario> beneficiarios = new List<DML.Beneficiario>();

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario beneficiario = new DML.Beneficiario();

                    beneficiario.Id = row.Field<long>("Id");                  
                    beneficiario.Nome = row.Field<string>("Nome");                   
                    beneficiario.CPF = row.Field<string>("CPF");

                    beneficiarios.Add(beneficiario);
                }
            }

            return beneficiarios;
        }

        private List<DML.Beneficiario> ConsultarBeneficiariosCliente(long IdCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", IdCliente));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiariosCliente", parametros);           
            
            return ConverterEmBeneficiarios(ds);
        }
    }
}
