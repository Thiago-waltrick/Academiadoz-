// Thiago Augusto Ruskowski Waltrick
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Persistence.Configurations
{
    public class LogradouroConfiguration : IEntityTypeConfiguration<Logradouro>
    {
        public void Configure(EntityTypeBuilder<Logradouro> builder)
        {
            builder.ToTable("tb_logradouro");

            // Logradouro no domínio é um VO sem Id; usamos uma chave sombra para persistência inicial
            builder.Property<int>("Id").ValueGeneratedOnAdd();
            builder.HasKey("Id");

            builder.Property(l => l.Nome).HasColumnName("nome").IsRequired().HasMaxLength(200);
            builder.Property(l => l.Bairro).HasColumnName("bairro").HasMaxLength(150);
            builder.Property(l => l.Cidade).HasColumnName("cidade").HasMaxLength(100);
            builder.Property(l => l.Estado).HasColumnName("estado").HasMaxLength(2);

            // Cep é um value object; mapeamos a propriedade interna Codigo como coluna 'cep'
            builder.OwnsOne(l => l.Cep, cp =>
            {
                cp.Property(c => c.Codigo).HasColumnName("cep").HasMaxLength(8);
            });
        }
    }
}
