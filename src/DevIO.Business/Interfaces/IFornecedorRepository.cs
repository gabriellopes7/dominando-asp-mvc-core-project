using AppMvcBasica.Models;

namespace DevIO.Business.Interfaces
{
    public interface IFornecedorRepository : IRepository<Fornecedor>
    {
        //Não precisa de métodos, pq já é extensão de IRepository
        Task<Fornecedor> ObterFornecedorEndereco(Guid id); //Alem do fornecedor, obter o endereço
        Task<Fornecedor> ObterFornecedorProdutosEndereco(Guid id); //Alem do fornecedor, ter os produtos e endereço
    }
}
