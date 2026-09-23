// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class AcessoAlunoService : IAcessoAlunoService
    {
        private readonly IAcessoAlunoRepository _repo;

        public AcessoAlunoService(IAcessoAlunoRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyCollection<AcessoAluno> ObterPorAlunoId(int alunoId)
        {
            return _repo.GetByAlunoId(alunoId);
        }

        public AcessoAluno RegistrarAcesso(int alunoId)
        {
            var res = AcessoAluno.Criar(0, alunoId);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao criar AcessoAluno: " + string.Join(',', res.Notifications));
            _repo.Add(res.Value);
            return res.Value;
        }

        public void Remover(int id)
        {
            var ent = _repo.GetById(id);
            _repo.Remove(ent);
        }
    }
}
