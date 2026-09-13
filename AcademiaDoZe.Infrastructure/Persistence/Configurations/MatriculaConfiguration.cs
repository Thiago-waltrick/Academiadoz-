// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Persistence.Configurations
{
    public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.ToTable("tb_matricula");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.AlunoId).HasColumnName("aluno_id");
            builder.Property(m => m.Plano).HasColumnName("plano");
            builder.Property(m => m.DataInicio).HasColumnName("data_inicio");
            builder.Property(m => m.DataFim).HasColumnName("data_fim");

            // FK para aluno (não há navegação em Matricula, configuramos pelo Id)
            builder.HasOne<Aluno>().WithMany().HasForeignKey("AlunoId").HasConstraintName("fk_matricula_aluno");

            builder.HasIndex(m => m.AlunoId).HasDatabaseName("idx_matricula_aluno");
        }
    }
}
