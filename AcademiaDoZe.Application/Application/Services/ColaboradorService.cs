// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Extensions;
using EnumExtensions = AcademiaDoZe.Application.Extensions.EnumExtensions;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorRepository _repo;

        public ColaboradorService(IColaboradorRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyCollection<ColaboradorDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<ColaboradorDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = _repo.GetAll();
                return (IReadOnlyCollection<ColaboradorDto>)list.Select(c => c.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task<ColaboradorDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetById(id);
            return Task.FromResult(ent.ToDto());
        }

        public Task<ColaboradorDto> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetByCpf(cpf);
            return Task.FromResult(ent.ToDto());
        }

        public Task CriarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            var cpfVo = Domain.ValueObjects.Cpf.Criar(dto.Cpf).Value;
            var emailVo = Domain.ValueObjects.Email.Criar(dto.Email).Value;
            var telVo = string.IsNullOrWhiteSpace(dto.Telefone) ? null : Domain.ValueObjects.Telefone.Criar(dto.Telefone).Value;

            var logDto = dto.Endereco?.Nome != null ? dto.Endereco : null;
            Domain.Entities.Logradouro? logradouro = null;
            if (logDto != null)
            {
                var cepVo = string.IsNullOrWhiteSpace(logDto.Cep) ? null : Domain.ValueObjects.Cep.Criar(logDto.Cep).Value;
                logradouro = Domain.Entities.Logradouro.Criar(logDto.Nome, logDto.Bairro, logDto.Cidade, logDto.Estado, cepVo).Value;
            }
            var endereco = Domain.ValueObjects.Endereco.Criar(logradouro!, "S/N", string.Empty).Value;

            var res = Colaborador.Criar(dto.Id, dto.Nome, cpfVo, emailVo, dto.DataNascimento, telVo!, endereco, dto.Tipo.ToDomain(), dto.Vinculo.ToDomain(), dto.DataAdmissao);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao criar Colaborador: " + string.Join(',', res.Notifications));

            // Password handling: if DTO had a password field (it doesn't), use PasswordHasher here. Domain currently does not hold password property.
            _repo.Add(res.Value);
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));
            var existing = _repo.GetById(dto.Id);

            var cpfVo = Domain.ValueObjects.Cpf.Criar(dto.Cpf).Value;
            var emailVo = Domain.ValueObjects.Email.Criar(dto.Email).Value;
            var telVo = string.IsNullOrWhiteSpace(dto.Telefone) ? null : Domain.ValueObjects.Telefone.Criar(dto.Telefone).Value;

            var logDto2 = dto.Endereco?.Nome != null ? dto.Endereco : null;
            Domain.Entities.Logradouro? logradouro2 = null;
            if (logDto2 != null)
            {
                var cepVo2 = string.IsNullOrWhiteSpace(logDto2.Cep) ? null : Domain.ValueObjects.Cep.Criar(logDto2.Cep).Value;
                logradouro2 = Domain.Entities.Logradouro.Criar(logDto2.Nome, logDto2.Bairro, logDto2.Cidade, logDto2.Estado, cepVo2).Value;
            }
            var endereco = Domain.ValueObjects.Endereco.Criar(logradouro2!, "S/N", string.Empty).Value;

            var res = Colaborador.Criar(dto.Id, dto.Nome, cpfVo, emailVo, dto.DataNascimento, telVo!, endereco, dto.Tipo.ToDomain(), dto.Vinculo.ToDomain(), dto.DataAdmissao);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao atualizar Colaborador: " + string.Join(',', res.Notifications));

            _repo.Update(res.Value);
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        {
            var existing = _repo.GetById(id);
            _repo.Remove(existing);
            return Task.CompletedTask;
        }
    }
}
