// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Domain.Enums
{
    [Flags]
    public enum MatriculaRestricoes
    {
        Nenhuma = 0,
        Piscina = 1 << 0,
        SalaDeMusculacao = 1 << 1,
        HorarioNoturno = 1 << 2,
        AtividadesEspecificas = 1 << 3,
        Diabetes = 1 << 4,
        PressaoAlta = 1 << 5,
        Labirintite = 1 << 6,
        Alergias = 1 << 7,
        ProblemasRespiratorios = 1 << 8,
        RemedioContinuo = 1 << 9
    }
}
