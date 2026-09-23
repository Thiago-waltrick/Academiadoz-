// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAcessoColaboradorService
    {
        // Não existe DTO específico para acesso de colaborador na camada Application;
        // retornamos a entidade de domínio AcessoColaborador.
        IReadOnlyCollection<AcessoColaborador> ObterPorColaboradorId(int colaboradorId);

        // Registra um acesso para o colaborador e retorna a entidade criada.
        AcessoColaborador RegistrarAcesso(int colaboradorId);

        void Remover(int id);
    }
}
