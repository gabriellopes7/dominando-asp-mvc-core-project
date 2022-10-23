namespace AppMvcBasica.Models
{
    public class Produto : Entity
    {

        public Guid FornecedorId { get; set; } //Chave estrangeira do fornecedor


        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Imagem { get; set; }

        public decimal Valor { get; set; }


        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        /* EF Relation para o entity framework entender o relacionamento */
        public Fornecedor Fornecedor { get; set; }
    }
}
