// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Application.Security;

namespace AcademiaDoZe.Application.Services
{
    public class AcessoColaboradorService : IAcessoColaboradorService
    {
        private readonly IAcessoColaboradorRepository _repo;

        public AcessoColaboradorService(IAcessoColaboradorRepository repo)
        {
            _repo = repo;
        }

        public IReadOnlyCollection<AcessoColaborador> ObterPorColaboradorId(int colaboradorId)
        {
            return _repo.GetByColaboradorId(colaboradorId);
        }

        public AcessoColaborador RegistrarAcesso(int colaboradorId)
        {
            // Criar uma senha aleatória temporária, hasheá-la com PasswordHasher e armazenar como Senha value object
            var plain = System.Guid.NewGuid().ToString().Substring(0, 8);
            var hashed = PasswordHasher.Hash(plain);
            var senhaRes = Domain.ValueObjects.Senha.Criar(hashed);
            if (senhaRes.IsFailure) throw new System.InvalidOperationException("Falha ao criar Senha para AcessoColaborador: " + string.Join(',', senhaRes.Notifications));
            var res = AcessoColaborador.Criar(0, colaboradorId, senhaRes.Value);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao criar AcessoColaborador: " + string.Join(',', res.Notifications));
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
