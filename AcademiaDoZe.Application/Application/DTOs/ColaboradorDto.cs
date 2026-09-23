// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Application.Enums;

namespace AcademiaDoZe.Application.DTOs
{
    public class ColaboradorDto : PessoaDto
    {
        public AppColaboradorTipo Tipo { get; set; }
        public AppColaboradorVinculo Vinculo { get; set; }
        public DateTime DataAdmissao { get; set; }
    }
}
