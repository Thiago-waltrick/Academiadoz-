// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Persistence.Configurations
{
    public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder.ToTable("tb_aluno");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nome).HasColumnName("nome").IsRequired().HasMaxLength(200);
            builder.Property(a => a.DataNascimento).HasColumnName("data_nascimento");

            // Value objects
            builder.OwnsOne(a => a.Cpf, cpf => cpf.Property(c => c.Valor).HasColumnName("cpf").IsRequired().HasMaxLength(11));
            builder.OwnsOne(a => a.Email, e => e.Property(p => p.Endereco).HasColumnName("email").HasMaxLength(200));
            builder.OwnsOne(a => a.Telefone, t => t.Property(p => p.Numero).HasColumnName("telefone").HasMaxLength(20));

            // Endereco como owned type com Logradouro embutido
            builder.OwnsOne(a => a.Endereco, end =>
            {
                end.Property(e => e.Numero).HasColumnName("end_numero").HasMaxLength(20);
                end.Property(e => e.Complemento).HasColumnName("end_complemento").HasMaxLength(200);
                end.OwnsOne(e => e.Logradouro, l =>
                {
                    l.Property(p => p.Nome).HasColumnName("logradouro_nome").HasMaxLength(200);
                    l.Property(p => p.Bairro).HasColumnName("logradouro_bairro").HasMaxLength(150);
                    l.Property(p => p.Cidade).HasColumnName("logradouro_cidade").HasMaxLength(100);
                    l.Property(p => p.Estado).HasColumnName("logradouro_estado").HasMaxLength(2);
                    l.OwnsOne(p => p.Cep, c => c.Property(x => x.Codigo).HasColumnName("logradouro_cep").HasMaxLength(8));
                });
            });

            builder.HasIndex(a => a.Nome).HasDatabaseName("idx_aluno_nome");
        }
    }
}
