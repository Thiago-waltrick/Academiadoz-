// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Infrastructure.Persistence.Configurations
{
    public class ColaboradorConfiguration : IEntityTypeConfiguration<Colaborador>
    {
        public void Configure(EntityTypeBuilder<Colaborador> builder)
        {
            builder.ToTable("tb_colaborador");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome).HasColumnName("nome").IsRequired().HasMaxLength(200);
            builder.Property(c => c.DataAdmissao).HasColumnName("data_admissao");
            builder.Property(c => c.DataNascimento).HasColumnName("data_nascimento");

            builder.Property<int>("Tipo").HasColumnName("tipo");
            builder.Property<int>("Vinculo").HasColumnName("vinculo");

            builder.OwnsOne(c => c.Cpf, cpf => cpf.Property(p => p.Valor).HasColumnName("cpf").IsRequired().HasMaxLength(11));
            builder.OwnsOne(c => c.Email, e => e.Property(p => p.Endereco).HasColumnName("email").HasMaxLength(200));
            builder.OwnsOne(c => c.Telefone, t => t.Property(p => p.Numero).HasColumnName("telefone").HasMaxLength(20));

            builder.HasIndex(c => c.Nome).HasDatabaseName("idx_colaborador_nome");
        }
    }
}
