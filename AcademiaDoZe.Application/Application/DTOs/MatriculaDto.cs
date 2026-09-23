// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Application.Enums;

namespace AcademiaDoZe.Application.DTOs
{
    public class MatriculaDto
    {
        public int Id { get; set; }

        // Referência ao aluno
        public int AlunoId { get; set; }
        public AlunoDto Aluno { get; set; } = new AlunoDto();

        // Plano e datas
        public AppMatriculaPlano Plano { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        // Campos do material de referência
        public string Objetivo { get; set; } = string.Empty;
        public AppMatriculaRestricoes Restricoes { get; set; } = AppMatriculaRestricoes.Nenhuma;
        public string? ObsRestricao { get; set; }

        // Laudo médico / arquivo médico associado (opcional)
        public ArquivoDto? LaudoMedico { get; set; }
    }
}
