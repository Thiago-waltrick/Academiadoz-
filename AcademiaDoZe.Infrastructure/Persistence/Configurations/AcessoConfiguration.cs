// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Persistence.Configurations
{
    public class AcessoConfiguration : IEntityTypeConfiguration<AcessoAluno>
    {
        public void Configure(EntityTypeBuilder<AcessoAluno> builder)
        {
            // Conforme requisito mapeamos a entidade de acesso para a tabela tb_acesso
            builder.ToTable("tb_acesso");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AlunoId).HasColumnName("aluno_id");
            builder.Property(a => a.DataAcesso).HasColumnName("data_acesso");

            // FK para aluno (Matricula/Aluno)
            builder.HasOne<Aluno>().WithMany().HasForeignKey("AlunoId").HasConstraintName("fk_acesso_aluno");

            builder.HasIndex(a => a.AlunoId).HasDatabaseName("idx_acesso_aluno");
        }
    }
}
