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
            // Mapeamento atualizado para tb_acesso_aluno com campos de entrada/saida
            builder.ToTable("tb_acesso_aluno");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AlunoId).HasColumnName("aluno_id");
            builder.Property(a => a.Entrada).HasColumnName("entrada");
            builder.Property(a => a.Saida).HasColumnName("saida");

            // FK para aluno
            builder.HasOne<Aluno>().WithMany().HasForeignKey("AlunoId").HasConstraintName("fk_acesso_aluno");

            builder.HasIndex(a => a.AlunoId).HasDatabaseName("idx_acesso_aluno");
        }
    }
}
