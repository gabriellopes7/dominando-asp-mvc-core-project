using AppMvcBasica.Models;
using Microsoft.EntityFrameworkCore;

namespace DevIO.Data.Context
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options)
        {
            //Problema de tracking behavior
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        //Configuração para registrar nossos Mappings, via reflection
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Evitar que ao esquecer de mapear alguma coisa, a gente crie a coluna como varchar max
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                .Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");



            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            //Desativar o delete cascade, pois é perigoso, validar
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull; //Evitar que leve os filhos da entidade junto, ao deletar

            base.OnModelCreating(modelBuilder);
        }
    }
}
