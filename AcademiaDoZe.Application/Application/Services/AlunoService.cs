// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _repo;

        public AlunoService(IAlunoRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyCollection<AlunoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<AlunoDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = _repo.GetAll();
                return (IReadOnlyCollection<AlunoDto>)list.Select(a => a.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task<AlunoDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = _repo.GetById(id);
            return Task.FromResult(entity.ToDto());
        }

        public Task<AlunoDto> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
        {
            var entity = _repo.GetByCpf(cpf);
            return Task.FromResult(entity.ToDto());
        }

        public Task CriarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            // Use domain factories
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
            // DTO does not carry número/complemento; use safe defaults to satisfy domain Endereco.Criar requirements
            var endereco = Domain.ValueObjects.Endereco.Criar(logradouro!, "S/N", string.Empty).Value;

            var res = Aluno.Criar(dto.Id, dto.Nome, cpfVo, emailVo, dto.DataNascimento, telVo!, endereco);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao criar Aluno: " + string.Join(',', res.Notifications));

            _repo.Add(res.Value);
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));
            var existing = _repo.GetById(dto.Id);
            // Re-create via factory to validate
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

            var res = Aluno.Criar(dto.Id, dto.Nome, cpfVo, emailVo, dto.DataNascimento, telVo!, endereco);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao atualizar Aluno: " + string.Join(',', res.Notifications));

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
