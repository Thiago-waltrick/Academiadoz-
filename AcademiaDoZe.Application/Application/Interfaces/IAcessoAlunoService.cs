// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAcessoAlunoService
    {
        // Retorna registros de acesso para um aluno. Não existe DTO de acesso na camada Application;
        // utilizamos diretamente a entidade de domínio AcessoAluno conforme artefatos existentes.
        IReadOnlyCollection<AcessoAluno> ObterPorAlunoId(int alunoId);

        // Registra acesso para alunoId e retorna a entidade criada.
        AcessoAluno RegistrarAcesso(int alunoId);

        void Remover(int id);
    }
}
