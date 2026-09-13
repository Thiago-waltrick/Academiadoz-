// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Persistence
{
    public class AcademiaDbContext : DbContext
    {
        public AcademiaDbContext(DbContextOptions<AcademiaDbContext> options) : base(options)
        {
        }

        // DbSets principais
        public DbSet<Logradouro> Logradouros { get; set; } = null!;
        public DbSet<Aluno> Alunos { get; set; } = null!;
        public DbSet<Colaborador> Colaboradores { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;
        // mapeado para a tabela tb_acesso conforme requisito (usa AcessoAluno como representação de acesso de alunos)
        public DbSet<AcessoAluno> Acessos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Configurations.LogradouroConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AlunoConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ColaboradorConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.MatriculaConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AcessoConfiguration());
        }
    }
}
