//Repositorio genérico
using AppMvcBasica.Models;
using System.Linq.Expressions;

namespace DevIO.Business.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        //A camada de negócios não conhece a camada de acesso a dados, então ela se comunica por essa interface genérica que será injetada por dependência
        Task Adicionar(TEntity entity);
        Task<TEntity> ObterPorId(Guid id);
        Task<List<TEntity>> ObterTodos();
        Task Atualizar(TEntity entity);
        Task Remover(Guid id);

        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate); //Buscar por qualquer parâmetro da entidade
        Task<int> SaveChanges();
    }
}
